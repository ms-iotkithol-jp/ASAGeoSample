using Microsoft.ServiceBus.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MovingPointGeneratorWPF
{
    /// <summary>
    /// TrackingLog.xaml の相互作用ロジック
    /// </summary>
    public partial class TrackingLog : Window
    {
        public TrackingLog()
        {
            InitializeComponent();
        }

        public void AddLog(string json)
        {
            var sb = new StringBuilder(tbLog.Text);
            var writer = new StringWriter(sb);
            writer.WriteLine(json);
            tbLog.Text = sb.ToString();
            if (sendToEventHub)
            {
                var msg = new EventData(Encoding.UTF8.GetBytes(json));
                ehClient.SendAsync(msg);
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog().Value)
            {
                using (var fs = File.OpenWrite(dialog.FileName))
                {
                    var writer = new StreamWriter(fs);
                    var reader = new StringReader(tbLog.Text);
                    writer.Write(reader.ReadToEnd());
                    writer.Flush();
                }
            }
        }

        bool sendToEventHub = false;
        string sendLabelStart = "Send";
        string sendLabelStop = "Stop";
        EventHubClient ehClient = null;
        private void buttonEH_Click(object sender, RoutedEventArgs e)
        {
            if (buttonEH.Content.ToString() == sendLabelStart)
            {
                if (ehClient == null)
                {
                    ehClient = EventHubClient.CreateFromConnectionString(tbEHCS.Text, tbEHName.Text);

                }
                sendToEventHub = true;
                buttonEH.Content = sendLabelStop;
            }
            else if (buttonEH.Content.ToString() == sendLabelStop)
            {
                sendToEventHub = false;
                buttonEH.Content = sendLabelStart;
            }
        }

    }
}
