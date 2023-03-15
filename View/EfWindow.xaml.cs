using ADO_201.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADO_201.View
{
    /// <summary>
    /// Interaction logic for EfWindow.xaml
    /// </summary>
    public partial class EfWindow : Window
    {
        public EfContext efContext { get; set; } = new();
        private ICollectionView depListView;   // інтерфейс для налагодження View з колекцією
        private static readonly Random random = new();

        public EfWindow()
        {
            InitializeComponent();
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            efContext.Departments.Load();
            depList.ItemsSource = efContext.Departments.Local.ToObservableCollection();
            // отримання посилання на depList, але як інтерфейс ICollectionView
            depListView = CollectionViewSource.GetDefaultView(depList.ItemsSource);
            depListView.Filter =   // Predicate<object>
                obj => (obj as Department)?.DeleteDt == null;  // TODO: замінити на DepartmentsDeletedFilter

            UpdateMonitor();
            UpdateDailyStatistics();
        }
        private void UpdateMonitor()
        {
            MonitorBlock.Text = "Departments: " + efContext.Departments.Count();
            MonitorBlock.Text += "\nProducts: " + efContext.Products.Count();
            MonitorBlock.Text += "\nManagers: " + efContext.Managers.Count();
            MonitorBlock.Text += "\nSales: " + efContext.Sales.Count();            
        }
        private void UpdateDailyStatistics()
        {
            // загальна кількість чеків (записів Sales) за сьогодні
            var todaySales = efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today);
            // виконання запиту на даному етапі немає, тільки План

            SalesChecks.Content = todaySales.Count().ToString();  // .Count() - запускає запит



            // загальна кількість проданих товарів (сума Sales.Quantity) за сьогодні
            SalesPcs.Content = "0";
            // найкращий чек за кількістю (Sales.Quantity)
            BestPcs.Content = "0";
            // момент початку продажів за сьогодні (мін час)
            StartMoment.Content = "0";
            // момент закінчення продажів (за сьогодні)
            FinishMoment.Content = "0";
            // "середній чек" - середня кількість товарів, що продається у чеку (за сьогодні)
            AvgPcs.Content = "0";

            ///////////////////////////////////////////////////////////////////////////
            

            var query3 = efContext.Products
                .GroupJoin(
                    efContext.Sales.Where(s => s.SaleDt.Date == DateTime.Today),
                    p => p.Id,
                    s => s.ProductId,
                    (p, sales) => new { 
                        Name = p.Name, 
                        Cnt = sales.Count() 
                    }
                ).OrderByDescending(g => g.Cnt);

            foreach (var item in query3)
            {
                LogBlock.Text += $"{item.Name} -- {item.Cnt}\n";
            }
            BestProduct.Content = query3.First().Name;
            /* Д.З. Написати запити для визначення кращого товару
             * а) за кількістю чеків (класна робота)
             * б) за кількістю проданих шт
             * в) за сумою продажів 
             * Разом з назвою вивести також числову хар-ку (шт/грн)
             */
        }
        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            DepartmentCrudWindow dialog = new();
            if (dialog.ShowDialog() == true)
            {
                // dialog.Department - інша сутність, треба змінити під EF
                efContext.Departments.Add(
                    new Department()
                    {
                        Name = dialog.Department.Name,
                        Id = dialog.Department.Id
                    });
                // !! додавання даних до контексту не додає їх до БД - планування додавання
                efContext.SaveChanges();  // внесення змін до БД

                MonitorBlock.Text += "\nDepartments: " + efContext.Departments.Count();
            }
        }
        private bool DepartmentsDeletedFilter(object item)
        {
            if (item is Department department)
            {
                return department.DeleteDt == null;
            }
            return false;
        }
        private void ShowAllDepsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            depListView.Filter = DepartmentsDeletedFilter;
            ((GridView)depList.View).Columns[2].Width = 0;
        }
        private void ShowAllDepsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            depListView.Filter = null; // скидаємо фільтр - показує усі дані

            ((GridView)depList.View)   // Властивості Visible для колонок ListView немає, тому
                .Columns[2]            // приховування/відображення - через встановлення Width
                .Width = Double.NaN;   // Double.NaN - автоматичне визначення
        }
        private void GenerateSalesButton_Click(object sender, RoutedEventArgs e)
        {
            // Випадковий менеджер з наявних
            // Manager manager =  // "-" отримання .ToList() передає всі дані, для BigData неприйнятно
            //    efContext.Managers.ToList()[random.Next(efContext.Managers.Count())];
            // Manager manager = // System.InvalidOperationException - LINQ-to-Entity перекладає запит на SQL. Не всі 
            //    efContext.Managers.ElementAt(random.Next(efContext.Managers.Count())); // можливості мови C# мають аналоги у SQL
            // DbSet приймає методи розширення LINQ, але не всі вони врешті спрацьовують, оскільки
            //    це LINQ-to-Entity (LINQ-to-SQL), що накладає певні обмеження

            double maxPrice = efContext.Products.Max(p => p.Price);
            int manCnt = efContext.Managers.Count();
            int proCnt = efContext.Products.Count();

            for (int i = 0; i < 100; i++)
            {
                Manager manager = efContext.Managers.Skip(random.Next(manCnt)).First();
                // Випадковий товар
                Product product = efContext.Products.Skip(random.Next(proCnt)).First();
                // Випадкова кількість, але чим дорожче товар, тим менша гранична кількість
                int quantity = random.Next(1,
                    (int)(20 * (1 - product.Price / maxPrice) + 2));

                efContext.Sales.Add(new()
                {
                    Id = Guid.NewGuid(),
                    ManagerId = manager.Id,
                    ProductId = product.Id,
                    Quantity = quantity,
                    SaleDt = DateTime.Today.AddSeconds(random.Next(0, 86400))  // Дата "сьогодні" але з випадковим часом
                });
            }
            efContext.SaveChanges();
            UpdateMonitor();
            UpdateDailyStatistics();
        }

    }
}
/* Реалізувати видалення департаментів через діалог (CRUD), переконатись, що після
 * видалення вони зникають з переліку. (!видалення - встановлення дати видалення)
 */
/* Т.З. Реалізувати:
 * - Відділи на старті вікна показуються тільки невидалені
 * - Є чекбокс "показувати видалені", його відмітка має додати
 *   = видалені відділи
 *   = колонку з датою видалення
 *   
 * Аналіз
 * а) можно фільтрувати контекст
 * depList.ItemsSource = efContext.Departments.Local.ToObservableCollection()
 *  .Where(...) - це зруйнує зворотний зв'язок - додавання нових елементів не буде
 *  оновлювати перелік
 * б) вжити заходів фільтрації на рівні View (ListView) - обираємо
 */