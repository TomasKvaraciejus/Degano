﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Degano
{
    public class GasStation :
#if ANDROID
        Java.Lang.Object, // Necessary for casting GasStation to java object in MapHandler.Android.cs
#endif
    IComparable
    {
        public string name, address, type; // type variable denotes gas station company (e.g. "Viada", "Circle-K"), whereas name can store entire name of gas station (i.e. "Viada pilaite")
        public Location location;
        public double price95, price98, priceDiesel, priceLPG;

        public GasStation(string _name, string _address, Location _location, double _price95, double _price98, double _priceDiesel, double _priceLPG, string _type = "other")
        {
            name = _name;
            address = _address;
            location = _location;
            price95 = _price95;
            price98 = _price98;
            priceDiesel = _priceDiesel;
            priceLPG = _priceLPG;
        }

        int IComparable.CompareTo(object? obj) // This function is only preliminary
        {
            var _gasStation = (GasStation)obj;

            if (_gasStation == null)
                return 1;
            else
                return this.price95.CompareTo(_gasStation.price95);

        }
    }
}
