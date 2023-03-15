using ADO_201.Entity;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ProductCrudWindow.xaml
    /// </summary>
    public partial class ProductCrudWindow : Window
    {
        public Entity.Product Product { get; set; }

        public ProductCrudWindow()
        {
            InitializeComponent();
            Product = null!;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Product is null)  // режим додавання (Create)
            {
                DeleteButton.IsEnabled = false;
                Product = new Product { Id = Guid.NewGuid() };
            }
            else  // режими редагування чи видалення (U / D)
            {
                NameView.Text = Product.Name;
                PriceView.Text = Product.Price.ToString();
                DeleteButton.IsEnabled = true;
            }
            IdView.Text = Product.Id.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameView.Text.Equals(String.Empty))
            {
                MessageBox.Show("Введіть назву товара");
                NameView.Focus();
                return;
            }
            try
            {
                Product.Price = Convert.ToDouble(       // Варіант конвертора із 
                    PriceView.Text.Replace(',', '.'),   // зазначенням культури
                    CultureInfo.InvariantCulture        // у даному разі сприймається точка
                    );                                  // замість коми
            }
            catch
            {
                MessageBox.Show("Неправильний формат числа для ціни");
                PriceView.Focus();
                return;
            }
            Product.Name = NameView.Text;
            this.DialogResult = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Product = null!;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;  // те, що поверне ShowDialog
        }
    }
}
