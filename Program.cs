using System.Globalization;
using System.Text;
using System.IO;
using System.Collections;
using CsvHelper;
using CsvHelper.Configuration;
using Spectre.Console;

namespace VDV451Checker
{
    public static class Program
    {
        public static void Greeting()
        {

            AnsiConsole.Write(
            new FigletText("VDV 451 Check")
            .Centered()
            .Color(Color.Blue3));
        }

        public static void ProcessFiles(string filepath, bool AlleFehlerAnzeigen = true)
        {
            AnsiConsole.WriteLine("Verarbeite i10-Dateien aus: " + filepath);

            if (AlleFehlerAnzeigen == false)
            {
                var showFirstError = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                        .Title("Wie sollen Fehler dargestellt werden?")
                        .PageSize(3)
                        .AddChoices(new[] {
            "Alle Fehler anzeigen", "Nur ersten Fehler anzeigen",
                        }));

                AlleFehlerAnzeigen = showFirstError == "Alle Fehler anzeigen";
            }
            Tree root = StartChecks(filepath, AlleFehlerAnzeigen);

            AnsiConsole.Write(root);

        }

        private static Tree StartChecks(string filepath, bool AlleFehlerAnzeigen)
        {
            TimeTableFiles ttfiles = new TimeTableFiles(filepath);
            var root = new Tree(ttfiles.GetDirName());
            var preCheckResult = new PreChecks(ttfiles.ListFiles());
            if (preCheckResult.warnings.Count > 0)
            {
                foreach ( var warning in preCheckResult.warnings )
                {
                    AnsiConsole.Markup("[yellow bold]WARNUNG: " + warning + " [/]\n");
                }
            }

            foreach (var file in ttfiles.ListFiles(false))
            {
                var filenode = root.AddNode("[yellow bold]" + ttfiles.ReturnShortFilename(file) + "[/]");

                var result = new VDV452Check(file);

                string e = result.Errors.Count == 0 ? " [green bold]OK[/] " : "[red bold]" + result.Errors.Count + " Fehler[/]";
                var tblNumber = ttfiles.ReturnTableFromFilename(file);
                filenode.AddNode("Table: " + tblNumber + " - " + e);

                if (result.IsFormatConsistend)
                {
                    var table = new Table();
                    table.AddColumn("Name");
                    table.AddColumn("Typ");
                    table.AddColumn("Länge");


                    foreach (var f in result.GetColumnFormat())
                    {

                        string x = "";
                        x = f.Datatype == "char" ? "[dodgerblue2]" : "[gold1]";
                        x = x + f.Datatype + "[/]";

                        table.AddRow(f.ColumnName, x, f.ColumnLength);
                    }


                    int i = 0;
                    foreach (var msg in result.Errors)
                    {
                        filenode.AddNode(msg.MessageFormatted + ": ");
                        i++;
                        if (i > 0 && AlleFehlerAnzeigen == false)
                        {
                            break;
                        }
                    }

                }
                else
                {
                    AnsiConsole.Markup("[red italic]Anzahl Formate und Spalten in Tabelle stimment nicht überein: " + file + "[/]");
                } 
            }

            return root;
        }

        static void Main(string[] args)
        {
            Greeting();
            if (args.Length > 0)
            {
                Console.WriteLine();
                if (args.Length > 1)
                {
                    ProcessFiles(args[0], true);
                }
                else
                {
                    ProcessFiles(args[0]);
                }
            }
            else
            {
                AnsiConsole.WriteLine("Parameter fehlt. Pfad zu Verzeichnis mit i10-Dateien angeben!");
            }


        }
    }
}