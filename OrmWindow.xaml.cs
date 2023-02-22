using ADO_201.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
    /// Interaction logic for OrmWindow.xaml
    /// </summary>
    public partial class OrmWindow : Window
    {
        public ObservableCollection<Entity.Department> Departments { get; set; }
        public ObservableCollection<Entity.Product> Products { get; set; }
        public ObservableCollection<Entity.Manager> Managers { get; set; }
        public ObservableCollection<Entity.Sale> Sales { get; set; }

        private DepartmentCrudWindow _dialogDepartment;
        private SqlConnection _connection;

        public OrmWindow()
        {
            InitializeComponent();
            Departments = new();
            Managers = new();
            Products = new();
            Sales = new();
            DataContext = this;
            _connection = new(App.ConnectionString);
            _dialogDepartment = null!;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                SqlCommand cmd = new() { Connection = _connection };

                #region Load Departments
                cmd.CommandText = "SELECT D.Id, D.Name FROM Departments D";
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    Departments.Add(new Entity.Department
                    {
                        Id = reader.GetGuid(0),
                        Name = reader.GetString(1)
                    });
                }
                reader.Close();
                #endregion

                #region Load Products
                cmd.CommandText = "SELECT P.* FROM Products P WHERE P.DeleteDt IS NULL";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add(new(reader));
                }
                reader.Close();
                #endregion

                #region Load Managers
                cmd.CommandText = "SELECT M.Id, M.Surname, M.Name, M.Secname, M.Id_main_dep, M.Id_sec_dep, M.Id_chief  FROM Managers M";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(new Entity.Manager
                    {
                        Id = reader.GetGuid(0),
                        Surname = reader.GetString(1),
                        Name = reader.GetString(2),
                        Secname = reader.GetString(3),
                        IdMainDep = reader.GetGuid(4),
                        IdSecDep = reader.GetValue(5) == DBNull.Value
                                    ? null
                                    : reader.GetGuid(5),
                        IdChief = reader.IsDBNull(6)
                                    ? null
                                    : reader.GetGuid(6)
                    });
                }
                reader.Close();
                #endregion

                #region Load Sales
                cmd.CommandText = "SELECT S.* FROM Sales S";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Sales.Add(new(reader));
                }
                reader.Close();
                #endregion

                cmd.Dispose();
            }
            catch ( SqlException ex )
            {
                MessageBox.Show(
                    ex.Message,
                    "Window will be closed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // робота з БД через ORM зведена до роботи з колекцією
            // sender - item, що містить посилання на Entity.Department у колекції Departments
            if(sender is ListViewItem item)
            {
                if(item.Content is Entity.Department department)
                {
                    _dialogDepartment = new();
                    _dialogDepartment.Department = department;
                    if (_dialogDepartment.ShowDialog() == true)
                    {
                        if(_dialogDepartment.Department is null)  // Delete
                        {
                            MessageBox.Show("Deleted");
                        }
                        else  // Update
                        {
                            MessageBox.Show(department.ToString());
                        }                        
                    }
                }
            }
        }
        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    ManagerCrudWindow dialog = new() 
                    { 
                        Owner = this, 
                        Manager = manager 
                    };
                    if (dialog.ShowDialog() == true)
                    {
                        MessageBox.Show(dialog.Manager.ToString());
                    }
                }
            }
        }
        private void ProductsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Product product)
                {
                    ProductCrudWindow dialog = new() { Product = product };
                    if (dialog.ShowDialog() == true)
                    {
                        if (dialog.Product is null)  // Delete
                        {
                            MessageBox.Show("Deleted");
                        }
                        else  // Update
                        {
                            MessageBox.Show(product.Name + " " + product.Price);
                        }
                    }
                }
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProductCrudWindow dialog = new() { Product = null! };  //  Product = null - ознака створення нового товару
            if (dialog.ShowDialog() == true)
            {
                if (dialog.Product is not null) 
                {
                    String sql = "INSERT INTO Products(Id, Name, Price) VALUES( @id, @name, @price ) ";
                    using SqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.Product.Id);
                    cmd.Parameters.AddWithValue("@name", dialog.Product.Name);
                    cmd.Parameters.AddWithValue("@price", dialog.Product.Price);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Insert OK");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    /* Не рекомендується (див. CRUD.txt)
                    String sql = $"INSERT INTO Products(Id, Name, Price) " +
                        $"VALUES('{dialog.Product.Id}', N'{dialog.Product.Name}', {dialog.Product.Price}) ";
                    using SqlCommand cmd = new(sql, _connection);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Insert OK");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    */
                    // MessageBox.Show(dialog.Product.Name + " " + dialog.Product.Price);
                }
            }
        }


        private void SalesItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Sale sale)
                {
                    MessageBox.Show(sale.SaleDt.ToString());
                }
            }
        }
        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            SaleCrudWindow dialog = new(null!) { Owner = this } ;
            if (dialog.ShowDialog() == true)
            {
                using SqlCommand cmd = new(
                    $"INSERT INTO Sales(Id, ProductId, ManagerId, Quantity, SaleDt) " +
                    $"VALUES(@id, @prod, @manager, @count, @moment)", _connection
                );
                cmd.Parameters.AddWithValue("@id",      dialog.Sale.Id);
                cmd.Parameters.AddWithValue("@prod",    dialog.Sale.ProductId);
                cmd.Parameters.AddWithValue("@manager", dialog.Sale.ManagerId);
                cmd.Parameters.AddWithValue("@count",   dialog.Sale.Quantity);
                cmd.Parameters.AddWithValue("@moment",  dialog.Sale.SaleDt);
                try
                {
                    cmd.ExecuteNonQuery();
                    this.Sales.Add(dialog.Sale);
                    MessageBox.Show("Insert OK");
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Insert Fails: " + ex.Message);
                }
            }
            /* Д.З. Реалізувати задачі "Update", "Delete" для Sales
             */
        }
    }
}
