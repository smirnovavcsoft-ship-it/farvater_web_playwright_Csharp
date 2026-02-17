using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public class CounterpartyModel
    {
        public string Inn { get; set; }
        public string ShortTitle { get; set; }
        public string FullTitle { get; set; }
        public string Address { get; set; } = "";
        public string Ogrn { get; set; } = "";
        public string Kpp { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Type { get; set; } = "LEGALENTITY_DEF";
    }
}
