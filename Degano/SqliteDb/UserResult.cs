using SQLite;

namespace Degano.SqliteDb
{
    [Table("User")]
    public class UserResult
    {
        [PrimaryKey]
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Table("Card")]
    public class Cards
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string Email { get; set; }
        public string CardName { get; set; }
        public int Discount { get; set; }
    }
}
