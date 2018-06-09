using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities.Reader
{
    class CsvReader : Reader
    {
        private List<string> Keys;
        private char delimiter { get; set; }

        public CsvReader(List<string> k,char d) {
            Keys = k;
            delimiter = d;
        }

        public Dictionary<string, object> readLine(string line)
        {
            Dictionary<string, object> Processed = new Dictionary<string, object>();
            string[] info = line.Split(delimiter);
            for (int i = 0; i < info.Length; i++) {
                Processed.Add(Keys[i], Formater.DeletePaddings(info[i]));
            }
            return Processed;
        }
    }
}
