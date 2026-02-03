using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public class CounterpartyModel
    {
        public string inn { get; set; }
        public string shorttitle { get; set; }
        public string title { get; set; }
        public string address { get; set; } = "";
        public string ogrn { get; set; } = "";
        public string kpp { get; set; } = "";
        public string phone { get; set; } = "";
        public string email { get; set; } = "";
        public string type { get; set; } = "LEGALENTITY_DEF";
    }
}
