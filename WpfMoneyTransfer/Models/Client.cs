using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfMoneyTransfer.Models
{
    /// <summary>
    /// Client
    /// </summary>
    public class Client : INotifyPropertyChanged
    {
        private string _surname;
        private string _name;
        private string _middleName;
        private DateTime _dateOfBirth;
        private string _numberOfPhone;
        
        public Client(int id, string surname, string name, string middleName, DateTime dateOfBirth, string numberOfPhone)
        {
            ID = id;
            this._surname = surname;
            this._name = name;
            this._middleName = middleName;
            this._dateOfBirth = dateOfBirth;
            this._numberOfPhone = numberOfPhone;
        }
        public int ID { get; set; }

        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                OnPropertyChanged("Surname");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                _middleName = value;
                OnPropertyChanged("MiddleName");
            }
        }
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged("DateOfBirth");
            }
        }
        public string NumberOfPhone
        {
            get { return _numberOfPhone; }
            set
            {
                _numberOfPhone = value;
                OnPropertyChanged("NumberOfPhone");
            }
        }       
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
