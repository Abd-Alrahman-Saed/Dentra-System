
namespace Dental.Core.Entities
{
    public class MedicalHistory
    {
        public int Id { get; set; }

        public bool HasHypertension { get; set; }     // ضغط
        public bool HasDiabetes { get; set; }         // سكر
        public bool HasThyroidDisorder { get; set; }  // غدة
        public bool HasAllergy { get; set; }          // حساسية
        public bool HasHeartDisease { get; set; }     // قلب
        public bool HasBleedingDisorder { get; set; } // سيولة
        public bool HasKidneyDisease { get; set; }    // كلية
        public bool HasLiverDisease { get; set; }     // كبد
        public bool HasCancer { get; set; }           // سرطان

        public string? Notes { get; set; }

        public int PatientId { get; set; }

        public Patient Patient { get; set; } = null!;
    }
}
