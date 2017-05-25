using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MovingPointGeneratorWPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Loaded += MainWindow_Loaded;
        }

        private string guitorId = "Trip Guitor";
        Models.GuitorPosition guitorPosition;
        DispatcherTimer movingTimer;
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            guitorPosition = new Models.GuitorPosition(guitorId);
            guitorPosition.PosX = 760;
            guitorPosition.PosY = 50;
            mainGrid.DataContext = guitorPosition;

            movingTimer = new DispatcherTimer();
            movingTimer.Interval = TimeSpan.FromSeconds(1);
            movingTimer.Tick += MovingTimer_Tick;
        }

        private void MovingTimer_Tick(object sender, EventArgs e)
        {
            guitorPosition.PosX += deltaX;
            guitorPosition.PosY += deltaY;
            double dx = goalX - guitorPosition.PosX;
            double dy = goalY - guitorPosition.PosY;
            if (dx * dx + dy * dy < 2 * (deltaX * deltaX + deltaY * deltaY))
            {
                movingTimer.Stop();
                buttonCtrl.Content = movingStartLabel;
            }
        }

        string movingStartLabel = "Move Start";
        string movingStopLabel = "Move Stop";

        private void buttonCtrl_Click(object sender, RoutedEventArgs e)
        {
            if (buttonCtrl.Content.ToString() == movingStartLabel)
            {
                movingTimer.Start();
                buttonCtrl.Content = movingStopLabel;
            }
            else if (buttonCtrl.Content.ToString() == movingStopLabel)
            {
                movingTimer.Stop();
                buttonCtrl.Content = movingStartLabel;
            }
        }

        private void mapImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(mapCanvas);
            goalX = point.X;
            goalY = point.Y;
            deltaX = (goalX - guitorPosition.PosX) / cycle;
            deltaY = (goalY - guitorPosition.PosY) / cycle;
            buttonCtrl.IsEnabled = true;

            Canvas.SetLeft(imgGoal, point.X - imgGoal.ActualWidth / 2);
            Canvas.SetTop(imgGoal, point.Y - imgGoal.ActualHeight / 2);
            if (imgGoal.Visibility == Visibility.Hidden)
            {
                imgGoal.Visibility = Visibility.Visible;
            }
        }
        private double goalX;
        private double goalY;
        private double deltaX;
        private double deltaY;
        private double cycle = 100;

        private Point lastMousePoint;
        private bool onMouse = false;
        private void imgGuitor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            onMouse = true;
            lastMousePoint = e.GetPosition(imgMap);
        }

        private void imgGuitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (onMouse)
            {
                var current = e.GetPosition(imgMap);
                double dx = current.X - lastMousePoint.X;
                double dy = current.Y - lastMousePoint.Y;
                guitorPosition.PosX += dx;
                guitorPosition.PosY += dy;
                lastMousePoint = current;
            }
        }


        private void imgGuitor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            onMouse = false;
        }

        private void imgGuitor_MouseLeave(object sender, MouseEventArgs e)
        {
            onMouse = false;
        }

        private string trackingStartLabel = "Tracking Start";
        private string trackingStopLabel = "Tracking Stop";
        DispatcherTimer trackingTimer;
        TrackingLog tlWindow = null;

        private void buttonTracking_Click(object sender, RoutedEventArgs e)
        {
            if (buttonTracking.Content.ToString() == trackingStartLabel)
            {
                trackingTimer = new DispatcherTimer();
                trackingTimer.Interval = TimeSpan.FromSeconds(1);
                trackingTimer.Tick += TrackingTimer_Tick;
                trackingTimer.Start();
                buttonTracking.Content = trackingStopLabel;
                if (tlWindow != null)
                {
                    tlWindow.Close();
                }
                tlWindow = new TrackingLog();
                tlWindow.Show();
            }
            else if (buttonTracking.Content.ToString() == trackingStopLabel)
            {
                trackingTimer.Stop();
                buttonTracking.Content = trackingStartLabel;
            }
        }

        private void TrackingTimer_Tick(object sender, EventArgs e)
        {
            var tracking = new
            {
                id = guitorPosition.Id,
#if true
                latitude = guitorPosition.Latitude,
                longitude = guitorPosition.Longitude,
#else
                position = new
                {
                    type="Point",
                    coordinates=new double[] { guitorPosition.Longitude, guitorPosition.Latitude }
                },
#endif
                trackingTime = DateTime.Now
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(tracking);
            Debug.WriteLine(json);
            tlWindow.AddLog(json);
        }
    }
}
