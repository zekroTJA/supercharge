using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI.Caching
{
    public class RegistrationCache
    {

        public readonly Dictionary<string, string> RegistrationCodeCache = new Dictionary<string, string>();

        public RegistrationCache()
        {
        }

    }
}
