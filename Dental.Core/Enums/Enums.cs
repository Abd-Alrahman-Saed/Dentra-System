

namespace Dental.Core.Enums
{
    public enum Gender
    {
        Male = 1,
        Female = 2
    }

    public enum ToothStatus
    {
        Healthy = 1,
        WithNotes = 2,
        Treated = 3,
        Problem = 4
    }

    public enum ImageType
    {
        Real = 1,   // صورة واقعية
        XRay = 2    // أشعة
    }

    public enum ToothArch
    {
        UpperLeft = 1,  
        UpperRight = 2,  
        LowerLeft = 3,
        LowerRight = 4   
    }
}