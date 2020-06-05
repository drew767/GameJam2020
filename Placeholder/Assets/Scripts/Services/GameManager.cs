using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance;
    public static GameManager GetInstance()
    {
        return Instance;
    }

    PlayerController player = null;
    public PlayerController Player { get { return player; } }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
        Debug.Assert(player, "Player is not in scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeGame() { }
    public void ClearGame() { }
    public void CheckEndgameCondition() {}

    public GameObject GetNewObject(ESpawnItemType type)
    {
        return m_poolOfObjects.GetNewObject(type);
    }

    public void DestroyObject(ESpawnItemType type, GameObject objectToDestroy)
    {
        m_poolOfObjects.DestroyObject(type, objectToDestroy);
    }


    public PortalManager m_portalManager;
    public ObjectPool m_poolOfObjects;
}
