using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDV451Checker
{
    public class PreChecks
    {
        public List<string> warnings = new List<string>();
        private List<string> Filenames = new List<string>();
        public PreChecks(List<string> filenames) {
            Filenames = filenames;
            CheckForDuplicates();
        }

        private void CheckForDuplicates()
        {
            var duplicates = Filenames.Where(s => s.Length > 3).GroupBy(s => s.Substring(1, 3)).Where(g => g.Count() > 1).ToDictionary(g => g.Key, g => g.Count());
            if (duplicates.Any())
            {
                warnings.Add("Doppelte Tabellen im Verzeichnis gefunden - Verzeichnis prüfen!");
            }
            //i9463330.x10

            var days = Filenames.Where(s => s.Length > 3).GroupBy(s => s.Substring(4, 3)).Where(g => g.Count() > 1);
            if (days.Any())
            {
                string warning = string.Join(", ", days.Select(s => s.Key));
               

                warnings.Add("Gleiche Tabellen für unterschiedliche Tage gefunden: " + warning);
                int i = 0;
            }
            
        }

    }
}
