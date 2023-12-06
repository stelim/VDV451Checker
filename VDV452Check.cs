using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace VDV451Checker
{
    public class VDV452Check
    {
        private readonly Dictionary<string, string> TableNames = new Dictionary<string, string>();

        public VDV452Check(string f)
        {
            Filepath = f.Trim();
            InitializeTableNames();
            ReadAndCheckFilesFromPath(Filepath);
        }

        private string Filepath { get; set; }
        private List<string> FoundRows = new List<string>();
        private List<string> ColumnNames = new List<string>();
        private List<ColumnFormat> Format = new List<ColumnFormat>();
        
        public bool IsFormatConsistend;

        public List<ColumnFormat> GetColumnFormat() { return Format; }
        public int errors = 0;
        public List<Error> Errors = new List<Error>();

        private void ReadAndCheckFilesFromPath(string filepath)
        {
            List<string> badRecord = new List<string>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = Encoding.ASCII,
                Delimiter = ";",
                HasHeaderRecord = false,
                //Mode = CsvMode.Escape,
                TrimOptions = TrimOptions.Trim,
                BadDataFound = context => badRecord.Add(context.RawRecord)
            };
            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, config))
            {


                while (csv.Read())
                {

                    var rowType = csv.GetField(0);
                    // TODO: add more checks for different row types!
                    /* 
                                        if (rowType == "mod")
                                        {

                                        }

                                        if (rowType == "src")
                                        {

                                        }

                                        if (rowType == "chs")
                                        {

                                        }

                                        if (rowType == "ver")
                                        {

                                        }

                                        if (rowType == "ifv")
                                        {

                                        }

                                        if (rowType == "dve")
                                        {

                                        }

                                        if (rowType == "fft")
                                        {

                                        }
                    */

                    if (rowType == "tbl")
                    {
                        CheckTableNumberIsKnown(csv.GetField(1).Trim());
                    }

                    if (rowType == "atr")
                    {
                        readAttributesFromFile(csv);
                    }

                    if (rowType == "frm")
                    {
                        checkFRM(csv);
                    }

                    if (rowType != null && rowType == "rec")
                    {
                        checkREC(csv);
                    }
                }
            }

            IsFormatConsistend = ColumnFormatIsConsistent();
        }

        private void readAttributesFromFile(CsvReader csv)
        {
            var atrrec = csv.GetRecord<dynamic>();
            if (atrrec != null)
            {
                foreach (var property in (IDictionary<String, object>)atrrec)
                {
                    string name = property.Value.ToString();
                    if (name != "atr")
                    {
                        ColumnNames.Add(name.Trim());
                    }
                }
            }
        }

        private void checkFRM(CsvReader csv)
        {
            var frmRec = csv.GetRecord<dynamic>();
            if (frmRec != null)
            {
                foreach (var (property, charRegExp, numMatch) in from property in (IDictionary<String, Object>)frmRec
                                                                 where property.Value.ToString() != "frm"
                                                                 let numRegExp = new Regex(@"\bnum\[(\d+)\.\d\]\z")// laut VDV451 num immer nur Ganzzahlig als .0!
                                                                 let charRegExp = new Regex(@"\bchar\[(\d+)\]\z")
                                                                 let numMatch = numRegExp.Match(property.Value.ToString())
                                                                 select (property, charRegExp, numMatch))
                {
                    if (numMatch.Success)
                    {
                        string length = numMatch.Groups[1].Value;
                        ColumnFormat cf = new ColumnFormat("num", length);
                        Format.Add(cf);
                    }
                    else
                    {
                        Match charMatch = charRegExp.Match(property.Value.ToString());
                        if ((charMatch != null) && (charMatch.Success))
                        {
                            string length = charMatch.Groups[1].Value;
                            ColumnFormat cf = new ColumnFormat("char", length);
                            Format.Add(cf);
                        }
                        else
                        {
                            throw new Exception("no format found");
                        }
                    }
                }
            }
        }

        private void checkREC(CsvReader csv)
        {
            var record = csv.GetRecord<dynamic>();
            if (record != null)
            {
                int x = 0;

                foreach (var field in (IDictionary<String, Object>)record)
                {
                    if (field.Value.ToString() != "rec")
                    {
                        var foo = field.Value.ToString();

                        var format = Format[x];

                        if (foo.Length > format.Length)
                        {
                            Errors.Add(new Error(format.ColumnName, foo, "Feldlänge größer als zulässig!", format.Length));
                        }

                        x++;
                    }

                }
            }
        }

        private void CheckTableNumberIsKnown(string tableName)
        {
            FoundRows.Add("tbl");
            var tablenumber = Path.GetFileNameWithoutExtension(Filepath).Substring(1, 3);

            if (!TableNames.ContainsKey(tablenumber))
            {
                throw new Exception("Tablenumer " + tablenumber + " ( " + tableName + " ) not found in known tables!");
            }
        }

        private void InitializeTableNames()
        {
            TableNames.Clear();
            TableNames.Add("993", "BASIS_VER_GUELTIGKEIT");
            TableNames.Add("485", "MENGE_BASIS_VERSIONEN");
            TableNames.Add("348", "FIRMENKALENDER");
            TableNames.Add("290", "MENGE_TAGESART");

            TableNames.Add("998", "MENGE_ONR_TYP");
            TableNames.Add("997", "MENGE_ORT_TYP");
            TableNames.Add("229", "REC_HP");
            TableNames.Add("295", "REC_OM");
            TableNames.Add("253", "REC_ORT");

            TableNames.Add("443", "FAHRZEUG");
            TableNames.Add("992", "ZUL_VERKEHRSBETRIEB");
            TableNames.Add("333", "MENGE_BEREICH");
            TableNames.Add("293", "MENGE_FZG_TYP");
            TableNames.Add("996", "REC_ANR");
            TableNames.Add("994", "REC_ZNR");

            TableNames.Add("299", "REC_SEL");
            TableNames.Add("995", "REC_SEL_ZP");
            TableNames.Add("222", "MENGE_FGR");
            TableNames.Add("999", "ORT_HZTF");
            TableNames.Add("282", "SEL_FZT_FELD");
            TableNames.Add("225", "REC_UEB");
            TableNames.Add("247", "UEB_FZT");
            TableNames.Add("332", "MENGE_FAHRTART");
            TableNames.Add("571", "FLAECHEN_ZONE");
            TableNames.Add("539", "FL_ZONE_ORT");
            TableNames.Add("572", "MENGE_FLAECHEN_ZONE_TYP");
            TableNames.Add("540", "SEL_FZT_FELD_ZP");

            TableNames.Add("246", "LID_VERLAUF");
            TableNames.Add("226", "REC_LID");

            TableNames.Add("715", "REC_FRT");
            TableNames.Add("308", "REC_FRT_HZT");
            TableNames.Add("310", "REC_UMLAUF");

            TableNames.Add("232", "REC_UMS");
            TableNames.Add("334", "MENGE_GEBIET");
            TableNames.Add("432", "EINZELANSCHLUSS");
            TableNames.Add("946", "GRUNDROUTE_VERLAUF");
        }

        public bool ColumnFormatIsConsistent()
        {
            if (Format.Count == ColumnNames.Where(s => !string.IsNullOrEmpty(s)).Count()) { return true; } // gleiche Anzahl Spalten und Formate!
            return false;
        }

    }
}
