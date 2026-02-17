using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public class UserModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; } = "";
        public string? Description { get; set; } // Обычно lastName + firstName
        public string? Login { get; set; }
        public string? Mail { get; set; } = "";
        public string? Phone { get; set; } = "";
        public string? PersonnelNumber { get; set; } = "";
        public bool IsDisabled { get; set; } = false;
        public bool IsLeader { get; set; } = false;
        public bool IsDomainUser { get; set; } = false;
        public bool IsSysadmin { get; set; } = false;
        public bool IsExternal { get; set; } = false;
        public string? Language { get; set; } = "ru";

        // Правила (права доступа)
        public Dictionary<string, UserRuleDto> rules { get; set; } = new();
    }

    public class UserRuleDto
    {
        public bool Value { get; set; } = false;
        public bool ByGroup { get; set; } = false;
        public List<string> groups { get; set; } = new();
    }
}
