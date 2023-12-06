using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDV451Checker
{
    public class ColumnFormat
    {
        public string ColumnName { get; private set; }
        public string Datatype { get; private set; }
        public int Length { get; private set; }
        public string ColumnLength { get; private set; }

        public ColumnFormat(string datatype, string length)
        {
            ColumnName = ""; // wird später gefüllt, da andere Zeile in 452-Datensatz!
            Datatype = datatype;
            ColumnLength = length;

            int l;
            if (int.TryParse(length, out l))
            {
                Length = l;
            }
            else
            {
                Length = 0;
                throw new Exception();
            }

        }



    }
}
