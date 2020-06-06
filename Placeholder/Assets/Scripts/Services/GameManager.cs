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

        PauseGame();
    }

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
        Debug.Assert(player, "Player is not in scene");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_gameIsTicking)
        {
            m_gameTime += Time.deltaTime;

            CheckPauseMenuRequest();
        }
    }

    void CheckPauseMenuRequest()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGame();
            LayoutManager.GetInstance().PushLayout(ELayoutId.PauseMenu);
        }
    }

    public void PauseGame()
    {
        m_gameIsTicking = false;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        m_gameIsTicking = true;
        Time.timeScale = 1;
    }

    public void StartPlayingGame()
    {
        ClearGame();
        PrepareToStartGame();

        m_gameTime = 0.0f;
        ResumeGame();
        m_gameIsFinished = false;
    }

    public void ClearGame() 
    {
        InvalidateAllSpawnedObjects();
    }
    public void PrepareToStartGame() { }

    public void CheckEndgameCondition() 
    {
        m_gameIsFinished = player.GetIsDead();

        if (m_gameIsFinished)
        {
            LayoutManager.GetInstance().PushLayout(ELayoutId.GameEndScreen);
        }
    }

    public GameObject GetNewObject(ESpawnItemType type)
    {
        return m_poolOfObjects.GetNewObject(type);
    }

    public void DestroyObject(GameObject objectToDestroy)
    {
        m_poolOfObjects.DestroyObject(objectToDestroy);
    }

    void InvalidateAllSpawnedObjects()
    {
        m_poolOfObjects.DeactivateAllPools();
    }

    public float GetGameTime() { return m_gameTime; }
    public string GetGameTimeInString()
    {
        string minutes = Mathf.Floor(m_gameTime / 60).ToString("00");
        string seconds = (m_gameTime % 60).ToString("00");
        return string.Format("{0}:{1}", minutes, seconds);
    }

    PlayerController player = null;
    public PlayerController Player { get { return player; } }

    float m_gameTime = 0.0f;
    bool m_gameIsTicking = false;
    bool m_gameIsFinished = false;
    public PortalManager m_portalManager;
    public ObjectPool m_poolOfObjects;
}
