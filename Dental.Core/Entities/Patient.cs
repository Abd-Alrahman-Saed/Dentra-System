
using Dental.Core.Enums;

namespace Dental.Core.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public Gender Gender { get; set; } = Gender.Male;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public MedicalHistory? MedicalHistory { get; set; }

        public DentalHistory? DentalHistory { get; set; }

        public ICollection<Tooth> Teeth { get; set; } = new List<Tooth>();
    }
}
