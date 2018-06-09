using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.Process
{
    class GetDataFromDocument : Process
    {

        protected override ProcessingRequest handleRequest(ProcessingRequest request)
        {
            return request;
        }

        public override bool validateInput(ProcessingRequest request)
        {
            return true;
        }
    }
}
