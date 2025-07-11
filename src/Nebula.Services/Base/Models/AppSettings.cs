﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nebula.Services.Base.Models
{
    public class AppSettings
    {
        public string DataStore { get; set; } = "/data";
        public string MySQLConn { get; set; } = "server=127.0.0.1;database=tmpdata;user=root;password=password";
    }
}