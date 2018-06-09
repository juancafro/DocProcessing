using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities
{
    class GroupSettings
    {
        public string Header { get; set; }
        public string LineStyle { get; set; }
        public string LastLineStyle { get; set; }
        public string SimpleHeader { get; set; }
        public string EndOfPageStyle { get; set; } = @"</table>" + Environment.NewLine + "</div>";
        
        

    }
}
