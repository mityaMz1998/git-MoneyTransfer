using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data;
using System.Windows;
using WpfMoneyTransfer.Models;
using WpfMoneyTransfer.ViewModels;
using System.Data.SqlClient;
using System.Configuration;

namespace WpfMoneyTransfer.Views
{
    public class MainWindowClientsViewModel
    {
        public static MainWindow mw;
        static string connectionString;
        static SqlDataAdapter adapter;
        static DataTable clientsTable;
        static DataTable cardsTable;
        public static int findId;

        public static ObservableCollection<Client> Clients { get; set; }

        public static RoutedCommand ExitCommand { get; set; }
        public static RoutedCommand OpenPayments { get; set;}
        public static RoutedCommand LoadDb { get; set; }
        public static RoutedCommand AddClient { get; set; }
        public static RoutedCommand EditClient { get; set; }
        public static RoutedCommand RemoveClient { get; set; }

        static MainWindowClientsViewModel()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            Clients = new ObservableCollection<Client>();

            ExitCommand = new RoutedCommand("ExitCommand", typeof(MainWindow));
            OpenPayments = new RoutedCommand("OpenPayments", typeof(MainWindow));
            LoadDb = new RoutedCommand("LoadDb", typeof(MainWindow));
            AddClient = new RoutedCommand("AddClient", typeof(MainWindow));
            EditClient = new RoutedCommand("EditClient", typeof(MainWindow));
            RemoveClient = new RoutedCommand("RemoveClient", typeof(MainWindow));           
        }
        public static void AddClient_Executed(object sender, ExecutedRoutedEventArgs e) // команда для открытия формы добавления клиентов и карточек
        {
            AddEditClientAndCard addEditClientAndCard = new AddEditClientAndCard(false);
            addEditClientAndCard.Owner = mw;
            addEditClientAndCard.ShowDialog();
        }
        public static void AddClient_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        public static void EditClient_Executed(object sender, ExecutedRoutedEventArgs e) // команда для открытия формы обновления (редактирования) клиентов и карточек
        {
            Client cl = (sender as MainWindow).listClientsAndCards.SelectedItem as Client;
            if (cl != null)
            {
                AddEditClientAndCard addEditClientAndCard = new AddEditClientAndCard(true, cl);
                addEditClientAndCard.Owner = mw;
                addEditClientAndCard.ShowDialog();
            }
        }
        public static void EditClient_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as MainWindow).listClientsAndCards.SelectedItem != null;
        }
        public static void RemoveClient_Executed(object sender, ExecutedRoutedEventArgs e) // команда для удаления клента из списка
        {
            if (mw.listClientsAndCards.Items.Count != 0)
            {
                var res = MessageBox.Show("Вы действительно хотите удалить этого клиента?",
                                          "Удалить объект", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    AddEditClientAndCardViewModel.SP_RemoveClient((mw.listClientsAndCards.SelectedItems[0] as Client).ID);

                    foreach (Card card in AddEditClientAndCardViewModel.Cards)
                    {
                        AddEditClientAndCardViewModel.SP_RemoveCard(AddEditClientAndCardViewModel.ClientRemoveId);
                    }
                    var line = mw.listClientsAndCards.SelectedItems[0];
                    Clients.Remove((Client)line);
                    mw.listClientsAndCards.Items.Refresh();
                }
            }
        }
        public static void RemoveClient_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as MainWindow).listClientsAndCards.SelectedItem != null;
        }
        public static void LoadDb_Executed(object sender, ExecutedRoutedEventArgs e) // команда для загрузки таблицы клиентов в список
        {
            string sql = "SELECT * FROM Clients";
            clientsTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(clientsTable);
                foreach (DataRow row in clientsTable.Rows)
                {
                    Clients.Add(new Client((int)row["ID"], (string)row["Surname"], (string)row["Name"], (string)row["MiddleName"], (DateTime)row["DateOfBirth"],
                               (string)row["NumberOfPhone"]));
                }
                mw.listClientsAndCards.ItemsSource = Clients;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
        public static void LoadDb_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        public static void OpenPayments_Executed(object sender, ExecutedRoutedEventArgs e) // команда для открытия формы платежей
        {
            Payments payments = new Payments();
            payments.Owner = mw;
            payments.ShowDialog();
        }
        public static void OpenPayments_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        public static void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e) // команда для закрытия формы
        {
            (sender as MainWindow).Close();
        }
        public static void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        public static void SP_FindId(int clientId) // хранимая процедура, предназначенная для вывода карточных данных указанного клиента
        {
            string sql = "SELECT c.Id,CardNumber,ExpirationDate,Balance,BindingPhone,c.ClientId FROM Clients as cl " +
                         "INNER JOIN Cards as c on (cl.Id = c.ClientId) WHERE cl.Id =" + clientId;
            cardsTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                connection.Open();
                adapter.Fill(cardsTable);
                foreach (DataRow row in cardsTable.Rows)
                {
                    AddEditClientAndCardViewModel.Cards.Add(new Card((int)row["Id"], (long)row["CardNumber"], (string)row["ExpirationDate"], (decimal)row["Balance"], (bool)row["BindingPhone"],
                               (int)row["ClientId"], true));
                }
                AddEditClientAndCardViewModel.addEditClientAndCard.listCard.ItemsSource = AddEditClientAndCardViewModel.Cards;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
    }
}
