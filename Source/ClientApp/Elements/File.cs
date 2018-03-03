using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Elements
{
    public class FileBase
    {
        public string Name;
        public string Path;
        public Guid FileID;

        public FileBase(string name, string path, Guid id)
        {
            Name = name;
            Path = path;
            FileID = id;
        }
    }
}
