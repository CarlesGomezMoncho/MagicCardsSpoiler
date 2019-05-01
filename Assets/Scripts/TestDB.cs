﻿using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class TestDB : MonoBehaviour
{
    void Start()
    {
        //Obrim (i creem) BD
        string conn = "URI=file:" + Application.persistentDataPath + "/DB";
        IDbConnection dbcon = new SqliteConnection(conn);
        dbcon.Open();

        //creem una taula
        IDbCommand dbcmd;

        dbcmd = dbcon.CreateCommand();
        string q_createTable = "CREATE TABLE IF NOT EXISTS my_table (id INTEGER PRIMARY KEY, val INTEGER )";

        dbcmd.CommandText = q_createTable;
        dbcmd.ExecuteReader();

        //afegim una fila
        IDbCommand cmnd = dbcon.CreateCommand();
        cmnd.CommandText = "INSERT INTO my_table (id, val) VALUES (0, 5)";
        cmnd.ExecuteNonQuery();

        //llegim la taula
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM my_table";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log("id: " + reader[0].ToString());
            Debug.Log("val: " + reader[1].ToString());
        }

        //tanquem conexió
        dbcon.Close();
    }


}
