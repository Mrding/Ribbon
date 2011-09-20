using Luna.Common;

namespace Luna.Shifts.Domain
{
    public interface ISeatingSeat
    {
        //IDictionary<ISimpleEmployee,int> PriorityEmployees { get; }

        Entity PriorityOrganization { get; }

        /// <summary>
        /// 是否對優先人員及組織外開放
        /// </summary>
        bool IsOpen { get; }
     

        int XCord { get; }
        int YCord { get; }
    }
}