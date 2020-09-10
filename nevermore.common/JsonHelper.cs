using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nevermore.common
{
    public class JsonHelper
    {
        public static T ConvertFromJsonToObject<T>(string aJsonString)
        {
            return JsonConvert.DeserializeObject<T>(aJsonString);
        }
    }
}
