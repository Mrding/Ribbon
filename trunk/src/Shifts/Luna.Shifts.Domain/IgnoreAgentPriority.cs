using System.ComponentModel;
using Luna.Common;

namespace Luna.Shifts.Domain
{
    [TypeConverter(typeof(EnumTypeConverter))]
    public enum IgnoreAgentPriority
    {
        [FieldDisplayName("遵循席位之人员顺位")]
        UseAllAgentPriority = 0,
        [FieldDisplayName("忽略席位之人員順位3")]
        IgnoreAgentPriority3 = 1,
        [FieldDisplayName("忽略席位之人員順位2，3")]
        IgnoreAgentPriority2And3 = 2,
        [FieldDisplayName("忽略席位之全部人员顺位")]
        IgnoreAllAgentPriority = 3
    }
}