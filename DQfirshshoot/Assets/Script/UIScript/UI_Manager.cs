using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UI_Manager : MonoBehaviour
{
    //public UILabel infolbl;
    public static UI_Manager Instance;

    private float S_LastClickTime = 0;
    void Awake() { Instance = this; }
    void OnDestroy()
    {
        Instance = null;
    }
    void OnApplicationQuit()
    {


    }

    #region Const Variable
    public static Vector3 FontSizeSmall = new Vector3(18, 18, 1);
    #endregion
    void Start()
    {
        uicamera = NGUITools.FindCameraForLayer(gameObject.layer).GetComponent<UICamera>();//
        UIRoot[] roots = NGUITools.FindActive<UIRoot>();

        for (int i = 0, imax = roots.Length; i < imax; ++i)
        {
            UIRoot root = roots[i];
            DontDestroyOnLoad(root.gameObject);
        }
        UnityEngine.Random.seed = (int)System.DateTime.Now.Ticks;
        float height = Screen.height > Screen.width ? Screen.width : Screen.height;
        float manualHeight = Mathf.Max(2, height);
        float size = 2f / manualHeight;
        RootScale = new Vector3(size, size, size);
    }
    void FixedUpdate()
    {
        float Diff = Time.realtimeSinceStartup - (float)lastInterval;
        if (Diff > 1f)
        {

            int integerDiff = Mathf.FloorToInt(Diff);
            //float floatDiff = Diff - integerDiff;
            ServerStampTime += integerDiff;
            lastInterval += integerDiff;
            _isServerDataTimeDirty = true;
        }
    }
    #region Window Handler
    private List<UI_Panel> _guiWindows = new List<UI_Panel>();
    private Dictionary<string, bool> _guiLoading = new Dictionary<string, bool>();
    private delegate void CreateGUICallback<T>(T guiPage) where T : UI_Panel;
    public delegate void GUICallback<T>(T guiPage) where T : UI_Panel;
    private void CloseAllPanel()
    {
        foreach (UI_Panel gui in _guiWindows)
        {
            gui.Close();
        }
        _guiWindows.Clear();
    }
    public void ClosePanel<T>() where T : UI_Panel
    {
        T gui = GetUI_Panel<T>();
        if (gui != null)
            gui.Close();
    }
    public T GetUI_Panel<T>() where T : UI_Panel
    {
        System.Type type = typeof(T);
        foreach (UI_Panel window in _guiWindows)
        {
            if ("IPHONE_" + window.GetType().ToString() == type.ToString())
            {
                return (T)(window);
            }

            if (window.GetType() == type)
            {
                return (T)(window);
            }
        }
        return (T)null;
    }
    public void CreatePanel<T>() where T : UI_Panel
    {
        CreatePanel<T>(null, true);
    }
    public void CreatePanel<T>(GUICallback<T> callback) where T : UI_Panel
    {
        CreatePanel<T>(callback, true);
    }
    public void CreatePanel<T>(GUICallback<T> callback, float layout) where T : UI_Panel
    {
        if (layout < 1)
            layout = 1;

        T ui = GetUI_Panel<T>();

        if (null != ui)
        {
            if (null != callback)
                callback(ui);
            return;
        }

        bool guiIsLoading = false;
        if (_guiLoading.TryGetValue(typeof(T).ToString(), out guiIsLoading) && guiIsLoading)
            return;

        _guiLoading[typeof(T).ToString()] = true;
        CreateGUIWindow<T>
        (typeof(T).ToString(),
            delegate(T gui)
            {
                _guiLoading[typeof(T).ToString()] = false;

                gui.InitializeGUI();

                UI_Manager.Instance.AddWindow(gui);
                if (null != callback)
                    callback(gui);
            }
        );

    }
    public void CreatePanel<T>(GUICallback<T> callback, bool updateLayout) where T : UI_Panel
    {
        T ui = GetUI_Panel<T>();
        if (null != ui)
        {
            if (null != callback)
                callback(ui);
            return;
        }

        bool guiIsLoading = false;
        if (_guiLoading.TryGetValue(typeof(T).ToString(), out guiIsLoading) && guiIsLoading)
            return;

        _guiLoading[typeof(T).ToString()] = true;
        //	if (_mIsLoadingGUILogin)
        //		return;
        //	_mIsLoadingGUILogin = true;

        //		Debug.Log("CREATE PANEL:"+typeof(T).ToString());
        CreateGUIWindow<T>
        (typeof(T).ToString(),
            delegate(T gui)
            {
                _guiLoading[typeof(T).ToString()] = false;

                //gui.UpdateLevelDepth();
                gui.InitializeGUI();

                UI_Manager.Instance.AddWindow(gui);
                if (null != callback)
                    callback(gui);
            }
        );
    }
    private void CreateGUIWindow<T>(string name, CreateGUICallback<T> callback) where T : UI_Panel
    {
        StartCoroutine(DoCreateGUIWindow<T>(name, callback));
    }
    private IEnumerator DoCreateGUIWindow<T>(string name, CreateGUICallback<T> callback) where T : UI_Panel
    {
        if (Application.CanStreamedLevelBeLoaded(name))
        {
            AsyncOperation asynOp = Application.LoadLevelAdditiveAsync(name);
            while (!asynOp.isDone)
            {
                yield return null;
                T gui = null;
                GameObject go = GameObject.Find(name);
                if (null != go)
                {
                    gui = go.AddComponent<T>();
                }
                else
                {
                    Debug.LogWarning("GameObject Can't Find:" + name);
                }

                if (null != gui)
                {

                    callback(gui);
                }
                else
                {
                    Debug.LogWarning("GameObject Can't Find Component:");
                }
            }
            yield return null;
        }
    }
    public void AddWindow(UI_Panel window)
    {
        _guiWindows.Add(window);
    }
    public void RemoveWindow(UI_Panel window)
    {
        _guiWindows.Remove(window);
    }
    #endregion
    #region Public Property
    public UICamera uicamera
    {
        get;
        internal set;
    }
    public static Vector3 RootScale = new Vector3(0.003125f, 0.003125f, 0.003125f);
    public float PanelScale
    {
        get
        {
            float scaleY = Screen.height / 640f;
            float scaleX = Screen.width / 960f;

            return scaleY < scaleX ? scaleY : scaleX;

        }
    }
    #endregion

    #region Time
    [HideInInspector]
    public bool ServerTimeInited = false;
    [HideInInspector]
    public int ServerStampTime = 0;
    private bool _isServerDataTimeDirty = true;
    private DateTime _serverDateTime = new DateTime(1970, 1, 1, 0, 0, 0);//(new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(_UnixTimeStamp)
    [HideInInspector]
    public int timeZone = 8;
    [HideInInspector]
    public long voiceRoomId = 0;
    private int lastInterval = 0;

    public float GetLastClickedTime() { return S_LastClickTime; }
    public void SetLastClickedTime(float time)
    {
        S_LastClickTime = time;
    }
    public void InitServerTime(int _stamp)
    {
        InitServerTime(_stamp, timeZone, voiceRoomId);
    }
    public void InitServerTime(int _stamp, int _timeZone, long _voiceroomId)
    {
        //DateTime.
        Debug.Log("ServerTime:" + _stamp + " voiceRoomId:" + _voiceroomId);
        ServerTimeInited = true;
        timeZone = _timeZone;
        ServerStampTime = _stamp;
        voiceRoomId = _voiceroomId;

        lastInterval = (int)Time.realtimeSinceStartup;
        ServerDateTime.AddHours(_timeZone);
        ServerDateTime.AddSeconds(_stamp);
    }
    public DateTime GetTime(int timeStamp)
    {
        //TimeZone
        //long lTime = long.Parse(timeStamp);
        DateTime dtStart = new DateTime(1970, 1, 1).AddSeconds(timeStamp + timeZone * 3600);
        //dtStart.AddHours(timeZone);
        //TimeSpan toNow = new TimeSpan(lTime); 
        return dtStart;//.Add(toNow);    
    }
    //public string 
    //int tempId=0;
    public DateTime ServerDateTime
    {
        internal set { }
        get
        {
            if (_isServerDataTimeDirty)
            {
                //tempId++;
                //Debug.Log(tempId.ToString());
                _isServerDataTimeDirty = false;
                _serverDateTime = new DateTime(1970, 1, 1).AddSeconds(ServerStampTime + timeZone * 3600);
            }
            return _serverDateTime;
        }
    }

    #endregion
}
