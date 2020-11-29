using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public GameObject RankingGO;
    public GameObject RankPrefab;

    private string rutaDB;
    private string strConexion;
    private string DBFileName = "RankingDB.db";
    private List<Ranking> rankings = new List<Ranking>();

    private IDbConnection dbConnection;
    private IDbCommand dbCommand;
    private IDataReader reader;

    void Start()
    {
        //InsertarPuntos("Carl", 40);
        //BorrarPuntos(3);
        ObtenerRanking();
        ListarRanking();
    }

    void openDB()
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

                    while (!loadDB.isDone) { }

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

    void ObtenerRanking()
    {
        rankings.Clear();
        openDB();

        // Creo la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT * FROM Ranking ORDER BY Score DESC LIMIT 15";
        dbCommand.CommandText = sqlQuery;

        // Leo la DB (id, marca, color, cantidades)
        try
        {
            reader = dbCommand.ExecuteReader();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }

        while (reader.Read())
        {
            rankings.Add(new Ranking(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3)));
        }

        reader.Close();
        reader = null;
        closeDB();
    }

    void ListarRanking()
    {
        int position = 1;
        Rank RankComponents;
        GameObject NewRank;
        foreach(Ranking rank in rankings)
        {
            NewRank = Instantiate(RankPrefab, RankingGO.transform);

            RankComponents = NewRank.GetComponent<Rank>();
            RankComponents.PosicionGO.GetComponent<Text>().text = "#" + position.ToString();
            RankComponents.NombreGO.GetComponent<Text>().text = rank.Name;
            RankComponents.PuntosGO.GetComponent<Text>().text = rank.Score.ToString();
            position++;
        }
    }

    void InsertarPuntos(string n, int s)
    {
        openDB();

        // Creo la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("INSERT INTO Ranking(Name, Score) values(\"{0}\", \"{1}\")", n, s);
        dbCommand.CommandText = sqlQuery;

        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }

        closeDB();
    }

    void BorrarPuntos(int id)
    {
        openDB();

        // Creo la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "DELETE FROM Ranking WHERE PlayerId = \"" + id + "\"";
        dbCommand.CommandText = sqlQuery;

        try
        {
            dbCommand.ExecuteScalar();
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
        }

        closeDB();
    }

    void closeDB()
    {
        // Cierro conexiones
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }
}