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
using System.Windows.Shapes;

namespace ADO_ToDoList
{
    /// <summary>
    /// Логика взаимодействия для AddTaskDialog.xaml
    /// </summary>
    public partial class AddTaskDialog : Window
    {
        
        public AddTaskDialog(DateTime minDate, UInt16 sliderMinVal, UInt16 sliderMaxVal)
        {
            InitializeComponent();
            sldPriority.Interval = 1;
            sldPriority.LargeChange = 1;
            sldPriority.SmallChange = 1;
            sldPriority.Minimum = sliderMinVal;
            sldPriority.Maximum = sliderMaxVal;
            datepickerStartDate.DisplayDateStart = minDate;
            datepickerDueDate.DisplayDateStart = minDate;
        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Простая проверка
            Boolean allFilled = (
                !String.IsNullOrWhiteSpace(tbTitle.Text) &&
                !String.IsNullOrWhiteSpace(tbDescription.Text) &&
                !String.IsNullOrWhiteSpace(datepickerStartDate.Text) &&
                !String.IsNullOrWhiteSpace(datepickerDueDate.Text) 
            );
            if (allFilled)
            {
                this.DialogResult = true;
                Close();
            }
            else MessageBox.Show("Заполните все поля");
        }

        public new TaskEntity ShowDialog()
        {
            bool? result = base.ShowDialog();
            if (result ?? false)
            {
                String title = tbTitle.Text,
                    description = tbDescription.Text;
                DateTime startDate = (DateTime)datepickerStartDate.SelectedDate,
                    dueDate = (DateTime)datepickerDueDate.SelectedDate;
                int priority = Convert.ToUInt16(sldPriority.Value);
            TaskEntity taskEntity = new TaskEntity(title, description, startDate,
                    dueDate, priority, false);

                return taskEntity;
            }
            else return null;
        }

        private void sldPriority_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }
    }
}
