using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingPointGeneratorWPF.Models
{
    public class GuitorPosition : INotifyPropertyChanged
    {
        private double posX;
        private double posY;
        private double latitude;
        private double longitude;

        private double deltaLatitude = -3.06192e-05;
        private double deltaLongitude = 3.88484e-05;

        private readonly double ltLatitude = 35.66772396;
        private readonly double ltLongitude = 139.7288072;

        public GuitorPosition(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }
        public double PosX
        {
            get
            {
                return posX;
            }
            set
            {
                posX = value;
                Longitude = ltLongitude + posX * deltaLongitude;
                OnPropertyChanged("PosX");
            }
        }

        public double PosY
        {
            get
            {
                return posY;
            }
            set
            {
                posY = value;
                Latitude = ltLatitude + posY * deltaLatitude;
                OnPropertyChanged("PosY");
            }
        }

        public double Latitude
        {
            get { return latitude; }
            set
            {
                latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        public double Longitude
        {
            get { return longitude; }
            set
            {
                longitude = value;
                OnPropertyChanged("Longitude");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
