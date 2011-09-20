using System;
using Luna.Common;

namespace Luna.Infrastructure.Domain.Model
{
    using System.Collections.Generic;

    [IgnoreRegister]
    public interface IBackendModel
    {
        DateTime GetUniversialTime();

        IDictionary<string, IList<TimeZoneInfo>> GetCountriesTimeZones();

        //void GetLicense();
    }
}