using System.Data;
using UnityEngine;

public class LocationDb : DBHelper
{
    private const string Tag = "Riz: LocationDb:\t";

    private const string TABLE_NAME = "Locations";
    private const string KEY_ID = "id";
    private const string KEY_TYPE = "type";
    private const string KEY_LAT = "Lat";
    private const string KEY_LNG = "Lng";
    private const string KEY_DATE = "date";
    private string[] COLUMNS = new string[] { KEY_ID, KEY_TYPE, KEY_LAT, KEY_LNG, KEY_DATE };

    public LocationDb() : base()
    {
        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
            KEY_ID + " TEXT PRIMARY KEY, " +
            KEY_TYPE + " TEXT, " +
            KEY_LAT + " TEXT, " +
            KEY_LNG + " TEXT, " +
            KEY_DATE + " DATETIME DEFAULT CURRENT_TIMESTAMP )";
        dbcmd.ExecuteNonQuery();
    }

    public void addData(LocationEntity location)
    {
        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText =
            "INSERT INTO " + TABLE_NAME
            + " ( "
            + KEY_ID + ", "
            + KEY_TYPE + ", "
            + KEY_LAT + ", "
            + KEY_LNG + " ) "

            + "VALUES ( '"
            + location._id + "', '"
            + location._type + "', '"
            + location._Lat + "', '"
            + location._Lng + "' )";
        dbcmd.ExecuteNonQuery();
    }

    public override IDataReader GetDataById(int id)
    {
        return base.GetDataById(id);
    }

    public override IDataReader GetDataByString(string str)
    {
        Debug.Log(Tag + "Getting Location: " + str);

        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText =
            "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + str + "'";
        return dbcmd.ExecuteReader();
    }

    public override void DeleteDataByString(string id)
    {
        Debug.Log(Tag + "Deleting Location: " + id);

        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText =
            "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
        dbcmd.ExecuteNonQuery();
    }

    public override void DeleteDataById(int id)
    {
        base.DeleteDataById(id);
    }

    public override void DeleteAllData()
    {
        Debug.Log(Tag + "Deleting Table");

        base.DeleteAllData(TABLE_NAME);
    }

    public override IDataReader GetAllData()
    {
        return base.GetAllData(TABLE_NAME);
    }

    public IDataReader getNearestLocation(LocationInfo loc)
    {
        Debug.Log(Tag + "Getting nearest centoid from: "
            + loc.latitude + ", " + loc.longitude);
        IDbCommand dbcmd = GetDbCommand();

        string query =
            "SELECT * FROM "
            + TABLE_NAME
            + " ORDER BY ABS(" + KEY_LAT + " - " + loc.latitude
            + ") + ABS(" + KEY_LNG + " - " + loc.longitude + ") ASC LIMIT 1";

        dbcmd.CommandText = query;
        return dbcmd.ExecuteReader();
    }

    public IDataReader getLatestTimeStamp()
    {
        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText =
            "SELECT * FROM " + TABLE_NAME + " ORDER BY " + KEY_DATE + " DESC LIMIT 1";
        return dbcmd.ExecuteReader();
    }
}