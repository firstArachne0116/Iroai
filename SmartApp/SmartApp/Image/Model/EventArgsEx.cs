using System;
using System.Collections.Generic;

namespace SmartApp.Model
{
    public class EventArgsEx : EventArgs
    {
        public string TitleName { get; set; }

        public byte[] ByteArray { get; set; }

        public List<ImageColorInfo> InputList { get; set; }

        public string Filepath { get; set; }

    }
}
