using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.Reader
{
    interface Reader
    {
        Dictionary<string, Object> readLine (string line);
    }
}
