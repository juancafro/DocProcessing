using DocProcessing.Entities.OutLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DocProcessing.Entities.Reader
{
    class DisposableLineReader : IDisposable 
    {
        private StringOutLine outLine;
        private StreamReader reader;
        public int columnsNameRow;
        private int currentRow = -1;
        public string currentLine = null;
        public bool EndReached = false;


        public DisposableLineReader(StringOutLine o,string File)
        {
            reader = new StreamReader(File);
            outLine = o;
        }

        public Dictionary<String, Object> readLine()
        {
            if ((currentLine = reader.ReadLine()) != null)
            {
                Dictionary<string, object> stringFields = new Dictionary<string, object>();
                int duplicatedNames = 0;
                foreach (LineDataSpec field in outLine.lineFields)
                {
                    if (!(field.Name.Equals(outLine.lineFields.Last().Name)))
                    {
                        string value;
                        value = currentLine.Substring(Convert.ToInt32(field.StartsAt) - 1, Convert.ToInt32(field.Length));
                        try
                        {
                            stringFields.Add(field.Name, Formater.DeletePaddings(value));
                        }
                        catch (ArgumentException)
                        {
                            duplicatedNames++;
                            stringFields.Add(field.Name + duplicatedNames, Formater.DeletePaddings(value));
                        }
                    }
                }
                return stringFields;
            }
            else {
                EndReached = true;
                return null;
            }
                
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(reader);
            GC.SuppressFinalize(this);
        }
    }
}
