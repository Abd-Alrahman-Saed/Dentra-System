

namespace Dental.Core.Entities
{
    public class DentalHistory
    {
        public int Id { get; set; }

        public DateTime? LastDentalVisitDate { get; set; }
        public string? LastDentalVisitReason { get; set; }

        public bool HasGumBleeding { get; set; }        // نزيف في اللثة
        public bool HasToothPain { get; set; }          // ألم في الأسنان
        public bool HasHotColdSensitivity { get; set; } // حساسية من السخن أو البارد
        public bool HasBruxism { get; set; }            // صرير الأسنان (طحن الأسنان)
        public bool HasJawPain { get; set; }            // ألم في الفك
        public bool HadOrthodonticTreatment { get; set; } // عمل تقويم قبل كده
        public bool HasCrownsOrBridges { get; set; }    // عنده تركيبات أو كباري
        public bool HasPreviousFillings { get; set; }   // عنده حشوات سابقة
        public bool HadToothExtraction { get; set; }    // خلع أسنان قبل كده
        public bool HadRootCanalTreatment { get; set; } // عمل علاج عصب قبل كده
        public bool HasGumDisease { get; set; }         // أمراض لثة

        public string? OralHygieneHabits { get; set; }  // عادات تنظيف الفم (تفريش، خيط،...)
        public string? Notes { get; set; }

        public int PatientId { get; set; }

        public Patient Patient { get; set; } = null!;
    }
}
