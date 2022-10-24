namespace Degano
{
    public class DatabaseEntry
    {
        public string address, brand, diesel, lpg, lat, lng, name, petrol95, petrol98;

        public DatabaseEntry(string _address, string _brand, string _diesel, string _lpg, string _lat, string _lng, string _name, string _petrol95, string _petrol98)
        {
            address = _address;
            brand = _brand;
            diesel = _diesel;
            lpg = _lpg;
            lat = _lat;
            lng = _lng;
            name = _name;
            petrol95 = _petrol95;
            petrol98 = _petrol98;
        }
    }
}
