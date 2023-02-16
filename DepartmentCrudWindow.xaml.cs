using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADO_201
{
    /// <summary>
    /// Interaction logic for DepartmentCrudWindow.xaml
    /// </summary>
    public partial class DepartmentCrudWindow : Window
    {
        // обмінне поле - передається з викликаючого вікна
        public Entity.Department Department { get; set; }

        public DepartmentCrudWindow()
        {
            InitializeComponent();
            Department = null!;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Department is null)  // режим додавання (Create)
            {
                DeleteButton.IsEnabled = false;
            }
            else  // режими редагування чи видалення (U / D)
            {
                IdView.Text = Department.Id.ToString();
                NameView.Text = Department.Name;
                DeleteButton.IsEnabled = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Department.Name = NameView.Text;
            this.DialogResult = true;
            // this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Department = null!;
            this.DialogResult = true;
            // this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;  // те, що поверне ShowDialog
            // this.Close();
        }
    }
}
/* Д.З. Реалізувати роботу CRUD :
 * за натиском кнопки Save
 * - перевірити нову назву на порожність, видати повідомлення
 * - перевірити нову назву на збіг з попередньою (немає змін) - Cancel
 * - у разі OK подати команду БД на зміну назви відділу, повідомити про її
 *     результат (успіх/помилка)
 * за кнопкою Delete
 * - запитати підтвердження (Певні? так/ні)
 * - у разі OK подати команду БД на зміну назви відділу, повідомити про її
 *     результат (успіх/помилка)
 * у OrmWindow поруч з переліком відділів додати кнопку "Створити новий відділ",
 * реалізувати її роботу
 * - згенерувати новий Id
 * - запитати назву, перевірити а) на порожність, б) на наявність у БД такої ж
 * - ...
 */