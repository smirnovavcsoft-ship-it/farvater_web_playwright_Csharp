using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public class UserModel
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleName { get; set; } = "";
        public string? description { get; set; } // Обычно lastName + firstName
        public string? login { get; set; }
        public string? mail { get; set; } = "";
        public string? phone { get; set; } = "";
        public string? personnelNumber { get; set; } = "";
        public bool isDisabled { get; set; } = false;
        public bool isLeader { get; set; } = false;
        public bool isDomainUser { get; set; } = false;
        public bool isSysadmin { get; set; } = false;
        public bool isExternal { get; set; } = false;
        public string? language { get; set; } = "ru";

        // Правила (права доступа)
        public Dictionary<string, UserRuleDto> rules { get; set; } = new();
    }

    public class UserRuleDto
    {
        public bool value { get; set; } = false;
        public bool byGroup { get; set; } = false;
        public List<string> groups { get; set; } = new();
    }
}
