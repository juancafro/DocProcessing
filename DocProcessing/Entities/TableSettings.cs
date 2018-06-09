using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities
{
    class TableSettings : ContentSettings
    {
        public Dictionary<string,GroupSettings> Groups;
        public string GroupDelimiter { get; set; }
    }
}
