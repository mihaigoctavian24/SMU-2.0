using System.Runtime.Serialization;

namespace UniversityManagement.Domain.Enums;

public enum StudentStatus
{
    [EnumMember(Value = "active")]
    Active,
    [EnumMember(Value = "suspended")]
    Suspended,
    [EnumMember(Value = "expelled")]
    Expelled,
    [EnumMember(Value = "graduated")]
    Graduated,
    [EnumMember(Value = "withdrawn")]
    Withdrawn
}
