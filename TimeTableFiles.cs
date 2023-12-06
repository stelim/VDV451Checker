using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VDV451Checker
{
    class TimeTableFiles
    {
        private List<string> files = new List<string>();
        public string filePath {get; private set;}
    


    public TimeTableFiles(string fp) {
            filePath = fp;

            if (!Directory.Exists(filePath))
            {
                throw new FileNotFoundException(fp);
            }

            foreach (var filename in Directory.GetFiles(filePath,"i*"))
            {
                if (File.Exists(filename) && filename.EndsWith("x10"))
                {   
                    files.Add(filename);
                }
            }

        }

        public string GetDirName()
        {
            string? name = Path.GetFileName(filePath);
            if (name == null)
            {
                throw new DirectoryNotFoundException(filePath);
            }
            
            return name;
        }


        public List<string> ListFiles(bool filenameonly = true)
        {
            if (!filenameonly)
            {
                return files;
            }
            else
            {
                var filenames = new List<string>();
                foreach (var filename in files)
                {
                    filenames.Add(Path.GetFileNameWithoutExtension(filename));
                }

                return filenames;
            }
            
            
        }

        public string ReturnTableFromFilename(string filename)
        {
            string foo = Path.GetFileNameWithoutExtension(filename).Substring(1,3);
            return foo;
        }

        public string ReturnShortFilename(string filename)
        {
            return Path.GetFileName(filename);
        }

    }
}
