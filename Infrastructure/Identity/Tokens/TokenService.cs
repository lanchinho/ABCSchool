﻿using ABCShared.Library.Constants;
using ABCShared.Library.Identity;
using ABCShared.Library.Models.Requests.Token;
using ABCShared.Library.Models.Responses.Token;
using Application;
using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity.Tokens;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMultiTenantContextAccessor<ABCSchoolTenantInfo> _tenantContextAccessor;
    private readonly JwtSettings _jwtSettings;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IMultiTenantContextAccessor<ABCSchoolTenantInfo> tenantContextAccessor,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _tenantContextAccessor = tenantContextAccessor;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenResponse> LoginAsync(TokenRequest request)
    {
        if (!_tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            throw new UnauthorizedException(["Tenant Subscription is not active. Contact administrator"]);

        var userInDb = await _userManager.FindByNameAsync(request.Username)
            ?? throw new UnauthorizedException(["Authentication not succcessful."]);

        if (!await _userManager.CheckPasswordAsync(userInDb, request.Password))
            throw new UnauthorizedException(["Incorrect Username or Password."]);

        if (!userInDb.IsActive)
            throw new UnauthorizedException(["User not active, contact administrator"]);

        if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id
            && _tenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
            throw new UnauthorizedException(["Subscription has expired"]);

        return await GenerateTokenInformationAsync(userInDb);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var userPrincipal = GetClaimsPrincipalFromExpiringToken(request.CurrentJwt);
        var userEmail = userPrincipal.GetEmail();

        var userInDb = await _userManager.FindByEmailAsync(userEmail)
            ?? throw new UnauthorizedException(["Authentication failed."]);

        if (userInDb.RefreshToken != request.CurrentRefreshToken || userInDb.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new UnauthorizedException(["Invalid token"]);

        return await GenerateTokenInformationAsync(userInDb);
    }

    private ClaimsPrincipal GetClaimsPrincipalFromExpiringToken(string expiringToken)
    {
        var tkValidationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(expiringToken, tkValidationParams, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException(["Invalid token provided. Failed to generate a new token."]);
        }

        return principal;
    }

    private async Task<TokenResponse> GenerateTokenInformationAsync(ApplicationUser user)
    {
        //Token
        var jwt = await GenerateToken(user);

        //Refresh Token
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.TokenExpiryTimeInDays);
        await _userManager.UpdateAsync(user);

        return new TokenResponse
        {
            Jwt = jwt,
            RefreshToken = user.RefreshToken,
            RefreshTokenexpiryDate = user.RefreshTokenExpiryTime
        };
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        return GenerateEncryptedToken(GetSigningCredentials(), await GetUserClaims(user));
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiryTimeInMinutes),
            signingCredentials: signingCredentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach (var role in userRoles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var currentRole = await _roleManager.FindByNameAsync(role);

            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
            permissionClaims.AddRange(allPermissionsForCurrentRole);
        }

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id),
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.Name, user.FirstName),
            new (ClaimTypes.Surname, user.LastName),
            new (ClaimConstants.Tenant, _tenantContextAccessor.MultiTenantContext.TenantInfo.Id),
            new (ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
        }
        .Union(roleClaims)
        .Union(userClaims)
        .Union(permissionClaims);

        return claims;
    }

    private static string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }
}
