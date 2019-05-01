using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationEntity
{

    public string _id;
    public string _type;
    public string _Lat;
    public string _Lng;
    public string _dateCreated;

    public LocationEntity(string id, string type, string Lat, string Lng)
    {
        _id = id;
        _type = type;
        _Lat = Lat;
        _Lng = Lng;
        _dateCreated = "";
    }

    public LocationEntity(string id, string type, string Lat, string Lng, string dateCreated)
    {
        _id = id;
        _type = type;
        _Lat = Lat;
        _Lng = Lng;
        _dateCreated = dateCreated;
    }

    public static LocationEntity getFakeLocation()
    {
        return new LocationEntity("0", "Test_Type", "0.0", "0.0");
    }
}

