using System;
using System.Collections.Generic;
using System.Text;

namespace App1
{
    public interface IImageService
    {
        void ResizeImage(string sourceFile, string targetFile, float maxWidth, float maxHeight);
        
    }
}
