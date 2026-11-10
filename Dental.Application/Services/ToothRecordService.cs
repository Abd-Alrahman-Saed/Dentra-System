using Dental.Core.Entities;
using Dental.Core.Enums;
using Dental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dental.Application.Services
{
    public class ToothRecordService
    {
        private readonly AppDbContext _context;

        private static string GetImagesRootFolder()
        {
            var customPath = Environment.GetEnvironmentVariable("DENTRA_IMAGES_PATH");
            if (!string.IsNullOrWhiteSpace(customPath) && Directory.Exists(customPath))
            {
                return customPath;
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var dir = new DirectoryInfo(baseDir);
            while (dir != null)
            {
                string testPath = Path.Combine(dir.FullName, "Images");
                if (Directory.Exists(testPath))
                {
                    return testPath;
                }
                dir = dir.Parent;
            }

            string defaultPath = Path.Combine(baseDir, "Images");
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
            }
            return defaultPath;
        }

        private string SaveImage(string originalPath, int patientId, int toothId)
        {
            string imagesRootFolder = GetImagesRootFolder();

            string patientFolder = Path.Combine(imagesRootFolder, $"Patient_{patientId}");

            string toothFolder = Path.Combine(patientFolder, $"Tooth_{toothId}");

            if (!Directory.Exists(toothFolder))
            {
                Directory.CreateDirectory(toothFolder);
            }

            string extension = Path.GetExtension(originalPath);

            string uniqueName = $"{patientId}_{toothId}_{Guid.NewGuid()}{extension}";

            string newPath = Path.Combine(toothFolder, uniqueName);

            File.Copy(originalPath, newPath, true);

            return newPath;
        }

        public ToothRecordService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRecordAsync(
            int toothId,
            string notes,
            ToothStatus toothStatus,
            List<string> realImagePaths,
            List<string> xrayImagePaths)
        {
            var tooth = await _context.Teeth
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == toothId);

            if (tooth == null)
                throw new Exception("Tooth not found");

            tooth.Status = toothStatus;

            int patientId = tooth.PatientId;

            var record = new ToothRecord
            {
                ToothId = toothId,
                Notes = notes,
                VisitDate = DateTime.Now,
                Images = new List<ToothImage>()
            };

            foreach (var path in realImagePaths)
            {
                string savedPath = SaveImage(path, patientId, toothId);

                record.Images.Add(new ToothImage
                {
                    FilePath = savedPath,
                    ImageType = ImageType.Real
                });
            }

            foreach (var path in xrayImagePaths)
            {
                string savedPath = SaveImage(path, patientId, toothId);

                record.Images.Add(new ToothImage
                {
                    FilePath = savedPath,
                    ImageType = ImageType.XRay
                });
            }

            await _context.ToothRecords.AddAsync(record);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ToothRecord>> GetRecordsByToothIdAsync(int toothId)
        {
            return await _context.ToothRecords
                .AsNoTracking()
                .Include(r => r.Images)
                .Where(r => r.ToothId == toothId)
                .OrderByDescending(r => r.VisitDate)
                .ToListAsync();
        }

        public async Task UpdateRecordNotesAsync(int recordId, string notes)
        {
            var record = await _context.ToothRecords
                .FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
                throw new Exception("Record not found");

            record.Notes = notes;

            await _context.SaveChangesAsync();
        }

        public async Task<ToothRecord?> GetRecordByIdAsync(int recordId)
        {
            return await _context.ToothRecords
                .AsNoTracking()
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == recordId);
        }

        public async Task AddImageAsync(int recordId, string originalImagePath, ImageType imageType)
        {
            var record = await _context.ToothRecords
                .Include(r => r.Tooth)
                .FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
                throw new Exception("Record not found");

            int patientId = record.Tooth.PatientId;
            int toothId = record.ToothId;

            string savedPath = SaveImage(originalImagePath, patientId, toothId);

            var image = new ToothImage
            {
                ToothRecordId = recordId,
                FilePath = savedPath,
                ImageType = imageType
            };

            await _context.ToothImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }
    }
}
