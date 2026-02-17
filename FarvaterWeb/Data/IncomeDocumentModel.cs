using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public class IncomingDocumentModel
    {
        public string Title { get; set; }
        public int Identifier { get; set; }

        // Отправитель (ReferenceDto)
        public ReferenceModel Sender { get; set; }

        // Тип документа (ClassifierDto)
        public ClassifierModel DocumentType { get; set; }

        public string ShortDescription { get; set; }
        public string Content { get; set; }

        // Статус (Черновик и т.д.)
        public ReferenceModel Status { get; set; }

        public string Description { get; set; }
        public string Handle { get; set; }

        // Технические поля для дат
        public DateTime CreateTime { get; set; }
    }

    public class ReferenceModel
    {
        public string Description { get; set; }
        public string Handle { get; set; }
        public string SysId { get; set; }
    }

    public class ClassifierModel
    {
        public string Handle { get; set; }
        public string Description { get; set; }
        public string SysId { get; set; }
    }
}
