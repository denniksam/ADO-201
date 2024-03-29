﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace ADO_201.Service
{
    internal class Logger
    {
        private readonly String filename;
        public Logger(String filename) 
        {
            this.filename = Path.Combine(
                AppContext.BaseDirectory.Substring(0,
                    AppContext.BaseDirectory.IndexOf("bin")),
                filename);
        }
        public void Log(String message, String level = "INFO")
        {
            File.AppendAllText(filename, $"{level} - {message}\r\n");
        }
    }
}
