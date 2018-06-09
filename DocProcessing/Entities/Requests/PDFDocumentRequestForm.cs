using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.Requests
{
    class PDFDocumentRequestForm
    {
        public string TemplateUrl { get; set; }
        public string TemplateHtml { get; set; } = @"";
        public string TemplateType { get; set; }
        public List<object> SourceData { get; set; }
        public string IdentifierField { get; set; }
        public string PasswordField { get; set; }
        public string SourceExt { get; set; } = "HTML";
        public string TargetExt { get; set; } = "PDF";
        public string TTL { get; set; }
        public string TagList { get; set; }
    }
}
