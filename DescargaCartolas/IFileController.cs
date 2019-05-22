using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DescargaCartolas
{
    public interface IFileController
    {
        void WriteBinaryFile(byte[] data);
        void WriteTextFile(String text);
    }
}
