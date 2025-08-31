using System.ComponentModel;

namespace IdentityCardGenerator.Models
{
    public class IdentityCard : INotifyPropertyChanged
    {
        private string _name;
        private string _idNumber;
        private string _photoPath;
        private string _barcodePath;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string IdNumber
        {
            get => _idNumber;
            set
            {
                _idNumber = value;
                OnPropertyChanged(nameof(IdNumber));
            }
        }

        public string PhotoPath
        {
            get => _photoPath;
            set
            {
                _photoPath = value;
                OnPropertyChanged(nameof(PhotoPath));
            }
        }

        public string BarcodePath
        {
            get => _barcodePath;
            set
            {
                _barcodePath = value;
                OnPropertyChanged(nameof(BarcodePath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}