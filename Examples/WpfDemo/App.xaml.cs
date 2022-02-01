using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TallyConnector;
using TallyConnector.Models;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Tally tally = new();

        public App()
        {
            GetVoucher();
        }

        async void GetVoucher()
        {
            if (await tally.Check())
            {
                Voucher v1 = await tally.GetVoucherByMasterID("1296");
                //Voucher v = await tally.GetVoucherByVoucherNumber("20-21/Example","20210331");
            }


        }
    }
}
