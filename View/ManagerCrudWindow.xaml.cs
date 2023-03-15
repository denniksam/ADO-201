using ADO_201.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for ManagerCrudWindow.xaml
    /// </summary>
    public partial class ManagerCrudWindow : Window
    {
        public Entity.Manager? Manager;

        // посилання на Owner.Departments
        private ObservableCollection<Entity.Department> OwnerDepartments;

        public ManagerCrudWindow()
        {
            InitializeComponent();
            Manager = null;
            OwnerDepartments = null!;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Owner is OrmWindow owner) {
                DataContext = Owner;
                OwnerDepartments = owner.Departments;
            }
            else
            {
                MessageBox.Show("Owner is not OrmWindow");
                Close();
            }

            if (this.Manager is null)
            {
                Manager = new Entity.Manager();  // Id генерується у конструкторі
                DeleteButton.IsEnabled = false;
            }
            else
            {
                SurnameView.Text = this.Manager.Surname; 
                NameView.Text = this.Manager.Name; 
                SecnameView.Text = this.Manager.Secname;
                MainDepComboBox.SelectedItem =
                    OwnerDepartments
                    .Where(d => d.Id == this.Manager.IdMainDep)
                    .First();
                SecDepComboBox.SelectedItem =
                    OwnerDepartments
                    .Where(d => d.Id == this.Manager.IdSecDep)
                    .FirstOrDefault();
                ChiefComboBox.SelectedItem = 
                    (Owner as OrmWindow)?
                    .Managers
                    .FirstOrDefault(m => m.Id == this.Manager.IdChief);
                DeleteButton.IsEnabled = true;
            }
            IdView.Text = this.Manager.Id.ToString();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Manager is null) { return; }

            if(NameView.Text.Equals(String.Empty))
            {
                MessageBox.Show("Необхідно ввести прізвище");
                NameView.Focus();
                return;
            }
            if(MainDepComboBox.SelectedItem is null) 
            {
                MessageBox.Show("Необхідно вибрати робочий відділ");
                MainDepComboBox.Focus();
                return;
            }

            this.Manager.Surname = SurnameView.Text;
            this.Manager.Name = NameView.Text;
            this.Manager.Secname = SecnameView.Text;

            if (MainDepComboBox.SelectedItem is Entity.Department maindep)  
                this.Manager.IdMainDep = maindep.Id;
            else
                MessageBox.Show("MainDepComboBox.SelectedItem CAST Error");

            if(SecDepComboBox.SelectedItem is null)
                this.Manager.IdSecDep = null;
            else if(SecDepComboBox.SelectedItem is Entity.Department secdep)
                this.Manager.IdSecDep = secdep.Id;
            else
                MessageBox.Show("SecDepComboBox.SelectedItem CAST Error");

            this.DialogResult = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; 
        }

        private void ClearChiefButton_Click(object sender, RoutedEventArgs e)
        {
            ChiefComboBox.SelectedItem = null;
        }

        private void ClearSecDepButton_Click(object sender, RoutedEventArgs e)
        {
            SecDepComboBox.SelectedIndex = -1;
        }
    }
}
/* Д.З. Закінчити роботу з CRUD співробітників
 * Додати ознаку звільнення (видалення з БД)
 * Реалізувати внесення усіх змін до БД
 */