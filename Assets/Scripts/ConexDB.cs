using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class ConexDB : MonoBehaviour {

    string rutaDB;
    string strConexion;
    string DBFileName = "RopaDB.db";
    public Text MyText; // Para debug en Android

    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

	void Start () {
        abrirDB();
	}

    void abrirDB()
    {

        // Compruebo la plataforma
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor: // Windows
            case RuntimePlatform.OSXEditor: // MAC
                rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
                break;
            case RuntimePlatform.Android:
                rutaDB = Application.persistentDataPath + "/" + DBFileName;

                // Si no existe en persistant data la copio
                if (!File.Exists(rutaDB))
                {
                    WWW loadDB = new WWW("jar;file://" + Application.dataPath + "!/assets/" + DBFileName);

                    while (!loadDB.isDone)
                    {
                        // Luego
                    }

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

        // Creo la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT * FROM Camisas";
        dbCommand.CommandText = sqlQuery;

        // Leo la DB (id, marca, color, cantidades)
        try
        {
            reader = dbCommand.ExecuteReader();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            MyText.text = e.ToString();
        }

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string marca = reader.GetString(1);
            string color = reader.GetString(2);
            int cant = reader.GetInt32(3);

            Debug.Log(id + " - " + marca + " - " + color + " - " + cant);
            MyText.text = id.ToString() + " - " + marca + " - " + color + " - " + cant.ToString();
        }

        // Cierro conexiones
        reader.Close();
        reader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }
}
