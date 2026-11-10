using Dental.Core.Enums;
using System;


namespace Dental.Core.Entities
{
    public class ToothImage
    {
        public int Id { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public ImageType ImageType { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public int ToothRecordId { get; set; }

        public ToothRecord ToothRecord { get; set; } = null!;
    }
}
