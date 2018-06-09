using DocProcessing.Entities.Data;
using System.Collections.Generic;

namespace DocProcessing.Entities.Process
{
    public class ProcessingRequest
    {
        List<ProcessResume> processResumes = new List<ProcessResume>();
        BaseEntity ObjectToProcess { get; set; }
    }
}
