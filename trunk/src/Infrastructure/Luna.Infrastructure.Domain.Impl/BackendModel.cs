using System;
using Luna.Data;
using Luna.Infrastructure.Domain.Model;

namespace Luna.Infrastructure.Domain.Impl
{
    using System.Collections.Generic;

    public class BackendModel : IBackendModel
    {
        private static TimeSpan _differ;
        public DateTime GetUniversialTime()
        {
            try
            {
                if (_differ != default(TimeSpan))
                    return DateTime.Now.Subtract(_differ);
                var result = (DateTime)AdoUtil.ExecuteScalar(string.Format("select getDate()"));
                _differ = DateTime.Now - result;
                return result;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public IDictionary<string,IList<TimeZoneInfo>> GetCountriesTimeZones()
        {
            var timeZoneInfos = new Dictionary<string, IList<TimeZoneInfo>>();
            string nameSuffix = " Standard Time";
            var china = TimeZoneInfo.FindSystemTimeZoneById("China" + nameSuffix);
            var hawaii = TimeZoneInfo.FindSystemTimeZoneById("Hawaiian" + nameSuffix);
            var alaskan = TimeZoneInfo.FindSystemTimeZoneById("Alaskan" + nameSuffix);
            var pacific = TimeZoneInfo.FindSystemTimeZoneById("Pacific" + nameSuffix);
            var mountain = TimeZoneInfo.FindSystemTimeZoneById("Mountain" + nameSuffix);
            var central = TimeZoneInfo.FindSystemTimeZoneById("Central America" + nameSuffix);
            var eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern" + nameSuffix);
            timeZoneInfos["China"] = new List<TimeZoneInfo>() { china };
            timeZoneInfos["US"] = new List<TimeZoneInfo>() { china, hawaii, alaskan, pacific, mountain, central, eastern };
            timeZoneInfos["TaiWan"] = new List<TimeZoneInfo>() { china };
            return timeZoneInfos;
        }

        //public void GetLicense()
        //{
        //    var reader = AdoUtil.ExecuteReader("select License from LicenseKey");
        //    while (reader.Read())
        //    {
        //        LicenseKey.AddLicense(LicenseKey.Decrypt(reader.GetString(0)));
        //    }
        //    reader.Close();
        //}
    }
}