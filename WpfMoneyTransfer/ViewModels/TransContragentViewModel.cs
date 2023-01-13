using System.Configuration;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;

namespace WpfMoneyTransfer.ViewModelы
{
    /// <summary>
    /// Class to сhoosing a payment method
    /// </summary>
    public class TransContragentViewModel
    {
        CheckBox _chSign;
        MaskedTextBox _mskTbInput;
        TextBox _txtTbInput;
        Button _btnSearch;

        static string connectionString;
        public long cardNumber;
        string clientFio;
        public bool cardFound;

        public TransContragentViewModel(CheckBox chSign, MaskedTextBox mskTbInput, TextBox txtTbInput, Button btnSearch)
        {
            _chSign = chSign;
            _mskTbInput = mskTbInput;
            _btnSearch = btnSearch;
            _txtTbInput = txtTbInput;
            _chSign.Tag = this;
            _btnSearch.Tag = this;
            _chSign.Command = Switch;
            _btnSearch.Command = LostFocusButtonSearch;
            _chSign.CommandBindings.Add(new CommandBinding(Switch, Switch_Executed, Switch_CanExecute));
            _btnSearch.CommandBindings.Add(new CommandBinding(LostFocusButtonSearch, LostFocusButtonSearch_Executed, LostFocusButtonSearch_CanExecute));
        }
        public static RoutedCommand Switch { get; set; }
        public static RoutedCommand LostFocusButtonSearch { get; set; }
        static TransContragentViewModel()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            Switch = new RoutedCommand("Switch", typeof(CheckBox));
            LostFocusButtonSearch = new RoutedCommand("LostFocusButtonSeach", typeof(Button));
        }

        /// <summary>
        /// Command to select the payment method (card number or phone number)
        /// </summary>
        public static void Switch_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TransContragentViewModel tr = (sender as CheckBox).Tag as TransContragentViewModel;
            tr._mskTbInput.Text = null;
            tr._txtTbInput.Text = null;
            if (tr._chSign.IsChecked == true)
                tr._mskTbInput.Mask = "0000 0000 0000 0000";
            else
                tr._mskTbInput.Mask = "0(000)000-00-00";
        }
        public static void Switch_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Command to output the search result by phone number
        /// </summary>
        public static void LostFocusButtonSearch_Executed(object sender, ExecutedRoutedEventArgs e) 
        {
            TransContragentViewModel tr = (sender as Button).Tag as TransContragentViewModel;
            tr.SpGetCardNumber1(tr._mskTbInput.Text, tr._chSign.IsChecked.Value);
            if (tr.cardFound == true)
                tr._txtTbInput.Text = $"{tr.cardNumber} ({tr.clientFio})";
        }
        public static void LostFocusButtonSearch_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            TransContragentViewModel tr = (sender as Button).Tag as TransContragentViewModel;
            if (tr._chSign.IsChecked == false)
                e.CanExecute = tr._mskTbInput.Text.Length == 15;
            else
                e.CanExecute = tr._mskTbInput.Text.Length == 19;
        }

        /// <summary>
        /// Stored procedure designed to search for and output a card number by phone number
        /// </summary>
        public void SpGetCardNumber1(string number, bool isCard)
        {
            string sqlTransCardNumber = "SP_GetCardNumder1";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlTransCardNumber, connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter numberParam = new SqlParameter
                {
                    ParameterName = "@number",
                    Value = number
                };
                command.Parameters.Add(numberParam);

                SqlParameter isCardParam = new SqlParameter
                {
                    ParameterName = "@isCard",
                    Value = isCard
                };
                command.Parameters.Add(isCardParam);

                SqlParameter cardNumberParam = new SqlParameter
                {
                    ParameterName = "@cardNumber",
                    SqlDbType = SqlDbType.BigInt
                };
                cardNumberParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(cardNumberParam);

                SqlParameter clientFioParam = new SqlParameter
                {
                    ParameterName = "@clientFio",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 30
                };
                clientFioParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(clientFioParam);
                var result = command.ExecuteNonQuery();

                cardFound = cardNumberParam != null;
                cardNumber = cardFound ? (long)cardNumberParam.Value : 0;
                clientFio = clientFioParam.Value.ToString();
            }
        }
    }
}
