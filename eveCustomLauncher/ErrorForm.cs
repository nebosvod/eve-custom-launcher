using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace eveCustomLauncher
{
    public partial class ErrorForm : Form
    {
        public ErrorForm(Exception ex)
        {
            InitializeComponent();
            txtErrorMessage.Text = ex.Message;
            txtLog.Text = Log.Instance.GetText();
            txtLog.Text += "\r\n\r\nStack trace: \r\n" + ex.StackTrace;
        }

        
        private void btnCopyErrorData_Click(object sender, EventArgs e)
        {
            string text = txtErrorMessage.Text + "\r\n" + txtLog.Text;
            Thread thread = new Thread(() => Clipboard.SetText(text));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}
