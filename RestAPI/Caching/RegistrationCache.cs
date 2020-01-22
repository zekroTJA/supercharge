using RiotAPIAccessLayer.TimedDictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Caching
{
    public class RegistrationCache
    {

        public readonly TimedDictionary<string, string> RegistrationCodeCache 
            = new TimedDictionary<string, string>(TimeSpan.FromMinutes(10));

        public RegistrationCache()
        {
        }

    }
}
