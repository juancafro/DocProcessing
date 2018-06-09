using DocProcessing.Entities.OutLine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocProcessing.Entities.Reader
{
    class LineReader : Reader
    {
        private StringOutLine outLine;
        public LineReader(StringOutLine o)
        {
            outLine = o;

        }

        public Dictionary<String, Object> readLine(string Line) {
            try
            {

                Dictionary<string, object> stringFields = new Dictionary<string, object>();
                int duplicatedNames = 0;
                foreach (LineDataSpec field in outLine.lineFields)
                {
                    if (!(field.Name.Equals(outLine.lineFields.Last().Name)))
                    {
                        string value;
                        value = Line.Substring(Convert.ToInt32(field.StartsAt) - 1, Convert.ToInt32(field.Length));
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
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
