using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfMoneyTransfer.Views;

namespace WpfMoneyTransfer.ViewModelы
{
    /// <summary>
    /// Form for making payments
    /// </summary>
    public class PaymentsViewModel : INotifyPropertyChanged
    {
        public static Payments payments;
        static string connectionString;
        public static RoutedCommand AcceptPayment { get; set; }
        public static RoutedCommand CancelPayment { get; set; }
        static PaymentsViewModel()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            AcceptPayment = new RoutedCommand("AcceptPayment", typeof(Payments));
            CancelPayment = new RoutedCommand("CancelPayment", typeof(Payments));
        }

        /// <summary>
        /// Сommand to make the payment
        /// </summary>
        public static void AcceptPayment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Payments p = sender as Payments;
            SpTransCardNumber(p.send.cardNumber, p.recieve.cardNumber, decimal.Parse(payments.txtSumTransfer.Text));
            ClearElements();
        }
        public static void AcceptPayment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = payments.send.cardFound && payments.recieve.cardFound && payments.txtSumTransfer.Text.Length > 0;
        }

        /// <summary>
        /// Сommand to clearing elements in the form
        /// </summary>
        private static void ClearElements()
        {
            payments.checkFrom.IsChecked = null;
            payments.txtFromCard.Clear();
            payments.txtToCard.Clear();
            payments.txtSumTransfer.Clear();            
        }

        /// <summary>
        /// Stored procedure designed to perform a money transfer
        /// </summary>
        public static void SpTransCardNumber(long from, long to, decimal sum)
        {
            string sqlTransCardNumber = "SP_TransCardNumber";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlTransCardNumber, connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter fromParam = new SqlParameter
                {
                    ParameterName = "@from",
                    Value = from
                };
                command.Parameters.Add(fromParam);
                SqlParameter toParam = new SqlParameter
                {
                    ParameterName = "@to",
                    Value = to
                };
                command.Parameters.Add(toParam);
                SqlParameter sumParam = new SqlParameter
                {
                    ParameterName = "@sum",
                    Value = sum
                };
                command.Parameters.Add(sumParam);
                var result = command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Command to cancel (close the form)
        /// </summary>
        public static void CancelPayment_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (sender as Payments).Close();
        }
        public static void CancelPayment_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
