using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum ELayoutId
{
    MainMenu,
    Hud,
    PauseMenu,
    GameEndScreen,
}

public class LayoutManager : MonoBehaviour
{
    static LayoutManager Instance;
    public static LayoutManager GetInstance()
    {
        return Instance;
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //////////////////////////////////////////
        InitDictionary();
        InitStartMenu();
    }
    [System.Serializable]
    public class LayoutItem
    {
        public LayoutItem(ELayoutId id, GameObject layoutInstance)
        {
            if (layoutInstance == null)
            {
                Debug.LogError("Layout initializatiuon error: " + id.ToString() + " instance was Invalid");
                return;
            }
            m_id = id;
            m_layoutGameObject = layoutInstance;
        }

        public ELayoutId m_id;
        public GameObject m_layoutGameObject;
    }

    public LayoutItem[] m_layoutCollection;
    private Dictionary<ELayoutId, GameObject> m_layoutDictionary;

    public List<LayoutItem> m_activatedLayouts = new List<LayoutItem>();

    void InitDictionary()
    {
        int length = m_layoutCollection.Length;
        m_layoutDictionary = new Dictionary<ELayoutId, GameObject>(length);
        foreach (var item in m_layoutCollection)
        {
            m_layoutDictionary.Add(item.m_id, item.m_layoutGameObject);
        }
    }
    void InitStartMenu()
    {
        PushLayout(ELayoutId.MainMenu);
    }
    public void PushLayout(ELayoutId id, bool disablePreviousLayout = true)
    {
        GameObject refer = m_layoutDictionary[id];
        if (refer == null)
        {
            Debug.LogError("Trying to push invalid layout: " + id.ToString() + " instance was NULL in dictionary");
        }

        if (m_activatedLayouts.Count > 0)
        {
            m_activatedLayouts[m_activatedLayouts.Count - 1].m_layoutGameObject.SetActive(!disablePreviousLayout);
            if (disablePreviousLayout)
            {
                m_activatedLayouts[m_activatedLayouts.Count - 1].m_layoutGameObject.GetComponent<ILayoutItem>().OnBackgroundDeactivation();
            }
        }
        LayoutItem layoutItem = new LayoutItem(id, refer);
        ILayoutItem ilayoutItemInterface = layoutItem.m_layoutGameObject.GetComponent<ILayoutItem>();
        if (ilayoutItemInterface == null)
        {
            Debug.LogError("Requested layout has no interface implementation: " + id.ToString());
        }
        ilayoutItemInterface.OnLoad();
        layoutItem.m_layoutGameObject.SetActive(true);
        m_activatedLayouts.Add(layoutItem);
    }

    public void PopLayout()
    {
        if (m_activatedLayouts.Count == 0)
        {
            return;
        }

        m_activatedLayouts[m_activatedLayouts.Count - 1].m_layoutGameObject.SetActive(false);
        m_activatedLayouts[m_activatedLayouts.Count - 1].m_layoutGameObject.GetComponent<ILayoutItem>().OnUnload();
        m_activatedLayouts.RemoveAt(m_activatedLayouts.Count - 1);

        if (m_activatedLayouts.Count > 0)
        {
            m_activatedLayouts[m_activatedLayouts.Count - 1].m_layoutGameObject.GetComponent<ILayoutItem>().OnReactivationFromBackground();
            m_activatedLayouts[m_activatedLayouts.Count - 1].m_layoutGameObject.SetActive(true);
        }
    }

    public void ClearLayoutStack()
    {
        foreach (var item in m_activatedLayouts)
        {
            item.m_layoutGameObject.GetComponent<ILayoutItem>().OnUnload();
            item.m_layoutGameObject.SetActive(false);
        }
        m_activatedLayouts.Clear();
    }
}


//#region ILayoutItem
//public void OnLoad() { }
//public void OnBackgroundDeactivation() { }
//public void OnReactivationFromBackground() { }
//public void OnUnload() { }
//#endregion