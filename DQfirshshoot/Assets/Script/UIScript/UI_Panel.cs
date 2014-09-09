using UnityEngine;
using System.Text;
using System.Collections;

public abstract class UI_Panel : MonoBehaviour
{
    public enum UIPanelsFlag
    {
        UI_NoWhere,
        UI_ShipList,
        UI_DailyAward,
        UI_Battle,
        UI_BattleAutoEnter,
        UI_GeneralSystem,
        UI_Waihai,
        UI_RecruitHero,
        UI_BuildShip,
        UI_ShipFormation,
        UI_VSInformation,
        UI_ConveyFleet,
        UI_PVP,
        UI_ResFight,
    }
    public const float GUI_NEAREST_Z = -1000.0f;
    public const float GUI_FARTHEST_Z = 0.0f;
    public const float GUI_ORDER_SPACE = 100.0f;

    public abstract void InitializeGUI();


    public virtual void Awake()
    {
        if (null != UI_Manager.Instance)
        {


            transform.parent = UI_Manager.Instance.uicamera.transform;

            UpdateLevelDepth();

            //	_//DepthLevel = value;

        }
        Resources.UnloadUnusedAssets();
    }
    public void UpdateLevelDepth()
    {
        float scaleMin = UI_Manager.Instance.PanelScale;

        transform.localScale = new Vector3(scaleMin, scaleMin, scaleMin);

        Vector3 pos = transform.localPosition;
        pos.z = (GUI_NEAREST_Z + DepthLevel * GUI_ORDER_SPACE);///UI_Manager.RootScale.z;
        transform.localPosition = pos;
    }
    public virtual void OnDestroy()
    {
        if (null != UI_Manager.Instance)
            UI_Manager.Instance.RemoveWindow(this);
        Resources.UnloadUnusedAssets();
    }

    private bool isHidden = false;
    public bool Hidden
    {
        set
        {
            isHidden = value;

            NGUITools.SetActive(this.gameObject, !isHidden);
        }
        get { return isHidden; }
    }

    #region Public Method

    public void CloseImmediate()
    {
        DestroyImmediate(this.gameObject);
    }
    // for register
    public void CloseImmediate(GameObject go)
    {
        DestroyImmediate(this.gameObject);
    }
    public void Close()
    {
        Destroy(this.gameObject);
        GL.Clear(false, true, Color.black);
        Resources.UnloadUnusedAssets();
    }

    // for register
    public void Close(GameObject go)
    {
        Destroy(this.gameObject);
    }
    #endregion

    #region Public Property
    /// <summary>
    /// Gets or sets the depth level.0-near 10-far
    /// </summary>
    /// <value>
    /// The depth level. between 0 and 10,
    /// </value>
    public float DepthLevel = 10;
    public bool playOpenCloseSound = true;

    //private int _depthLevel=0;


    /// <summary>
    /// The is full screen. if your want invisiable next level panel,set ture in editor 
    /// </summary>
    public bool isFullScreen = false;

    public bool OnlyTopDisplay = false;

    public bool RefreshTutorialCollections = false;
    //public bool RefreshTutorialTips = false;
    #endregion
}
