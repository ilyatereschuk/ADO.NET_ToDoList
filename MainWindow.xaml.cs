using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace ADO_ToDoList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel vMdl = new ViewModel();
            todoviewMain.DataContext = vMdl;
            /*
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Addr=(local);Database=ToDoListDb;Trusted_Connection=sspi;User ID=Admin";
            sqlConnection.Open();

            sqlConnection.Close();
             * */
        }
    }
}
