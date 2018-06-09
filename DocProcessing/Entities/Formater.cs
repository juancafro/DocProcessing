using System;
using System.Collections.Generic;
using System.Text;

namespace DocProcessing.Entities
{
    class Formater
    {

        public static double FormatRealValue(string initial, int decimalDigits) {
            char[] newValue = initial.ToCharArray();
            string numberWithDelimiter = "";
            for (int i = 0; i < newValue.Length; i++) {
                if (i == (newValue.Length - decimalDigits)) {
                    numberWithDelimiter += ",";
                }
                numberWithDelimiter += newValue[i];
            }
            double value = Convert.ToDouble(numberWithDelimiter);
            return value;
        }

        public static string DeletePaddings(string initial) {
            try
            {
                if (initial != String.Empty)
                {
                    string noPadding = initial.TrimEnd(new char[] { ' ' });
                    noPadding = noPadding.TrimStart(new char[] { '0' });
                    return noPadding;
                }
                else {
                    return initial;
                }
            }
            catch (Exception ex) {
                throw;
            }
        }
    }
}
