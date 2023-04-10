using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public class FontFamily
    {
        private const int LANG_NEUTRAL = 0;
        private bool createDefaultOnFail;

        public static FontFamily GenericSansSerif => new FontFamily();
    }
}
