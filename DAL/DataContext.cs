using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_201.DAL
{
    internal class DataContext
    {
        private  SqlConnection _connection;
        internal DepartmentApi Departments { get; set; }
        internal ManagerApi    Managers    { get; set; }

        public DataContext() 
        { 
            _connection = new SqlConnection(App.ConnectionString);
            try
            {
                _connection.Open();
            }
            catch(Exception ex)
            {
                String msg =
                    DateTime.Now + ": " +
                    this.GetType().Name + "::" +
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name + " " +
                    ex.Message;

                App.Logger.Log(msg, "SEVERE");
                throw new Exception("Context creation failed. See server logs for details.");
            }
            Departments = new(_connection, this);
            Managers    = new(_connection, this);
        }
    }
}
/* Д.З. Реалізувати навігаційні властивості сутності Manager:
 * Chief - Manager, що є начальником
 * Subordinates - List<Manager> - підлеглі
 * Додати до вікна відомості про начальника та кількість підлеглих
 */