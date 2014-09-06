using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

/// <summary>
/// Starter.
/// </summary>
public class Starter : MonoBehaviour
{
    // Use this for initialization
    protected void Awake()
    {
        Globals.Instance.Awake();
    }
    // Use this for initialization
    protected void Start()
    {
        Globals.Instance.Initialize();
    }

    /// <summary>
    /// Lows the performence prompt update.
    /// </summary>
    void OnApplicationQuit()
    {
        Globals.Instance.QuitGame();
    }

    void OnApplicationPause(bool pause)
    {
    }

    void OnApplicationFocus(bool focus)
    {
    }
}
