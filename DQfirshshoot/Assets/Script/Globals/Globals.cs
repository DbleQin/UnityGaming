using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Globals : Singleton<Globals>
{
    /// <summary>
    /// EnterGameStarter
    /// </summary>
    public Starter MStarter = null;
    public PlayerData MPlayerData = null;

    // Globals Class
    public Globals()
    {

    }

    public void Awake()
    {
        GameObject go = GameObject.Find("GlobalScripts");
        if (go != null)
        {
            MStarter = go.GetComponent<Starter>();
            if (MStarter == null) UnityEngine.Debug.Log("global script doesn't have MStarter");

            MPlayerData = new PlayerData();
        }
    }

    public void Initialize()
    {
        //load first scenes
        UI_Manager.Instance.CreatePanel<UI_Main>(delegate(UI_Main pmgui)
        {
            UnityEngine.Debug.Log("2014/9/5 created by DQ");
        });
    }

    public void Release()
    {
        System.GC.Collect();
    }

    public void GameInsideToServerList()
    {
        Restart();
    }

    public void Relogin()
    {
        Restart();
    }

    public void Restart()
    {
        UnityEngine.Debug.Log("[Globals]: Restart begin...");
        UnityEngine.Debug.Log("[Globals]: Restart call Release()...");
        Globals.Instance.Release();
        GameObject dontAutoDelObj = GameObject.Find("GlobalScripts");
        GameObject root = GameObject.Find("UI_ROOT");
        if (null != dontAutoDelObj)
        {
            GameObject.Destroy(dontAutoDelObj);
            dontAutoDelObj = null;
            Resources.UnloadUnusedAssets();
        }
        if (null != root)
        {
            GameObject.Destroy(root);
            root = null;
            Resources.UnloadUnusedAssets();
        }

        GameObject globalSoundObj = GameObject.Find("GlobalSoundObject");
        if (null != globalSoundObj)
        {
            GameObject.Destroy(globalSoundObj);
            globalSoundObj = null;
            Resources.UnloadUnusedAssets();
        }

        UnityEngine.Debug.Log("[Globals]: Restart call Application.LoadLevel...");
        Application.LoadLevel("GameStarter");
        UnityEngine.Debug.Log("[Globals]: Restart End...");
    }


    public void QuitGame()
    {
        Application.Quit();
        // Test the Process state
        if (!Application.isEditor)
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (currentProcess != null)
            {
                currentProcess.Kill();
            }
        }
    }
}
