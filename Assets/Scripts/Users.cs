using Realms;
using MongoDB.Bson;

public partial class Users : IRealmObject
{
    [MapTo("_id")]
    [PrimaryKey]
    public ObjectId Id { get; set; }

    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }

    public Users()
    {
        Id = ObjectId.GenerateNewId();
    }
}
