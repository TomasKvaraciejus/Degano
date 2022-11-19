using FireSharp;
using FireSharp.Config;
using FireSharp.Extensions;
using FireSharp.Interfaces;
using FireSharp.Response;
using Java.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Degano
{
    public class Ex
    {
        public DateTime date { get; set; }
        public string exMessage { get; set; }

        public Ex(DateTime date, string exMessage)
        {
            this.date = date;
            this.exMessage = exMessage;
        }
    }
    public static class ExceptionLogger
    {
        public static async void Log(string error)
        {
            System.Diagnostics.Debug.WriteLine("Exception thrown: " + error + $"\ntimestamp: {DateTime.Now}\n");

            Ex ex = new Ex(DateTime.Now, error);

            IFirebaseConfig config = new FirebaseConfig
            {
                BasePath = "https://degano-70426-default-rtdb.europe-west1.firebasedatabase.app/"
            };
            IFirebaseClient client = new FirebaseClient(config);
            FirebaseResponse response = await client.SetAsync("Exceptions/", ex.ToJson);
        }
    }
}
