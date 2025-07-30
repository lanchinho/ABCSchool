using Application.Features.Schools;
using Domain;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Schools;

public class SchoolService : ISchoolService
{
    private readonly ApplicationDbContext _context;

    public SchoolService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(School school)
    {
        await _context.AddAsync(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<int> DeleteAsync(School school)
    {
        _context.Remove(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<List<School>> GetAllsync()
    {
        return await _context.School.AsNoTracking().ToListAsync();
    }

    public async Task<School> GetByIdAsync(int schoolId)
    {
        return await _context.School
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == schoolId);
    }

    public async Task<School> GetByNameAsync(string name)
    {
        return await _context.School
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<int> UpdateAsync(School school)
    {
        _context.School.Update(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }
}
