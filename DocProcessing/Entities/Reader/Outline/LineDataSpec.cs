using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.OutLine
{
    class LineDataSpec
    {
        public string Name { get; set; }
        public string Length { get; set; }
        public string StartsAt { get; set; }
        public string EndsAt { get; set; }
        public string Datatype { get; set; }
    }
}
