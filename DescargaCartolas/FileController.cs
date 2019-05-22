using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescargaCartolas
{
    public class FileController : IFileController
    {
        public FileController(string path)
        {
            this.path = path;
        }

        public string path { get; }

        public void WriteBinaryFile(byte[] data)
        {
            Task.Run(() =>
            {
                using (FileStream stream = new FileStream(this.path, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(data);
                        writer.Close();
                    }
                }
            });
            
        }

        public void WriteTextFile(string text)
        {
          
                FileMode mode = FileMode.Append;
                if (!File.Exists(path))
                    mode = FileMode.Create;

                using (FileStream stream = new FileStream(path, mode))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(text);
                        writer.Close();
                    }
                }
       
   
        }
    }
}
