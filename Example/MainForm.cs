using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gribov.pro.Async.Example
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            cbCount.SelectedIndex = 0;
            cbEmergency.SelectedIndex = 0;
            cbPeriod.SelectedIndex = 0;


#if DEBUG
            label1.ForeColor = Color.Red;
            label1.Text = "Cancellation of loading is disabled while debugging.\nTo check this opportunity please compile assembly\nto release version";
#endif
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            double period = double.Parse(cbPeriod.SelectedItem.ToString());
            int count = int.Parse(cbCount.SelectedItem.ToString());
            double emergency = double.Parse(cbEmergency.SelectedItem.ToString());

            label1.Text = "Load data...";
            LoadDataAsync(TimeSpan.FromSeconds(period), count, TimeSpan.FromSeconds(emergency)).ContinueWith((results) =>
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string result in results.Result)
                    {
                        sb.AppendLine(result);
                    }

                    Invoke(new Action(() =>
                        {
                            label1.Text = sb.ToString();
                        }));
                });
        }

        private async Task<string[]> LoadDataAsync(TimeSpan period, int count, TimeSpan emergency)
        {
            AsyncDataLoader<string> loader = new AsyncDataLoader<string>();

            loader.DataLoaders = new Func<string>[]
                {
                        new Func<string>
                            (() =>
                            {
                                double sleep = 1.7d;
                                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                                return $"Loaded in {sleep} second";
                            }), 
                            (() =>
                            {
                                double sleep = 1d;
                                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                                return $"Loaded in {sleep} second";
                            }),
                            (() =>
                            {
                                double sleep = 1.25d;
                                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                                return $"Loaded in {sleep} second";
                            }),
                            (() =>
                            {
                                double sleep = 0.01d;
                                Thread.Sleep(TimeSpan.FromSeconds(sleep));
                                return $"Loaded in {sleep} second";
                            }),
                };
            loader.LoadDataPeriod = period;
            loader.MinResultsCount = count;
            loader.EmergencyPeriod = emergency;
            return await loader.GetResultsAsync();
        }
    }
}
