using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Degano
{
    public static class ExceptionLogger
    {
        public static void Log(string error)
        {
            // we need to output this to the database
            System.Diagnostics.Debug.WriteLine("err: " + error + $"\ntimestamp: {DateTime.Now}\n");
        }
    }
}
