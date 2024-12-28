using System;
using System.Collections.Generic;
using System.Text;

namespace OS.OBDII.Models
{
    public class ELM327ResponseText
    {
        public  ELM327ResponseText(string text, string description = null, bool isError = true)
        {
            this.Description = description ?? text;
            this.Text = text;
            this.IsError = isError;

        }
        public string Text { get; set; }
        public string Description { get; set; }
        public bool IsError { get; set; }
    }


}
