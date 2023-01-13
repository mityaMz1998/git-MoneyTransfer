using System.Windows;
using System.Windows.Input;
using WpfMoneyTransfer.Models;
using WpfMoneyTransfer.ViewModels;

namespace WpfMoneyTransfer.Views
{
    /// <summary>
    /// Interaction logic for the form AddEditClientAndCard.xaml
    /// </summary>
    public partial class AddEditClientAndCard : Window
    {
        public bool flag = false;
        public Client cl;
        public static object lineClient;
        public AddEditClientAndCard(bool flag, Client cl = null)
        {
            InitializeComponent();

            this.flag = flag;
            this.cl = cl;

            MainWindowClientsViewModel.mw.listClientsAndCards.ItemsSource = MainWindowClientsViewModel.Clients;
            AddEditClientAndCardViewModel.addEditClientAndCard = this;
            listCard.ItemsSource = AddEditClientAndCardViewModel.Cards;

            if (flag)
            {
                lineClient = MainWindowClientsViewModel.mw.listClientsAndCards.SelectedItems[0];
                txtSurname.Text = (lineClient as Client).Surname;
                txtName.Text = (lineClient as Client).Name;
                txtMiddleName.Text = (lineClient as Client).MiddleName;
                calendarDateOfBirth.SelectedDate = (lineClient as Client).DateOfBirth;
                txtNumberOfPhone.Text = (lineClient as Client).NumberOfPhone;
                if ((lineClient as Client).ID == (MainWindowClientsViewModel.mw.listClientsAndCards.SelectedItems[0] as Client).ID)
                {
                    MainWindowClientsViewModel.SP_FindId((MainWindowClientsViewModel.mw.listClientsAndCards.SelectedItems[0] as Client).ID);
                }
            }

            DataContext = new AddEditClientAndCardViewModel();

            this.CommandBindings.Add(new CommandBinding(AddEditClientAndCardViewModel.AddCard, AddEditClientAndCardViewModel.AddCard_Executed,
                AddEditClientAndCardViewModel.AddCard_CanExecute));
            this.CommandBindings.Add(new CommandBinding(AddEditClientAndCardViewModel.EditCard, AddEditClientAndCardViewModel.EditCard_Executed,
                AddEditClientAndCardViewModel.EditCard_CanExecute));
            this.CommandBindings.Add(new CommandBinding(AddEditClientAndCardViewModel.SaveChangesCard, AddEditClientAndCardViewModel.SaveChangesCard_Executed,
                AddEditClientAndCardViewModel.SaveChangesCard_CanExecute));
            this.CommandBindings.Add(new CommandBinding(AddEditClientAndCardViewModel.RemoveCard, AddEditClientAndCardViewModel.RemoveCard_Executed,
                AddEditClientAndCardViewModel.RemoveCard_CanExecute));
            this.CommandBindings.Add(new CommandBinding(AddEditClientAndCardViewModel.SaveClientAndCard, AddEditClientAndCardViewModel.SaveClientAndCard_Executed,
                AddEditClientAndCardViewModel.SaveClientAndCard_CanExecute));
            this.CommandBindings.Add(new CommandBinding(AddEditClientAndCardViewModel.CancelCommand, AddEditClientAndCardViewModel.CancelCommand_Executed,
                AddEditClientAndCardViewModel.CancelCommand_CanExecute));
        }
    }
}
