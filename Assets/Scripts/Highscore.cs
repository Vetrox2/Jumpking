using System;
using System.Collections.Generic;
using Realms;
using MongoDB.Bson;
public partial class Highscore : IRealmObject
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId Id { get; set; }
    [MapTo("player")]
    [Required]
    public string Player { get; set; }
    [MapTo("time")]
    public double Time { get; set; }



    public Highscore()
    {
        Id = ObjectId.GenerateNewId();
    }

}