using ADO_201.DAL;
using ADO_201.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for DalWindow.xaml
    /// </summary>
    public partial class DalWindow : Window
    {
        private readonly DataContext dataContext;

        public ObservableCollection<Entity.Department> DepartmentList { get; set; }
        public ObservableCollection<Entity.Manager> ManagerList { get; set; }

        public DalWindow()
        {
            InitializeComponent();
            dataContext = new();
            DepartmentList = new(dataContext.Departments.GetAll());
            ManagerList = new(dataContext.Managers.GetAll());
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show(dataContext.Departments.GetAll().Count.ToString());
        }
        
        private void DepartmentsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Department department)
                {
                    MessageBox.Show(department.ToString());
                }
            }
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            DepartmentCrudWindow dialog = new();
            if(dialog.ShowDialog() == true)
            {
                if(dataContext.Departments.Add(dialog.Department))
                {
                    MessageBox.Show("Додано успішно");
                    DepartmentList.Add(dialog.Department);
                }
                else
                {
                    MessageBox.Show("Помилка додавання");
                }
            }
        }

        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    MessageBox.Show(manager.ToString());
                }
            }
        }
        private void AddManagerButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
/* Д.З. Реалізувати методи 
 *  Update(Entity.Department department) 
 *  Delete(Entity.Department department) 
 * на рівні DAL
 */