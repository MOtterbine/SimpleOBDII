using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS.OBDII.Views
{
    public class PopupInfo
    {
        public PopupInfo(string title, string msg, bool isYesNo = false, string okText = null, string cancelText = null)
        {
            this.IsYesNo = isYesNo;
            if (String.IsNullOrEmpty(title)) throw (new ArgumentException());
            if (String.IsNullOrEmpty(msg)) throw (new ArgumentException());
            this.Title = title;
            this.Message = msg;
            OkText = okText;
            CancelText = cancelText;

        }
        public string Title;
        public string Message;
        public bool IsYesNo { get; set; }

        public string OkText { get; set; } = null;
        public string CancelText { get; set; } = null;

    }
}
