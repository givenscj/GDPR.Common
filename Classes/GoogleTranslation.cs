using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Translation.Google
{
    public class Translation
    {
        public string translatedText { get; set; }
    }

    public class Data
    {
        public List<Translation> translations { get; set; }
    }

    public class GoogleResponse
    {
        public Data data { get; set; }
    }
}
