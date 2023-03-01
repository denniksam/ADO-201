using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADO_201.DAL
{
    internal class DepartmentApi
    {
        private readonly SqlConnection _connection;
        private readonly DataContext _dataContext;

        private List<Entity.Department> list;

        public DepartmentApi(SqlConnection connection, DataContext dataContext)
        {
            _connection = connection;
            list = null!;
            _dataContext = dataContext;
        }

        public bool Add(Entity.Department department)
        {
            try
            {
                using SqlCommand cmd = new() { Connection = _connection };
                cmd.CommandText = $"INSERT INTO Departments (Id, Name) VALUES(@id, @name)";
                cmd.Parameters.AddWithValue("@id", department.Id);
                cmd.Parameters.AddWithValue("@name", department.Name);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                String msg =
                    DateTime.Now + ": " +
                    this.GetType().Name + "::" +
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name + " " +
                    ex.Message;

                App.Logger.Log(msg, "SEVERE");
                return false;
            }
        }

        /// <summary>
        /// Returns list od DB table Departments
        /// </summary>
        /// <param name="reload">Send new query or use cached data</param>
        /// <returns></returns>
        public List<Entity.Department> GetAll(bool reload = false)
        {
            if(list != null && !reload) return list;

            list = new List<Entity.Department>();
            try
            {
                using SqlCommand cmd = new("SELECT * FROM Departments", _connection);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new(reader) { dataContext = _dataContext } );
                }
            }
            catch (Exception ex)
            {
                String msg = 
                    DateTime.Now + ": " +
                    this.GetType().Name + "::" +
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name + " " +
                    ex.Message;

                App.Logger.Log(msg, "SEVERE");                
            }
            return list;
        }
    }
}
