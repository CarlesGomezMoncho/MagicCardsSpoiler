using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TblSets : DBHelper
{
    private const string Tag = "MSG: TblSets:\t";

    private const string TABLE_NAME = "Sets";

    private const string KEY_ID = "id";
    private const string KEY_NAME = "name";
    private const string KEY_SHORTNAME = "shortname";
    private const string KEY_DATE = "date";

    //private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_SHORTNAME, KEY_DATE };

    public TblSets () :base()
    {
        IDbCommand dbcmd = GetDbCommand();

        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
            KEY_ID + " INTEGER NOT NULL PRIMARY KEY UNIQUE, " +
            KEY_NAME + " TEXT NOT NULL, " +
            KEY_SHORTNAME + " TEXT NOT NULL, " +
            KEY_DATE + " TEXT NOT NULL" +
            ")";

        dbcmd.ExecuteNonQuery();
    }

    public void AddSet(Set set)
    {
        IDbCommand dbcmd = GetDbCommand();

        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME+ " (" + KEY_ID + ", " + KEY_NAME + ", " + KEY_SHORTNAME + ", " + KEY_DATE + ") VALUES (@id,@name,@shortname,@date)";

        IDbDataParameter id = dbcmd.CreateParameter();
        id.ParameterName = "@id";
        id.Value = set.id;
        dbcmd.Parameters.Add(id);

        IDbDataParameter name = dbcmd.CreateParameter();
        name.ParameterName = "@name";
        name.Value = set.name;
        dbcmd.Parameters.Add(name);

        IDbDataParameter image = dbcmd.CreateParameter();
        image.ParameterName = "@shortname";
        image.Value = set.shortname;
        dbcmd.Parameters.Add(image);

        IDbDataParameter date = dbcmd.CreateParameter();
        date.ParameterName = "@date";
        date.Value = set.date;
        dbcmd.Parameters.Add(date);

        dbcmd.ExecuteNonQuery();
    }

    public List<Set> GetSetsList(string sqlQuery)
    {
        List<Set> setsConsulta = new List<Set>();

        IDbCommand dbcmd = GetDbCommand();
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            ArrayList fila = new ArrayList();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                fila.Add(reader.GetValue(i));
            }

            int id = int.Parse(fila[0].ToString());
            string name = fila[1].ToString();
            string shortname = fila[2].ToString();
            string date = fila[3].ToString();

            Set s = new Set(id, name, shortname, date);
            setsConsulta.Add(s);
        }


        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;


        return setsConsulta;
    }

    /// <summary>
    /// Retorna una llista de tots els sets
    /// </summary>
    /// <returns></returns>
    public List<Set> GetSets()
    {
        return GetSetsList("SELECT * FROM " + TABLE_NAME);
    }

    /// <summary>
    /// Busca el set amb el id pasat per parametre
    /// </summary>
    /// <param name="id">id de set a buscar</param>
    /// <returns>el set que se correspon amb el id pasat o null si no existeix</returns>
    public Set GetSet(int id)
    {
        string query = "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + id;
        //Debug.Log(query);

        List<Set> list = GetSetsList(query);

        if (list.Count == 0)
        {
            return null;
        }
        else
        {
            return list[0];
        }
    }

    public int GetLastID()
    {
        string query = "SELECT * FROM " + TABLE_NAME + " ORDER BY " + KEY_ID + " DESC ";

        //agafem una llista de sets ordenats inversament per id
        List<Set> list = GetSetsList(query);

        //retornem el id del primer set que serà el que te el nº mes alt
        return list[0].id;
    }

    public override void DeleteAllData()
    {
        Debug.Log(Tag + "Deleting Table");

        base.DeleteAllData(TABLE_NAME);
    }

    ////////////////////////////////////////////////////////////////////////////
    //posibles a borrar o modificar tot lo de baix
    ////////////////////////////////////////////////////////////////////////

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

    public override IDataReader GetAllData()
    {
        return base.GetAllData(TABLE_NAME);
    }

}
