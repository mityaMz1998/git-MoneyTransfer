using System.Windows;
using System.Windows.Input;

namespace WpfMoneyTransfer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowClientsViewModel.mw = this;
            DataContext = new MainWindowClientsViewModel();

            this.CommandBindings.Add(new CommandBinding(MainWindowClientsViewModel.ExitCommand, MainWindowClientsViewModel.ExitCommand_Executed,
                MainWindowClientsViewModel.ExitCommand_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MainWindowClientsViewModel.OpenPayments, MainWindowClientsViewModel.OpenPayments_Executed,
                MainWindowClientsViewModel.OpenPayments_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MainWindowClientsViewModel.LoadDb, MainWindowClientsViewModel.LoadDb_Executed,
                MainWindowClientsViewModel.LoadDb_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MainWindowClientsViewModel.AddClient, MainWindowClientsViewModel.AddClient_Executed,
                MainWindowClientsViewModel.AddClient_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MainWindowClientsViewModel.EditClient, MainWindowClientsViewModel.EditClient_Executed,
                MainWindowClientsViewModel.EditClient_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MainWindowClientsViewModel.RemoveClient, MainWindowClientsViewModel.RemoveClient_Executed,
                MainWindowClientsViewModel.RemoveClient_CanExecute));
        }
    }
}
