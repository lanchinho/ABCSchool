﻿namespace Infrastructure.Tenancy;

public class TenancyConstants
{
    public const string TenantIdName = "tenant";
    public const string DefaultPassword = "Psw@123";
    public const string FirstName = "Guilherme";
    public const string LastName = "Albuquerque";

    public static class Root
    {
        public const string Id = "root";
        public const string Name = "Root";
        public const string Email = "admin.root@abcschool.com";
    }
}
