using DocumentFormat.OpenXml.Bibliography;
using System.ComponentModel;

namespace IdentityCardGenerator.Models
{
    public class IdentityCard : INotifyPropertyChanged
    {
       
        private string? _idNumber;
        private string? _photoPath;
        private string? _barcodePath;
        private string? _department;
        private string? _phone;
        private string? _firstName;
        private string? _lastName;
        private string? _position;

        public string FirstName
        {
            get => _firstName!;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }
        public string LastName
        {
            get => _lastName!;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        public string IdNumber
        {
            get => _idNumber!;
            set
            {
                _idNumber = value;
                OnPropertyChanged(nameof(IdNumber));
            }
        }

        public string PhotoPath
        {
            get => _photoPath!;
            set
            {
                _photoPath = value;
                OnPropertyChanged(nameof(PhotoPath));
            }
        }

        public string BarcodePath
        {
            get => _barcodePath!;
            set
            {
                _barcodePath = value;
                OnPropertyChanged(nameof(BarcodePath));
            }
        }
        public required string Department
        {
            get => _department!;
            set
            {
                _department = value;
                OnPropertyChanged(nameof(Department));
            }
        }
        public required string Position
        {
            get => _position!;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }
        public required string Phone
        {
            get => _phone!;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }
   
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}