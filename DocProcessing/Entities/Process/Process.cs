using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.Process
{
    public abstract class Process
    {
        Process Successor { get; set; }

        public abstract bool validateInput(ProcessingRequest request);
        protected abstract ProcessingRequest handleRequest(ProcessingRequest request);
    }
}
