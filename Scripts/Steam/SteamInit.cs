using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SteamInit : MonoBehaviour
{
    public Image icon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(252490);
            Debug.Log(Steamworks.SteamClient.SteamId);
        }
        catch (System.Exception e)
        {
            Debug.Log("abierto");
            // Something went wrong - it's one of these:
            //
            //     Steam is closed?
            //     Can't find steam_api dll?
            //     Don't have permission to play app?
            //
        }
    }

    // Update is called once per frame
    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }

    public async void setAvatar()
    {
        var avatar = GetAvatar();
    }

    public async Task<Steamworks.Data.Image?> GetAvatar()
    {
        try
        {
            return await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }
}