using System.ComponentModel;
using Luna.Common;
using NHibernate.Type;

namespace Luna.Shifts.Domain
{
    [TypeConverter(typeof(EnumTypeConverter))]
    public enum StatisticCategory
    {
        [FieldDisplayName("人力")]
        Staffing,
        [FieldDisplayName("服務水平")]
        ServiceLevel,
        [FieldDisplayName("話務量")]
        CV,
        [FieldDisplayName("AHT")]
        AHT
    }

    [TypeConverter(typeof(EnumTypeConverter))]
    public enum ArrangeSeatMethodology
    {
        [FieldDisplayName("擴散")]
        Centralize = 0,
        [FieldDisplayName("依序")]
        Sequence = 1
    }

    public class ArrangeSeatMethodologyEnumType : EnumStringType
    {
        public ArrangeSeatMethodologyEnumType()
            : base(typeof(ArrangeSeatMethodology), 10) { }
    }

}