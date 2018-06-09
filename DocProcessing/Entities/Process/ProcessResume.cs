using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.Process
{
    class ProcessResume
    {
        string Description { get; set; }
        string Status { get; set; }
    }

    public enum ProcessStatus{
        Sucess=1,
        InvalidInputForProcess = 2,
        ExceptionProcessingInput = 3,
        UnexpectedException = 0,
    }
}
