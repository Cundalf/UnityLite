using System;

class Ranking
{
    public int id { get; set; }
    public string Name { get; set; }
    public int Score { get; set; }
    public string Date { get; set; } // no tengo la base como Date, trabajar con el formato yyyy-mm-dd hh:mm:ss

    public Ranking(int id, string name, int score, string date)
    {
        this.id = id;
        this.Name = name;
        this.Score = score;
        this.Date = date;
    }
}
