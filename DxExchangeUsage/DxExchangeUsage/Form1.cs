using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DxExchangeUsage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DxChangeAPI api = new DxChangeAPI();
            api.APIKey = "your api key";
            api.SecretKey = "your secret key";

            if (api.Login())
                MessageBox.Show(api.Token);

            if (api.GetTicker("BTC/USD"))
                MessageBox.Show(api.TickerData.symbol + " : " + api.TickerData.initialRate.ToString());

            if (api.GetTicker("ETH/BTC"))
                MessageBox.Show(api.TickerData.symbol + " : " + api.TickerData.initialRate.ToString());

            if (api.GetTicker("XLM/BTC"))
                MessageBox.Show(api.TickerData.symbol + " : " + api.TickerData.initialRate.ToString());

            if (api.Logout())
                MessageBox.Show("Log Out");
        }
    }
}
