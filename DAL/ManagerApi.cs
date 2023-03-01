using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADO_201.DAL
{
    internal class ManagerApi
    {
        private readonly SqlConnection _connection;
        private readonly DataContext _dataContext;
        private List<Entity.Manager> list;
        public ManagerApi(SqlConnection connection, DataContext dataContext)
        {
            _connection = connection;
            _dataContext = dataContext;
            list = null!;
        }
        public List<Entity.Manager> GetAll(bool includeDeleted = false)
        {
            if (list is not null) { return list; }

            list = new();
            try
            {
                string query = "SELECT * FROM Managers d";
                if (!includeDeleted) query += " WHERE d.DeleteDt IS NULL";
                using SqlCommand cmd = new(query, _connection);
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
