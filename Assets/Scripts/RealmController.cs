using System.Collections.Generic;
using System.Threading.Tasks;
using Realms;
using Realms.Sync;
using UnityEngine;
using Realms.Sync.ErrorHandling;
using Realms.Sync.Exceptions;
using System.Linq;
using System;
using Konscious.Security.Cryptography;
using MongoDB.Bson.IO;
using System.Text;

public class RealmController<T> where T : IRealmObject
{
    private Realm realm;
    private readonly string myRealmAppId = "application-0-lvuzw";
    public event Action<RealmController<T>> RealmLoaded;

    public bool SignUp(string login, string password)
    {
        if (realm.All<Users>().Any(Users => Users.Login == login))
            return false;

        realm.Write(() =>
        {
            realm.Add(new Users { Login = login, Password = HashPassword(password) });
        });
        return true;
    }
    public bool SignIn(string login, string password) => realm.All<Users>().Any(Users => Users.Login == login && Users.Password == HashPassword(password)); 
   
    public async void InitAsync()
    {
        var app = App.Create(myRealmAppId);
        User user = await Get_userAsync(app);
        FlexibleSyncConfiguration config = GetConfig(user);
        realm = await Realm.GetInstanceAsync(config);
        realm.Subscriptions.Update(() =>
        {
            var myTable = realm.All<T>();
            realm.Subscriptions.Add(myTable);
        });
        await realm.Subscriptions.WaitForSynchronizationAsync();
        RealmLoaded?.Invoke(this);
    }

    private FlexibleSyncConfiguration GetConfig(User user)
    {
        FlexibleSyncConfiguration config = new(user);

        config.ClientResetHandler = new DiscardUnsyncedChangesHandler
        {
            ManualResetFallback = (ClientResetException clientResetException) => clientResetException.InitiateClientReset()
        };
        return config;
    }

    private async Task<User> Get_userAsync(App app)
    {
        User user = app.CurrentUser;
        if (user == null)
        {
            user = await app.LogInAsync(Credentials.Anonymous());
        }
        return user;
    }

    public void Terminate()
    {
        realm?.Dispose();
    }

    public List<Highscore> GetHighscore()
    {
        try
        {
            var currentHighscore = realm.All<Highscore>().OrderBy(x => x.Time).ToList();
            return currentHighscore;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return null;
    }

    public void SendHighscore(string name, float time)
    {
        try
        {
            realm.Write(() =>
            {
                realm.Add(new Highscore { Player = name, Time = Math.Round(time, 2) });
            });

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    string HashPassword(string password)
    {
        var argon = new Argon2i(Encoding.UTF8.GetBytes(password));
        argon.DegreeOfParallelism = 16;
        argon.MemorySize = 8192;
        argon.Salt = Encoding.UTF8.GetBytes("542fsewrf23f");
        argon.Iterations = 10;
        var hash = argon.GetBytes(128).ToList();
        password = string.Empty;
        hash.ForEach(x => password += x.ToString());
        return password;
    }
}