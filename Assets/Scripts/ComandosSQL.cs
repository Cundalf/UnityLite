using System;
using UnityEngine;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;


public class ComandosSQL : MonoBehaviour {
    string rutaDB;
    string strConexion;
    string DBFileName = "chinook.db";

    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

    void Start()
    {
        abrirDB();
        //ComandoSelect("*", "albums");
        //ComandoWHERE("*", "albums", "AlbumId", "=", "33");
        ComandoWHERE_AND("CustomerId", "FirstName", "LastName", "Country", "customers", "Country", "=", "Brazil", "CustomerId", ">", "10");
        cerrarDB();
    }

    void abrirDB()
    {
        // Compruebo la plataforma
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
                rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
                break;

            case RuntimePlatform.Android:
                rutaDB = Application.persistentDataPath + "/" + DBFileName;

                // Si no existe en persistant data la copio
                if (!File.Exists(rutaDB))
                {
                    WWW loadDB = new WWW("jar;file://" + Application.dataPath + "!/assets/" + DBFileName);

                    while (!loadDB.isDone){}

                    File.WriteAllBytes(rutaDB, loadDB.bytes);
                }

                break;

            case RuntimePlatform.IPhonePlayer:
                rutaDB = Application.dataPath + "/Raw/" + DBFileName;
                break;
        }

        // Creo y abro la conexion
        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
    }

    void cerrarDB()
    {
        // Cierro conexiones
        reader.Close();
        reader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    void ComandoSelect(string item, string table)
    {
        // Creo la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT " + item + " FROM " + table;
        dbCommand.CommandText = sqlQuery;

        reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            try
            {
                Debug.Log(reader.GetInt32(0) + " - " + reader.GetString(1) + " - " + reader.GetInt32(2));
            }
            catch (FormatException fe)
            {
                Debug.Log(fe.Message);
                continue;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                continue;
            }
        }
    }

    void ComandoWHERE(string item, string table, string campo, string comparador, string dato)
    {
        // Creo la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT " + item + " FROM " + table + " WHERE " + campo + " " + comparador + " " + dato;
        dbCommand.CommandText = sqlQuery;

        reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            try
            {
                Debug.Log(reader.GetInt32(0) + " - " + reader.GetString(1) + " - " + reader.GetInt32(2));
            }
            catch (FormatException fe)
            {
                Debug.Log(fe.Message);
                continue;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                continue;
            }
        }
    }

    void ComandoWHERE_AND(string item1, string item2, string item3, string item4,
                            string table, 
                            string campo1, string comparador1, string dato1,
                            string campo2, string comparador2, string dato2)
    {
        // Creo la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT " + item1 + "," + item2 + "," + item3 + "," + item4 +
                          " FROM " + table + 
                          " WHERE " + campo1 + " " + comparador1 + " '" + dato1 + "'" +
                          " AND " + campo2 + " " + comparador2 + " '" + dato2 + "'";
        dbCommand.CommandText = sqlQuery;

        reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            try
            {
                Debug.Log(reader.GetInt32(0) + " - " + reader.GetString(1) + " - " + reader.GetString(2) + reader.GetString(3));
            }
            catch (FormatException fe)
            {
                Debug.Log(fe.Message);
                continue;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                continue;
            }
        }
    }
}
