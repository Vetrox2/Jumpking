using System.Collections.Generic;
using System.Threading.Tasks;
using Realms;
using Realms.Sync;
using UnityEngine;
using Realms.Logging;
using Logger = Realms.Logging.Logger;
using Realms.Sync.ErrorHandling;
using Realms.Sync.Exceptions;
using System.Linq;
using System;

public class RealmController
{
    private Realm realm;
    private readonly string myRealmAppId = "application-0-lvuzw";
    //private readonly string apiKey = "XXXXXXXXXXXXXXXXXXXXXXX";

    public RealmController()
    {
        InitAsync();
    }

    private async void InitAsync()
    {
        var app = App.Create(myRealmAppId);
        User user = await Get_userAsync(app);
        FlexibleSyncConfiguration config = GetConfig(user);
        realm = Realm.GetInstance(config);
        realm.Subscriptions.Update(() =>
        {
            var myScores = realm.All<Highscore>();
            realm.Subscriptions.Add(myScores);
        });
        await realm.Subscriptions.WaitForSynchronizationAsync();
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
            var currentHighscore = realm.All<Highscore>().OrderBy(x=>x.Time).ToList();
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
                realm.Add(new Highscore {Player=name, Time= Math.Round(time,2)});
            });
            
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}