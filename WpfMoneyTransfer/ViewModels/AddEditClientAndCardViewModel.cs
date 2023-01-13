using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using WpfMoneyTransfer.Models;
using System.Configuration;
using System.Windows.Input;
using System.Data;
using System.Windows;
using System.Data.SqlClient;
using WpfMoneyTransfer.Views;

namespace WpfMoneyTransfer.ViewModels
{
    /// <summary>
    /// Сlass for adding, editing, and deleting clients and their cards
    /// </summary>
    public class AddEditClientAndCardViewModel : INotifyPropertyChanged
    {
        public static AddEditClientAndCard addEditClientAndCard;

        static string connectionString;
        static object line;

        static bool flgEdit;
        static bool flgSave;

        static bool flgCommandAddCard;
        static bool flgCommandRemoveCard;

        static int ClientAddId;
        public static int ClientEditId;
        public static int ClientRemoveId;
        
        public static List<int> lstRemoveCards = new List<int>();
        public static ObservableCollection<Card> Cards { get; set; }

        public static RoutedCommand AddCard { get; set; }
        public static RoutedCommand EditCard { get; set; }
        public static RoutedCommand SaveChangesCard { get; set; }
        public static RoutedCommand RemoveCard { get; set; }
        public static RoutedCommand SaveClientAndCard { get; set; }
        public static RoutedCommand CancelCommand { get; set; }

        static AddEditClientAndCardViewModel()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            
            Cards = new ObservableCollection<Card>();

            AddCard = new RoutedCommand("ExitCommand", typeof(AddEditClientAndCard));
            EditCard = new RoutedCommand("OpenPayments", typeof(AddEditClientAndCard));
            SaveChangesCard = new RoutedCommand("LoadDb", typeof(AddEditClientAndCard));
            RemoveCard = new RoutedCommand("RemoveCard", typeof(AddEditClientAndCard));
            SaveClientAndCard = new RoutedCommand("SaveClientAndCard", typeof(MainWindow));
            CancelCommand = new RoutedCommand("CancelCommand", typeof(MainWindow));
        }

        /// <summary>
        /// Method for autoincrement of a new record
        /// </summary>
        private static int GetIDCard()
        {
            return Cards.Count() > 0 ? Cards.Max<Card>(x => x.ID) + 1 : 1;
        }

        /// <summary>
        /// Method for clearing card elements
        /// </summary>
        private static void ClearCardsElements()
        {
            addEditClientAndCard.txtCardNumber.Clear();
            addEditClientAndCard.txtExpirationDate.Clear();
            addEditClientAndCard.txtBalance.Clear();
            addEditClientAndCard.chkBindingPhone.IsChecked = false;
        }

        /// <summary>
        /// Method for clearing all elements int the form
        /// </summary>
        private static void ClearAllElements()
        {
            addEditClientAndCard.txtSurname.Clear();
            addEditClientAndCard.txtName.Clear();
            addEditClientAndCard.txtMiddleName.Clear();
            addEditClientAndCard.calendarDateOfBirth.SelectedDate = null;
            addEditClientAndCard.txtNumberOfPhone.Clear();
            Cards.Clear();
        }

        /// <summary>
        /// Command to add a new card
        /// </summary>
        public static void AddCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Card c = new Card(GetIDCard(), long.Parse(addEditClientAndCard.txtCardNumber.Text.Replace(" ", "")), addEditClientAndCard.txtExpirationDate.Text,
                                decimal.Parse(addEditClientAndCard.txtBalance.Text), addEditClientAndCard.chkBindingPhone.IsChecked.Value, 0 , false);
            Cards.Add(c);
            addEditClientAndCard.listCard.Items.Refresh();
            ClearCardsElements();
            flgCommandAddCard = true;
        }
        public static void AddCard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Command for update (edit) card
        /// </summary>
        public static void EditCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Card c = (sender as AddEditClientAndCard).listCard.SelectedItem as Card;
            if (c != null)
            {
                line = addEditClientAndCard.listCard.SelectedItems[0];

                string text = (line as Card).CardNumber.ToString();
                text = text.Insert(4, " ");
                text = text.Insert(9, " ");
                text = text.Insert(14, " ");

                addEditClientAndCard.txtCardNumber.Text = text;
                addEditClientAndCard.txtExpirationDate.Text = (line as Card).ExpirationDate;
                addEditClientAndCard.txtBalance.Text = (line as Card).Balance.ToString();
                addEditClientAndCard.chkBindingPhone.IsChecked = (line as Card).BindingPhone;
                addEditClientAndCard.listCard.Items.Refresh();
            }
            flgEdit = true;
        }
        public static void EditCard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as AddEditClientAndCard).listCard.SelectedItem != null;
        }

        /// <summary>
        /// Command to save card changes
        /// </summary>
        public static void SaveChangesCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int ID = (addEditClientAndCard.listCard.SelectedItems[0] as Card).ID;
            (line as Card).CardNumber = long.Parse(addEditClientAndCard.txtCardNumber.Text.Replace(" ", ""));
            (line as Card).ExpirationDate = addEditClientAndCard.txtExpirationDate.Text;
            (line as Card).Balance = decimal.Parse(addEditClientAndCard.txtBalance.Text);
            (line as Card).BindingPhone = addEditClientAndCard.chkBindingPhone.IsChecked.Value;
            Card c = Cards.FirstOrDefault(x => x.ID == ID);
            c = line as Card;
            addEditClientAndCard.listCard.Items.Refresh();
            ClearCardsElements();
            flgEdit = false;
            flgSave = true;
        }
        public static void SaveChangesCard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (flgEdit == true)
                e.CanExecute = true;
        }

        /// <summary>
        /// Command to delete the card
        /// </summary>
        public static void RemoveCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (addEditClientAndCard.listCard.Items.Count != 0)
            {
                var res = MessageBox.Show("Do you really want to delete this card?",
                                          "Delete object", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    var line = addEditClientAndCard.listCard.SelectedItems[0];
                    if (((Card)line)._inSQL == true)
                        lstRemoveCards.Add(((Card)line).ID);
                    Cards.Remove((Card)line);
                    addEditClientAndCard.listCard.Items.Refresh();
                }
            }
            flgCommandRemoveCard = true;
        }
        public static void RemoveCard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as AddEditClientAndCard).listCard.SelectedItem != null;
        }

        /// <summary>
        /// Command to save the client's data and his cards
        /// </summary>
        public static void SaveClientAndCard_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (addEditClientAndCard.flag)
            {
                int ID = (MainWindowClientsViewModel.mw.listClientsAndCards.SelectedItems[0] as Client).ID;
                (AddEditClientAndCard.lineClient as Client).Surname = addEditClientAndCard.txtSurname.Text;
                (AddEditClientAndCard.lineClient as Client).Name = addEditClientAndCard.txtName.Text;
                (AddEditClientAndCard.lineClient as Client).MiddleName = addEditClientAndCard.txtMiddleName.Text;
                (AddEditClientAndCard.lineClient as Client).DateOfBirth = (DateTime)addEditClientAndCard.calendarDateOfBirth.SelectedDate;
                (AddEditClientAndCard.lineClient as Client).NumberOfPhone = addEditClientAndCard.txtNumberOfPhone.Text;

                SP_EditClient((AddEditClientAndCard.lineClient as Client).Surname, (AddEditClientAndCard.lineClient as Client).Name, (AddEditClientAndCard.lineClient as Client).MiddleName,
                            (AddEditClientAndCard.lineClient as Client).DateOfBirth, (AddEditClientAndCard.lineClient as Client).NumberOfPhone, ID);

                if (flgCommandAddCard == true)
                {
                    foreach (Card card in Cards.Where(x => x._inSQL == false))
                    {
                        SP_AddCard(card.CardNumber, card.ExpirationDate, card.Balance, card.BindingPhone, ClientEditId);
                    }
                }
                if (flgCommandRemoveCard == true)
                {
                    foreach (int i in lstRemoveCards)
                    {
                        SP_RemoveCard(i);
                    }
                }
                foreach (Card card in Cards)
                {
                    SP_EditCard(card.CardNumber, card.ExpirationDate, card.Balance, card.BindingPhone, ClientEditId, card.ID);
                }

                Client cl = MainWindowClientsViewModel.Clients.FirstOrDefault(x => x.ID == ID);
                cl = AddEditClientAndCard.lineClient as Client;
                MainWindowClientsViewModel.mw.listClientsAndCards.Items.Refresh();
            }
            else
            {                
                SP_AddClient(addEditClientAndCard.txtSurname.Text, addEditClientAndCard.txtName.Text, addEditClientAndCard.txtMiddleName.Text,
                            (DateTime)addEditClientAndCard.calendarDateOfBirth.SelectedDate, addEditClientAndCard.txtNumberOfPhone.Text);

                foreach (Card card in Cards)
                {
                    SP_AddCard(card.CardNumber, card.ExpirationDate, card.Balance, card.BindingPhone, ClientAddId);
                }

                Client cl = new Client(ClientAddId, addEditClientAndCard.txtSurname.Text, addEditClientAndCard.txtName.Text, addEditClientAndCard.txtMiddleName.Text,
                                      (DateTime)addEditClientAndCard.calendarDateOfBirth.SelectedDate, addEditClientAndCard.txtNumberOfPhone.Text);
                MainWindowClientsViewModel.Clients.Add(cl);
                MainWindowClientsViewModel.mw.listClientsAndCards.Items.Refresh();
            }
            ClearAllElements();
            addEditClientAndCard.Close();
        }
        public static void SaveClientAndCard_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrEmpty((sender as AddEditClientAndCard).txtSurname.Text) && !String.IsNullOrEmpty((sender as AddEditClientAndCard).txtName.Text) &&
            !String.IsNullOrEmpty((sender as AddEditClientAndCard).txtMiddleName.Text) && (sender as AddEditClientAndCard).calendarDateOfBirth.SelectedDate != null &&
            !String.IsNullOrEmpty((sender as AddEditClientAndCard).txtNumberOfPhone.Text) && Cards.Count != 0;
        }

        /// <summary>
        /// Command to cancel (close the form)
        /// </summary>
        public static void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (sender as AddEditClientAndCard).Close();
            Cards.Clear();
        }
        public static void CancelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Stored procedure for adding a card
        /// </summary>
        public static void SP_AddCard(long cardNumber, string expiretionDate, decimal balance, bool bindingPhone, int clientId)
        {
            string sqlAddCard = "SP_AddCard";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlAddCard, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter cardNumberParam = new SqlParameter
                {
                    ParameterName = "@cardNumber",
                    Value = cardNumber
                };
                command.Parameters.Add(cardNumberParam);
                SqlParameter expirationDateParam = new SqlParameter
                {
                    ParameterName = "@expirationDate",
                    Value = expiretionDate
                };
                command.Parameters.Add(expirationDateParam);
                SqlParameter balanceParam = new SqlParameter
                {
                    ParameterName = "@balance",
                    Value = balance
                };
                command.Parameters.Add(balanceParam);
                SqlParameter bindingPhoneParam = new SqlParameter
                {
                    ParameterName = "@bindingPhone",
                    Value = bindingPhone
                };
                command.Parameters.Add(bindingPhoneParam);
                SqlParameter clientIdParam = new SqlParameter
                {
                    ParameterName = "@clientId",
                    Value = clientId
                };
                command.Parameters.Add(clientIdParam);
                var result = command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Stored procedure designed to update (edit) the card
        /// </summary>
        public static void SP_EditCard(long cardNumber, string expirationDate, decimal balance, bool bindingPhone, int clientId, int Id)
        {
            string sqlEditCard = "SP_EditCard";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlEditCard, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter cardNumberParam = new SqlParameter
                {
                    ParameterName = "@cardNumber",
                    Value = cardNumber
                };
                command.Parameters.Add(cardNumberParam);
                SqlParameter expirationDateParam = new SqlParameter
                {
                    ParameterName = "@expirationDate",
                    Value = expirationDate
                };
                command.Parameters.Add(expirationDateParam);
                SqlParameter balanceParam = new SqlParameter
                {
                    ParameterName = "@balance",
                    Value = balance
                };
                command.Parameters.Add(balanceParam);
                SqlParameter bindingPhoneParam = new SqlParameter
                {
                    ParameterName = "@bindingPhone",
                    Value = bindingPhone
                };
                command.Parameters.Add(bindingPhoneParam);
                SqlParameter clientIdParam = new SqlParameter
                {
                    ParameterName = "@clientId",
                    Value = clientId
                };
                command.Parameters.Add(clientIdParam);
                SqlParameter IdParam = new SqlParameter
                {
                    ParameterName = "@Id",
                    Value = Id
                };
                command.Parameters.Add(IdParam);
                var result = command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Stored procedure designed to delete a card
        /// </summary>
        public static void SP_RemoveCard(int clientId)
        {
            string sqlRemoveCard = "SP_RemoveCard";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlRemoveCard, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter IdParam = new SqlParameter
                {
                    ParameterName = "@clientId",
                    Value = clientId
                };
                command.Parameters.Add(IdParam);
                var result = command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Stored procedure designed to add a client
        /// </summary>
        public static void SP_AddClient(string surname, string name, string middleName, DateTime dateOfBirth, string numberOfPhone)
        {
            string sqlAddClient = "SP_AddClient";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlAddClient, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter surnameParam = new SqlParameter
                {
                    ParameterName = "@surname",
                    Value = surname
                };
                command.Parameters.Add(surnameParam);
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = name
                };
                command.Parameters.Add(nameParam);
                SqlParameter middleParam = new SqlParameter
                {
                    ParameterName = "@middleName",
                    Value = middleName
                };
                command.Parameters.Add(middleParam);
                SqlParameter dateOfBirthParam = new SqlParameter
                {
                    ParameterName = "@dateOfBirth",
                    Value = dateOfBirth
                };
                command.Parameters.Add(dateOfBirthParam);
                SqlParameter numberOfPhoneParam = new SqlParameter
                {
                    ParameterName = "@numberOfPhone",
                    Value = numberOfPhone
                };
                command.Parameters.Add(numberOfPhoneParam);
                SqlParameter IdParam = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int
                };
                IdParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(IdParam);
                var result = command.ExecuteNonQuery();
                ClientAddId = (int)IdParam.Value;
            }
        }

        /// <summary>
        /// Stored procedure designed to update (edit) the client
        /// </summary>
        public static void SP_EditClient(string surname, string name, string middleName, DateTime dateOfBirth, string numberOfPhone, int Id)
        {
            string sqlEditClient = "SP_EditClient";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlEditClient, connection);                
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter surnameParam = new SqlParameter
                {
                    ParameterName = "@surname",
                    Value = surname
                };
                command.Parameters.Add(surnameParam);
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = name
                };
                command.Parameters.Add(nameParam);
                SqlParameter middleNameParam = new SqlParameter
                {
                    ParameterName = "@middlename",
                    Value = middleName
                };
                command.Parameters.Add(middleNameParam);
                SqlParameter dateOfBirthParam = new SqlParameter
                {
                    ParameterName = "@dateOfBirth",
                    Value = dateOfBirth
                };
                command.Parameters.Add(dateOfBirthParam);
                SqlParameter numberOfPhoneParam = new SqlParameter
                {
                    ParameterName = "@numberOfPhone",
                    Value = numberOfPhone
                };
                command.Parameters.Add(numberOfPhoneParam);
                SqlParameter IdParam = new SqlParameter
                {
                    ParameterName = "@Id",
                    Value = Id
                };
                command.Parameters.Add(IdParam);
                var result = command.ExecuteNonQuery();
                ClientEditId = (int)IdParam.Value;
            }
        }

        /// <summary>
        /// Stored procedure designed to delete a client
        /// </summary>
        public static void SP_RemoveClient(int Id)
        {
            string sqlRemoveClient = "SP_RemoveClient";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlRemoveClient, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter IdParam = new SqlParameter
                {
                    ParameterName = "@Id",
                    Value = Id
                };
                command.Parameters.Add(IdParam);
                var result = command.ExecuteNonQuery();
                ClientRemoveId = (int)(IdParam.Value);
            }
        }
    }
}
