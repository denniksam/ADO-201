using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_201.Entity
{
    // ORM-сутність -- клас, що відображає таблицю Departments (рядок таблиці)
    public class Department
    {
        public Guid Id { get; set; }
        public String Name { get; set; }

        public override string ToString()
        {
            return Id.ToString()[..5] + "... " + Name;
        }
    }
}
