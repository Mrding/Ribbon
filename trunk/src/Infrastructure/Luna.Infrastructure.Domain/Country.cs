using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Luna.Infrastructure.Domain
{
    public class Country
    {

        private readonly string _name;
        private readonly string[] _timeZoneIds;

        public static string Local { get; set; }

        public Country(CultureInfo cultureInfo, string[] timeZoneIds)
        {
            _name = GetName(cultureInfo, cultureInfo.EnglishName);
            DisplayName = GetName(cultureInfo, cultureInfo.DisplayName);

            _timeZoneIds = timeZoneIds;
            TimeZones = (from string timezoneId in _timeZoneIds select TimeZoneInfo.FindSystemTimeZoneById(timezoneId)).ToArray();
        }

        private static string GetName(CultureInfo cultureInfo, string name)
        {
            if (!cultureInfo.EnglishName.Contains("(") || !cultureInfo.DisplayName.Contains("("))
                return string.Empty;
            var name1 = name.Split('(')[1];
            return name1.Substring(0, name1.Length - 1);
        }


        public override string ToString()
        {
            return _name;
        }

        public string DisplayName { get; set; }

        public string[] TimeZoneIds { get { return _timeZoneIds; } }

        public TimeZoneInfo[] TimeZones { get; private set; }
    }
}
