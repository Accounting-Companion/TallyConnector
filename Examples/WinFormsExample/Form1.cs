using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF = System.Windows.Forms;
using TallyConnector;
using TallyConnector.Models;

namespace WinFormsExample
{
    public partial class Form1 : WF.Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public Tally Ctally = new Tally();

        private async void button1_Click(object sender, EventArgs e)
        {
            string VoucherNumber = "20-21/Example";
            string Date = "31-Mar-2021";
            string VoucherMasterID = "1296";
            if (await Ctally.Check())
            {
                Voucher voucher = await Ctally.GetVoucherByVoucherNumber(VoucherNumber, Date);
                //If you have master id of voucher
                Voucher voucher2 = await Ctally.GetVoucher<Voucher>(VoucherMasterID, VoucherLookupField.MasterId);
            }

        }
    }
}
