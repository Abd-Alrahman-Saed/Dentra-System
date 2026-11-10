
using Dental.Core.Enums;

namespace Dental.Core.Entities
{
    public class Tooth
    {
        public int Id { get; set; }

        public int ToothNumber { get; set; }

        public string Name { get; set; } = string.Empty;

        public ToothArch Arch { get; set; }

        public ToothStatus Status { get; set; } = ToothStatus.Healthy;

        public int PatientId { get; set; }

        public Patient Patient { get; set; } = null!;

        public ICollection<ToothRecord> Records { get; set; } = new List<ToothRecord>();
    }
}
