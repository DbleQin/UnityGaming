using UnityEngine;
using System.Text;
using System.Collections;

public abstract class UI_Panel : MonoBehaviour
{
    public enum UIPanelsFlag
    {
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
        }
        Resources.UnloadUnusedAssets();
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
    public void Close()
    {
        Destroy(this.gameObject);
        GL.Clear(false, true, Color.black);
        Resources.UnloadUnusedAssets();
    }
    #endregion
}
