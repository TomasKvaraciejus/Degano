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
        public int Discount95 { get; set; }
        public int Discount98 { get; set; }
        public int DiscountLPG { get; set; }
        public int DiscountDiesel { get; set; }
    }
}
