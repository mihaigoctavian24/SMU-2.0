using System.Runtime.Serialization;

namespace UniversityManagement.Domain.Enums;

public enum UserRole
{
    [EnumMember(Value = "student")]
    Student,
    [EnumMember(Value = "professor")]
    Professor,
    [EnumMember(Value = "dean")]
    Dean,
    [EnumMember(Value = "rector")]
    Rector,
    [EnumMember(Value = "secretariat")]
    Secretariat,
    [EnumMember(Value = "admin")]
    Admin
}
