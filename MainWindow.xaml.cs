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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.SqlClient;   // Не забути NuGet
using System.IO;

namespace ADO_201
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // об'єкт підключення: головний елемент ADO
        private SqlConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
            // !! Створення об'єкта не відкриває підключення
            _connection = new();
            // Головний параметр підключення - рядок підключення
            _connection.ConnectionString = App.ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                StatusConnection.Content = "Connected";
                StatusConnection.Foreground = Brushes.Green;
            }
            catch (SqlException ex)
            {
                StatusConnection.Content = "Disconnected";
                StatusConnection.Foreground = Brushes.Red;
                MessageBox.Show(ex.Message);
                this.Close();
            }
            ShowMonitor();
            ShowDepartments();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_connection?.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        #region Запити без повернення результатів
        private void InstallDepartments_Click(object sender, RoutedEventArgs e)
        {
            // Команда - інструмент для виконання SQL запитів
            SqlCommand cmd = new();
            // Головні параметри команди:
            cmd.Connection = _connection;  // підключення (відкрите)
            cmd.CommandText =             // SQL запит (текст)
                @"CREATE TABLE Departments (
	                Id			UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	                Name		NVARCHAR(50) NOT NULL
                )";
            try                           // Виконання команди
            {
                cmd.ExecuteNonQuery();    // NonQuery - без повернення рез-ту
                MessageBox.Show("Create OK");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            cmd.Dispose();  // команда - некерований ресурс, вимагає утилізації
        }

        private void InstallProducts_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand cmd = new() { Connection = _connection };
            cmd.CommandText = @"CREATE TABLE Products (
	                                Id	  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	                                Name  NVARCHAR(50) NOT NULL,
	                                Price FLOAT  NOT NULL
                                )";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            cmd.CommandText = @"INSERT INTO Products
	                                ( Id, Name,	Price	)
                                VALUES
                                    ( 'DA1E17BB-A90D-4C79-B801-5462FB070F57', N'Гвоздь 100мм',			10.50	),
                                    ( 'A8E6BE17-5447-4804-AB61-F31ABF5A76D3', N'Шуруп 4х35',			4.25	),
                                    ( '21B0F444-2E4F-47D8-80C1-E69BF1C34CA8', N'Гайка М4',				6.50	),
                                    ( '2DCA5E44-B06D-4613-BB6A-D3BC91430BFE', N'Гровер М4',			    5.99	),
                                    ( '64A4DF8A-0733-4BE9-AABA-C01B4EC3612A', N'Болт 4х60',			    9.98	),
                                    ( 'B6D20749-B495-4B1A-BA1C-80B88E78B7CD', N'Гвоздь 80мм',			19.98	),
                                    ( '7B08197B-C55F-4389-891F-BF12A575DFFB', N'Отвертка PZ2',			35.50	),
                                    ( '870DA1A9-44F4-4018-B7FC-727A2058FAF0', N'Шуруповерт',			799		),
                                    ( '8FF90E21-DCDB-4D55-A557-7C6D57DBB029', N'Молоток',				216.50	),
                                    ( 'F7F1E576-AF8D-4749-869E-4A794FE69D42', N'Набор ""Новосел""',		52.40	),
                                    ( 'BB29F63D-1261-41F2-89E8-88F44D5EC409', N'Сверло 6х80',			39.98	),
                                    ( 'D17A4442-0A71-4673-B450-36929048ADEF', N'Шуруп 5х45',			5.98	),
                                    ( '69B125D7-99CC-42D6-A6FA-46687F333749', N'Винт ""потай"" 3х16',		3.98	),
                                    ( '94BC671A-A6B6-417A-BC9F-8AE4871A58EC', N'Дюбель 6х60',			5.50	),
                                    ( 'EFC6578A-00B7-4766-A7E3-79CDBA8C294B', N'Органайзер для шурупов',199		),
                                    ( '9654271B-AB52-4225-A30C-D75054B1733F', N'Лазерный дальномер',	1950	),
                                    ( 'F2585221-1ACA-4EFE-A5E8-C2F4534D1F92', N'Дрель электрическая',	990		),
                                    ( '4A550D3B-D1F2-40EF-AE4E-963612C6713A', N'Сварочный аппарат',		2099	),
                                    ( '17DB11D1-F50E-4CF4-9C54-CF1BD45802EA', N'Электроды 3мм',			49.98	),
                                    ( '7264D33A-16B9-4E22-B3F1-63D6DAE60078', N'Паяльник 40 Вт',		199.98	)";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ShowMonitorProducts();
            MessageBox.Show("Products Installed", "SQL success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InstallManagers_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand cmd = new() { Connection = _connection };
            cmd.CommandText = @"CREATE TABLE Managers (
	                            Id			UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	                            Surname		NVARCHAR(50) NOT NULL,
	                            Name		NVARCHAR(50) NOT NULL,
	                            Secname		NVARCHAR(50) NOT NULL,
	                            Id_main_dep UNIQUEIDENTIFIER NOT NULL REFERENCES Departments( Id ),
	                            Id_sec_dep	UNIQUEIDENTIFIER REFERENCES Departments( Id ),
	                            Id_chief	UNIQUEIDENTIFIER
                            )";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                cmd.CommandText = File.ReadAllText("sql/fill_managers.sql");
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IO error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            ShowMonitorManagers();
            MessageBox.Show("Managers Installed", "SQL success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DropDepartments_Click(object sender, RoutedEventArgs e)
        {
            using SqlCommand cmd = new("DROP TABLE Departments", _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Departments deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Delete Managers first");
            }
        }

        #endregion

        #region Скалярні запити - з одним результатом
        private void ShowMonitor()
        {
            ShowMonitorDepartments();
            ShowMonitorProducts();
            ShowMonitorManagers();
        }

        /// <summary>
        /// Відображає на моніторі кількість відділів (департаментів) у БД
        /// </summary>
        private void ShowMonitorDepartments()
        {
            using SqlCommand cmd = new("SELECT COUNT(*) FROM Departments", _connection);
            try
            {
                var res = cmd.ExecuteScalar();   // повертає "лівий-верхній" результат запиту
                // тип повернення - object, оскільки результат може бути довільного типу
                // для використання результат бажано конвертувати до очікуваного типу
                int cnt = Convert.ToInt32(res);
                StatusDepartments.Content = cnt.ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                StatusDepartments.Content = "--";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                StatusDepartments.Content = "--";
            }
        }

        private void ShowMonitorProducts()
        {
            using SqlCommand cmd = new("SELECT COUNT(*) FROM Products", _connection);
            try
            {
                var res = cmd.ExecuteScalar();   // повертає "лівий-верхній" результат запиту
                // тип повернення - object, оскільки результат може бути довільного типу
                // для використання результат бажано конвертувати до очікуваного типу
                int cnt = Convert.ToInt32(res);
                StatusProducts.Content = cnt.ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                StatusProducts.Content = "--";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                StatusProducts.Content = "--";
            }
        }

        private void ShowMonitorManagers()
        {
            using SqlCommand cmd = new("SELECT COUNT(*) FROM Managers", _connection);
            try
            {
                var res = cmd.ExecuteScalar();
                int cnt = Convert.ToInt32(res);
                StatusManagers.Content = cnt.ToString();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                StatusManagers.Content = "--";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);

                StatusManagers.Content = "--";
            }
        }

        #endregion

        #region Запити із результатами
        private void ShowDepartments()
        {
            using SqlCommand cmd = new("SELECT * FROM Departments", _connection);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                /* reader - інструмент для передачі даних від БД до програми.
                 * Принцип - передача по одному рядку (табличного результату)
                 * команда reader.Read() передає рядок. Дані залишаються у reader
                 * Для доступу до даних використовуються
                 * а) get-тери на кшталт reader.GetGuid(0);
                 * б) індексатори типу reader[1]
                 * індекси (0,1) - це порядкові індекси полів у запиті
                 * Наступний рядок - повторний виклик reader.Read()
                 */
                String str = "";
                while(reader.Read())
                {
                    str += reader.GetGuid(0) + " " + reader.GetString(1) + "\n";
                    // Завдання: вивести id скорочено у стилі a2fb...81
                }
                ViewDepartments.Text = str;
                reader.Close();   // незакритий reader блокує інші команди
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Query error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }            
        }
        /* Д.З. Реалізувати виведення даних таблиць Products та Managers
         * Забезпечити скорочене подання ідентифікаторів
         * Для співробітників (Managers) вивести 
         *  ід - Прізвище І.Б. - Назва робочого відділу
         * (інші дані не відображати)
         */
        #endregion
    }
}
