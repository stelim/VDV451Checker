using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDV451Checker
{
    public class Error
    {
        public string Value { get; set; }
        public int MaxLength { get; set; }
        public string Message { get; set; }
        public string MessageFormatted { get; set; }
        public string ColumnName { get; set; }
        public int RowNumber { get; set; }


        public Error(string colname, string value, string message, int maxlength)
        {
            MaxLength = maxlength;
            ColumnName = colname;
            Value = value;
            Message = message;
            
            MessageFormatted = "Feld [gold1]" + ColumnName + "[/]: " + message + " | Wert: [gold1]>[/]" + Value.EscapeMarkup()+ "[gold1]<[/] ([red]"+Value.Length+ "[/] / [green bold]" + maxlength+ "[/])";
        }
    }
}
