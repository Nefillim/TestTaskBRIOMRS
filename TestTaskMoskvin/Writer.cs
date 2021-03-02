using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TestTaskMoskvin
{
    class Writer : TextWriter
    {
        public override Encoding Encoding { get { return Encoding.ASCII; } }

        private TextBox textbox;
        public Writer()
        {
        }

        public override void Write(char value)
        {
            //textbox.Text += value;
        }

        public override void Write(string value)
        {
            //textbox.Text += value;
        }
    }
}
