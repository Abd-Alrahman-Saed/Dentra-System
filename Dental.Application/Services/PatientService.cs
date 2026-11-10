using Dental.Core.Entities;
using Dental.Core.Enums;
using Dental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dental.Application.Services
{
    public class PatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPatientAsync(
             Patient patient,
             MedicalHistory medicalHistory,
             DentalHistory dentalHistory)
        {
            patient.MedicalHistory = medicalHistory;
            patient.DentalHistory = dentalHistory;

            patient.Teeth = CreateDefaultTeeth();

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.MedicalHistory)
                .Include(p => p.DentalHistory)
                .Include(p => p.Teeth)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        private static List<Tooth> CreateDefaultTeeth()
        {
            var teeth = new List<Tooth>();

            for (int i = 1; i <= 32; i++)
            {
                ToothArch arch;

                if (i >= 1 && i <= 8)
                    arch = ToothArch.UpperRight;

                else if (i >= 9 && i <= 16)
                    arch = ToothArch.UpperLeft;

                else if (i >= 17 && i <= 24)
                    arch = ToothArch.LowerLeft;

                else
                    arch = ToothArch.LowerRight;

                teeth.Add(new Tooth
                {
                    ToothNumber = i,
                    Name = $"Tooth {i}",
                    Arch = arch,
                    Status = ToothStatus.Healthy
                });
            }

            return teeth;
        }
        public async Task AddPatientNoteAsync(int patientId, string note)
        {
            var patient = await _context.Patients
                .Include(p => p.DentalHistory)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
                return;

            if (patient.DentalHistory == null)
            {
                patient.DentalHistory = new DentalHistory
                {
                    PatientId = patient.Id,
                    Notes = note
                };
            }
            else
            {
                if (string.IsNullOrWhiteSpace(patient.DentalHistory.Notes))
                {
                    patient.DentalHistory.Notes = note;
                }
                else
                {
                    patient.DentalHistory.Notes += Environment.NewLine + Environment.NewLine + note;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}