using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using iTextSharp.text.xml.xmp;

namespace ClientApp.BaseClasses
{
    public class MetaData
    {
        private Hashtable info = new Hashtable();

        public Hashtable Info
        {
            get => info;
            set => info = value;
        }

        public string Author
        {
            get => (string)info["Author"];
            set => info.Add("Author", value);
        }
        public string Title
        {
            get => (string)info["Title"];
            set => info.Add("Title", value);
        }
        public string Subject
        {
            get => (string)info["Subject"];
            set => info.Add("Subject", value);
        }
        public string Keywords
        {
            get => (string)info["Keywords"];
            set => info.Add("Keywords", value);
        }
        public string Producer
        {
            get => (string)info["Producer"];
            set => info.Add("Producer", value);
        }

        public string Creator
        {
            get => (string)info["Creator"];
            set => info.Add("Creator", value);
        }

        public Hashtable getMetaData()
        {
            return this.info;
        }

        public byte[] getStreamedMetaData()
        {
            MemoryStream os = new MemoryStream();
            XmpWriter xmp = new XmpWriter(os, this.info);
            xmp.Close();
            return os.ToArray();
        }

    }
}
