using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfMoneyTransfer.Models
{
    public class Card : INotifyPropertyChanged
    {
        private long _cardNumber;
        private string _expirationDate;
        private decimal _balance;
        private bool _bindingPhone;
        private int _clientId;
        public bool _inSQL; // специальная переменная, обозначающая наличие записи в SQL Server (true - есть запись в базе, false - новая запись)
        public Card(int id, long cardNumber, string expirationDate, decimal balance, bool bindingPhone, int clientId, bool inSQL)
        {
            ID = id;
            _cardNumber = cardNumber;
            _expirationDate = expirationDate;
            _balance = balance;
            _bindingPhone = bindingPhone;
            _clientId = clientId;
            _inSQL = inSQL;
        }
        public int ID { get; set; }
        public long CardNumber
        {
            get { return _cardNumber; }
            set
            {
                _cardNumber = value;
                OnPropertyChanged("CardNumber");
            }
        }
        public string ExpirationDate
        {
            get { return _expirationDate; }
            set
            {
                _expirationDate = value;
                OnPropertyChanged("ExpirationDate");
            }
        }
        public decimal Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                OnPropertyChanged("Balance");
            }
        }
        public bool BindingPhone
        {
            get { return _bindingPhone; }
            set
            {
                _bindingPhone = value;
                OnPropertyChanged("BindingPhone");
            }
        }
        public int ClientId
        {
            get { return _clientId; }
            set
            {
                _clientId = value;
                OnPropertyChanged("ClientId");
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
