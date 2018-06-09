using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocProcessing.Entities.Reader
{
    class DisposableCsvReader : IDisposable
    {

        private List<string> columns;
        private char delimiter;
        private string FilePath;
        private StreamReader reader;
        public int columnsNameRow;
        private int currentRow = -1;
        public string currentLine = null;
        public bool EndReached = false;

        public DisposableCsvReader(string f, char d, int columnsRow)
        {
            FilePath = f;
            delimiter = d;
            reader = new StreamReader(f);
            columnsNameRow = columnsRow;
            getMap(columnsRow);

        }

        private void getMap(int columnsRow)
        {
            List<string> KeyList = new List<string>();
            while (currentRow < columnsRow)
            {
                currentLine = reader.ReadLine();
                currentRow++;

                if (currentLine == null) {
                    throw new IndexOutOfRangeException("InvalidColumnsNamesIndex : Reached end of file while parse");
                }
                if (currentRow == columnsRow)
                {
                    string[] keys = currentLine.Split(delimiter);
                    for (int i = 0; i < keys.Length; i++)
                    {
                        KeyList.Add(keys[i]);
                    }
                    break;
                }
            }
            columns = KeyList;
        }


        public Dictionary<string, object> readLine()
        {
            if ((currentLine = reader.ReadLine()) != null)
            {
                Dictionary<string, object> Processed = new Dictionary<string, object>();
                string[] info = currentLine.Split(delimiter);
                for (int i = 0; i < info.Length; i++)
                {
                    Processed.Add(columns[i], Formater.DeletePaddings(info[i]));
                }
                currentRow++;
                return Processed;
            }
            else
            {
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
