

namespace Dental.Core.Entities
{
    public class ToothRecord
    {
        public int Id { get; set; }

        public DateTime VisitDate { get; set; } = DateTime.Now;

        public string Notes { get; set; } = string.Empty;

        public int ToothId { get; set; }

        public Tooth Tooth { get; set; } = null!;

        public ICollection<ToothImage> Images { get; set; } = new List<ToothImage>();
    }
}
