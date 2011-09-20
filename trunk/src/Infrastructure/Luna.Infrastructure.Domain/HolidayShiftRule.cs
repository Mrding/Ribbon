namespace Luna.Infrastructure.Domain
{
    public enum HolidayShiftRule
    {
        //0表示否，1表示是，2表示皆可
        FreeToAssign = 2,
        NeverAssignDayOff = 1,
        NeverAssignShift = 0
    }
}