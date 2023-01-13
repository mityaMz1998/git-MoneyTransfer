using System.Windows;
using System.Windows.Input;
using WpfMoneyTransfer.ViewModelы;

namespace WpfMoneyTransfer.Views
{
    /// <summary>
    /// Interaction logic for the form Payments.xaml
    /// </summary>
    public partial class Payments : Window
    {
        public TransContragentViewModel send;
        public TransContragentViewModel recieve;
        public Payments()
        {
            InitializeComponent();
            PaymentsViewModel.payments = this;
            DataContext = new PaymentsViewModel();

            send = new TransContragentViewModel(checkFrom, mskTxtFromPhoneNumber, txtFromCard, btnSearchFrom);
            recieve = new TransContragentViewModel(checkTo, mskTxtToPhoneNumber, txtToCard, btnSearchTo);

            this.CommandBindings.Add(new CommandBinding(PaymentsViewModel.AcceptPayment, PaymentsViewModel.AcceptPayment_Executed,
                PaymentsViewModel.AcceptPayment_CanExecute));
            this.CommandBindings.Add(new CommandBinding(PaymentsViewModel.CancelPayment, PaymentsViewModel.CancelPayment_Executed,
                PaymentsViewModel.CancelPayment_CanExecute));
        }
    }
}
