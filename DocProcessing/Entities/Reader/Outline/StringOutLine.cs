using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace DocProcessing.Entities.OutLine
{
    class StringOutLine
    {
        public List<LineDataSpec> lineFields;

        public StringOutLine (string path,int  startRow,int startColumn,int sheet) 
        {
            lineFields = new List<LineDataSpec>();
            GetLineFieldsFromXlsx(path, startRow, startColumn, sheet); 
        }


        public void GetLineFieldsFromXlsx(string Path, int startRow, int startcolumn, int Sheet) {
            
            FileInfo file = new FileInfo(Path);
            ExcelPackage book = new ExcelPackage(file);
            ExcelWorksheet worksheet = book.Workbook.Worksheets[Sheet];
            int datastartRow = startRow;
            int rowCount = worksheet.Dimension.Rows;
            int ColCount = worksheet.Dimension.Columns;
            for (int i = datastartRow; i <= rowCount; i++) {
                LineDataSpec spec = new LineDataSpec()
                {
                    Name = worksheet.Cells[i, startcolumn].GetValue<String>().Replace(" ", ""),
                    Length = worksheet.Cells[i, startcolumn+1].GetValue<String>(),
                    StartsAt = worksheet.Cells[i, startcolumn+2].GetValue<String>(),
                    EndsAt = worksheet.Cells[i, startcolumn+3].GetValue<String>(),
                    Datatype = worksheet.Cells[i, startcolumn + 4].GetValue<String>().Replace(" ", "")
            };
                lineFields.Add(spec);
            }

        }


    }
}
