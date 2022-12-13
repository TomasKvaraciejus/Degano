using SQLite;
using System.Diagnostics.CodeAnalysis;

namespace Degano.SqliteDb
{
    [ExcludeFromCodeCoverage]
    [Table("User")]
    public class UserResult
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    [ExcludeFromCodeCoverage]
    [Table("Card")]
    public class Cards
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int id { get; set; }
        public string Email { get; set; }
        public string CardName { get; set; }
        public int Discount { get; set; }
    }
}
