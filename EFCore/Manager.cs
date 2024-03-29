﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_201.EFCore
{
    public class Manager
    {
        public Guid Id { get; set; }
        public String Surname { get; set; }
        public String Name { get; set; }
        public String Secname { get; set; }
        public Guid IdMainDep { get; set; }   // Guid - ValueType, вживається для полів з модифікатором NOT NULL
        public Guid? IdSecDep { get; set; }    // Якщо NULL не заборонений, то Guid не підходить, потрібен Guid?
        public Guid? IdChief { get; set; }     // Guid? - скорочення від Nullable<Guid>
        public DateTime? DeleteDt { get; set; }
    }
}
