using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public class ProjectModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string ProjectsObject { get; set; }

        // Системные списки (уже заполнены по умолчанию)
        public CapitalConstructionType CapitalConstructionType { get; set; } = new();
        public ProjectType ProjectType { get; set; } = new();

        // ГИП (сюда мы передадим handle нового пользователя)
        public Gip Gip { get; set; } = new();

        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string DiskLetter { get; set; } = "N";
        public long MaxSize { get; set; } = 50000000000;
    }

    // Эти два класса просто "держат" константы
    public class CapitalConstructionType { public string SysId { get; set; } = "NODE_CAPITALCONSTRUCTION_LINEAR_COMMONSTRUCT"; }
    public class ProjectType { public string SysId { get; set; } = "NODE_PROJECTTYPE_CONSTRUCTION"; }

    // А этот класс мы будем наполнять динамически
    public class Gip { public string Handle { get; set; } }
}

