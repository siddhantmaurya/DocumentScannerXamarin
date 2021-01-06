using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentScanner
{
    public interface ISaveViewFile
    {
        string SaveAndViewAsync(string filename, MemoryStream stream);
    }
}
