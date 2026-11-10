using Dental.Core.Entities;
using Dental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dental.Application.Services
{
    public class ToothService
    {
        private readonly AppDbContext _context;

        public ToothService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tooth>> GetTeethByPatientIdAsync(int patientId)
        {
            return await _context.Teeth
                .Where(t => t.PatientId == patientId)
                .OrderBy(t => t.ToothNumber)
                .ToListAsync();
        }

        public async Task<Tooth?> GetToothDetailsAsync(int toothId)
        {
            return await _context.Teeth
                .Include(t => t.Records)
                    .ThenInclude(r => r.Images)
                .FirstOrDefaultAsync(t => t.Id == toothId);
        }
    }
}