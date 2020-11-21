using System;
using System.Collections.Generic;
using System.Text;

namespace SmartApp.Interface
{
    public interface IScreenshotService
    {
        byte[] Capture();

        string Capture(string filename);
    }
}
