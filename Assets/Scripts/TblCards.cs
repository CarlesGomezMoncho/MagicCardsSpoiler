using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TblCards : DBHelper
{
    private const string Tag = "MSG: TblCards:\t";

    private const string TABLE_NAME = "Cards";

    private const string KEY_ID = "id";
    private const string KEY_NAME = "name";
    private const string KEY_IMAGE = "image";
    private const string KEY_ID_SET = "id_set"; 
    private const string KEY_TYPE = "type";
    private const string KEY_COLOR = "color";
    private const string KEY_RARITY = "rarity";
    private const string KEY_DATE = "date";

    //private string[] COLUMNS = new string[] { KEY_ID, KEY_NAME, KEY_IMAGE, KEY_ID_SET, KEY_TYPE, KEY_COLOR, KEY_RARITY, KEY_DATE };

    public TblCards() : base()
    {
        IDbCommand dbcmd = GetDbCommand();

        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
            KEY_ID + " INTEGER NOT NULL PRIMARY KEY UNIQUE, " +
            KEY_NAME + " TEXT NOT NULL, " +
            KEY_IMAGE + " TEXT NOT NULL, " +
            KEY_ID_SET + " INTEGER NOT NULL, " +
            KEY_TYPE + " TEXT NOT NULL, " +
            KEY_COLOR + " TEXT NOT NULL, " +
            KEY_RARITY + " TEXT NOT NULL, " +
            KEY_DATE + " TEXT NOT NULL" +
            ")";

        //Debug.Log(dbcmd.CommandText);
        dbcmd.ExecuteNonQuery();
    }

    public void AddCard(Card card)
    {
        IDbCommand dbcmd = GetDbCommand();

        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (" + KEY_ID + ", " + KEY_NAME + ", " + KEY_IMAGE + ", " + KEY_ID_SET + ", " + KEY_TYPE + ", " + KEY_COLOR+ ", " + KEY_RARITY + ", " + KEY_DATE +
            ") VALUES (@id,@name,@image,@id_set,@type,@color,@rarity,@date)";

        IDbDataParameter id = dbcmd.CreateParameter();
        id.ParameterName = "@id";
        id.Value = card.id;
        dbcmd.Parameters.Add(id);

        IDbDataParameter name = dbcmd.CreateParameter();
        name.ParameterName = "@name";
        name.Value = card.name;
        dbcmd.Parameters.Add(name);

        IDbDataParameter image = dbcmd.CreateParameter();
        image.ParameterName = "@image";
        image.Value = card.image;
        dbcmd.Parameters.Add(image);

        IDbDataParameter id_set = dbcmd.CreateParameter();
        id_set.ParameterName = "@id_set";
        id_set.Value = card.set;
        dbcmd.Parameters.Add(id_set);

        IDbDataParameter type = dbcmd.CreateParameter();
        type.ParameterName = "@type";
        type.Value = card.type;
        dbcmd.Parameters.Add(type);

        IDbDataParameter color = dbcmd.CreateParameter();
        color.ParameterName = "@color";
        color.Value = card.color;
        dbcmd.Parameters.Add(color);

        IDbDataParameter rarity = dbcmd.CreateParameter();
        rarity.ParameterName = "@rarity";
        rarity.Value = card.rarity;
        dbcmd.Parameters.Add(rarity);

        IDbDataParameter date = dbcmd.CreateParameter();
        date.ParameterName = "@date";
        date.Value = card.date;
        dbcmd.Parameters.Add(date);

        dbcmd.ExecuteNonQuery();
    }

    public List<Card> GetCardsList(string sqlQuery)
    {
        List<Card> cardsConsulta = new List<Card>();

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
            string image = fila[2].ToString();
            int id_set = int.Parse(fila[3].ToString());
            string type = fila[4].ToString();
            string color = fila[5].ToString();
            string rarity = fila[6].ToString();
            string date = fila[7].ToString();

            Card c = new Card(id, name, image, id_set, type, color, rarity, date);
            cardsConsulta.Add(c);
        }


        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;


        return cardsConsulta;
    }

    /// <summary>
    /// Retorna una llista de totes les cartes
    /// </summary>
    /// <returns></returns>
    public List<Card> GetCards()
    {
        return GetCardsList("SELECT * FROM " + TABLE_NAME);
    }

    /// <summary>
    /// Busca la cartat amb el id pasat per parametre
    /// </summary>
    /// <param name="id">id de la carta a buscar</param>
    /// <returns>la carta que se correspon amb el id pasat o null si no existeix</returns>
    public Card GetCard(int id)
    {
        string query = "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = " + id;
        //Debug.Log(query);

        List<Card> list = GetCardsList(query);

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
        List<Card> list = GetCardsList(query);

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
