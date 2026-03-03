using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManagerBase : MonoBehaviourPunCallbacks
{
    public static GameManagerBase Instance;

    public enum gameState
    {
        LevelSelect,
        Lobby,
        Running,
        Pause,
        Win,
        Lose
    }

    public enum mp_gameMode
    {
        FreeForAll = 0,
        Coop
    }

    [SerializeField] public gameState currState;
    [SerializeField] bool isMultiplayer;
    [SerializeField] public mp_gameMode currGameMode;
    [SerializeField] float freeForAllTimer = 0;
    [SerializeField] PhotonView PV;


    [SerializeField] string singlePlaySceneName = "SampleScene";

    [SerializeField] public LevelInfo.levelDifficulty difficulty             ;
    [SerializeField] public float mapRadius              ;
    [SerializeField] public int asteroidCount          ;
    [SerializeField] public int asteroidMinScale       ;
    [SerializeField] public int asteroidMaxScale       ;
    [SerializeField] public int asteroidMinGap         ;
    [SerializeField] public bool fluffyAsteroidBounds   ;
    [SerializeField] public float fluffyFactor           ;
    [SerializeField] public bool spawnOutsideAsteroids  ;
    [SerializeField] public LevelInfo.levelBackground backgroundSettings     ;
    [SerializeField] public LevelInfo.largeObject backgroundObject       ;
    [SerializeField] public int numOfWaves             ;
    [SerializeField] public int initialReward          ;
    [SerializeField] public int secondaryReward;

    public LevelInfo.LevelSettings levelData = new LevelInfo.LevelSettings();


    [SerializeField] public PlayerManager[] _playermangers;
    [SerializeField] public GameObject timer;
    [SerializeField] public GameObject Results;
    [SerializeField] public bool debug_showAICollisionAvoidance = false;
    [SerializeField] public bool debug_showAIStates = false;
    [SerializeField] public bool debug_showColiders = false;
    bool singlePlayerWon = false;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        levelData = new LevelInfo.LevelSettings();
        if (isMultiplayer)
        {
            PV = GetComponent<PhotonView>();
        }
    }

    //public override void OnJoinedRoom()
    //{
    //    if (!PhotonNetwork.IsMasterClient)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //}

    private void Update()
    {
        if (isMultiplayer)
        {
            
            MultiplayerUpdate();
        }
        else
        {
            SingleplayerUpdate();
        }
    }

    public void SingleplayerUpdate()
    {
        switch (currState)
        {
            case gameState.LevelSelect:
                break;
            case gameState.Running:
                break;
            case gameState.Pause:
                break;
            case gameState.Win:
                if (!singlePlayerWon)
                {
                    FindObjectOfType<Player4Base>().OnWin(levelData.initialReward);
                    singlePlayerWon = true;
                }
                break;
            case gameState.Lose:
                break;
        }
    }

    public void MultiplayerUpdate()
    {
        switch (currState)
        {
            case gameState.Lobby:
                checkData();
                break;
            case gameState.Running:
              
              
                
                break;
            case gameState.Win:
                //Results.SetActive(true);
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                //for (int i = 0; i < _playermangers.Length; i++)
                //{
                //    _playermangers[i].GameOver();
                //}
                break;
            case gameState.Lose:
                break;
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        freeForAllTimer = time;
    }


    public void setState(gameState newState)
    {
        currState = newState;
    }

    public gameState getState()
    {
        return currState;
    }

    public mp_gameMode getGameMode()
    {
        return currGameMode;
    }

    public void setLevel(LevelInfo.LevelSettings fromSettings)
    {
        if (isMultiplayer)
        {
            byte[] bit = convertToByte(fromSettings);
            PV.RPC("RPC_setLevel", RpcTarget.All, bit);
        }
        else
        {
            //Map Settings
            levelData.difficulty = fromSettings.difficulty;
            levelData.mapRadius = fromSettings.mapRadius;
            //Asteroid Settings
            levelData.asteroidCount = fromSettings.asteroidCount;
            levelData.asteroidMinScale = fromSettings.asteroidMinScale;
            levelData.asteroidMaxScale = fromSettings.asteroidMaxScale;
            levelData.asteroidMinGap = fromSettings.asteroidMinGap;
            levelData.fluffyAsteroidBounds = fromSettings.fluffyAsteroidBounds;
            levelData.fluffyFactor = fromSettings.fluffyFactor;
            levelData.spawnOutsideAsteroids = fromSettings.spawnOutsideAsteroids;
            //Background Settings
            levelData.backgroundSettings = fromSettings.backgroundSettings;
            levelData.backgroundObject = fromSettings.backgroundObject;
            //Enemy Settings
            levelData.numOfWaves = fromSettings.numOfWaves;
            //Completion Values
            levelData.initialReward = fromSettings.initialReward;
            levelData.secondaryReward = fromSettings.secondaryReward;
        }
    }

    void checkData()
    {
        //DEBUG CHECKING THESE VALUES EVERY FRAME
        difficulty = levelData.difficulty;
        mapRadius = levelData.mapRadius;
        asteroidCount = levelData.asteroidCount;
        asteroidMinScale = levelData.asteroidMinScale;
        asteroidMaxScale = levelData.asteroidMaxScale;
        asteroidMinGap = levelData.asteroidMinGap;
        fluffyAsteroidBounds = levelData.fluffyAsteroidBounds;
        fluffyFactor = levelData.fluffyFactor;
        spawnOutsideAsteroids = levelData.spawnOutsideAsteroids;
        backgroundSettings = levelData.backgroundSettings;
        backgroundObject = levelData.backgroundObject;
        numOfWaves = levelData.numOfWaves;
        initialReward = levelData.initialReward;
        secondaryReward = levelData.secondaryReward;
    }

    public byte[] convertToByte(LevelInfo.LevelSettings set)
    {
        List<int> arraySteps = new List<int>();
        arraySteps.Add(0);
        for (int i = 1; i < 14; i++)
        {
            arraySteps.Add(sizeof(int) * i);
        }

        int size = sizeof(int) * 14;
        byte[] bit = new byte[size];

        bit[arraySteps[0]] = (byte)set.mapRadius;
        bit[arraySteps[1]] = (byte)((int)set.difficulty);
        bit[arraySteps[2]] = (byte)set.asteroidCount;
        bit[arraySteps[3]] = (byte)set.asteroidMinScale;
        bit[arraySteps[4]] = (byte)set.asteroidMaxScale;
        bit[arraySteps[5]] = (byte)set.asteroidMinGap;
        bit[arraySteps[6]] = (byte)((int)set.backgroundSettings);
        bit[arraySteps[7]] = (byte)((int)set.backgroundObject);
        bit[arraySteps[8]] = (byte)set.numOfWaves;
        bit[arraySteps[9]] = (byte)set.initialReward;
        bit[arraySteps[10]] = (byte)set.secondaryReward;

        int fluffnum = 0;
        if (set.fluffyAsteroidBounds == true)
        {
            fluffnum = 1;
        }
        int spawnoutnum = 0;
        if (set.spawnOutsideAsteroids == true)
        {
            spawnoutnum = 1;
        }

        bit[arraySteps[11]] = (byte)fluffnum;
        bit[arraySteps[12]] = (byte)set.fluffyFactor;
        bit[arraySteps[13]] = (byte)spawnoutnum;
        
        

        return bit;
    }

    public LevelInfo.LevelSettings convertToSettings(byte[] bit)
    {
        List<int> arraySteps = new List<int>();
        arraySteps.Add(0);
        for (int i = 1; i < 14; i++)
        {
            arraySteps.Add(sizeof(int) * i);
        }

        LevelInfo.LevelSettings set = new LevelInfo.LevelSettings();

        set.mapRadius = bit[arraySteps[0]];
        set.difficulty = (LevelInfo.levelDifficulty)bit[arraySteps[1]];
        set.asteroidCount = bit[arraySteps[2]];
        set.asteroidMinScale = bit[arraySteps[3]];
        set.asteroidMaxScale = bit[arraySteps[4]];
        set.asteroidMinGap = bit[arraySteps[5]];
        set.backgroundSettings = (LevelInfo.levelBackground)bit[arraySteps[6]];
        set.backgroundObject = (LevelInfo.largeObject)bit[arraySteps[7]];
        set.numOfWaves = bit[arraySteps[8]];
        set.initialReward = bit[arraySteps[9]];
        set.secondaryReward = bit[arraySteps[10]];

        int fluffnum = bit[arraySteps[11]];
        if (fluffnum == 1)
        {
            set.fluffyAsteroidBounds = true;
        }
        else
        {
            set.fluffyAsteroidBounds = false;
        }
        set.fluffyFactor = bit[arraySteps[12]];
        int spawnoutnum = bit[arraySteps[13]];
        if (spawnoutnum == 1)
        {
            set.spawnOutsideAsteroids = true;
        }
        else
        {
            set.spawnOutsideAsteroids = false;
        }

        return set;
    }

    [PunRPC]
    void RPC_setLevel(byte[] bit)
    {
        LevelInfo.LevelSettings fromSettings = convertToSettings(bit);
        //Map Settings
        levelData.difficulty = fromSettings.difficulty;
        levelData.mapRadius = fromSettings.mapRadius;
        //Asteroid Settings
        levelData.asteroidCount = fromSettings.asteroidCount;
        levelData.asteroidMinScale = fromSettings.asteroidMinScale;
        levelData.asteroidMaxScale = fromSettings.asteroidMaxScale;
        levelData.asteroidMinGap = fromSettings.asteroidMinGap;
        levelData.fluffyAsteroidBounds = fromSettings.fluffyAsteroidBounds;
        levelData.fluffyFactor = fromSettings.fluffyFactor;
        levelData.spawnOutsideAsteroids = fromSettings.spawnOutsideAsteroids;
        //Background Settings
        levelData.backgroundSettings = fromSettings.backgroundSettings;
        levelData.backgroundObject = fromSettings.backgroundObject;
        //Enemy Settings
        levelData.numOfWaves = fromSettings.numOfWaves;
        //Completion Values
        levelData.initialReward = fromSettings.initialReward;
        levelData.secondaryReward = fromSettings.secondaryReward;
    }

    public void getLevel(out LevelInfo.LevelSettings set)
    {
        //Map Settings
        set.difficulty = levelData.difficulty;
        set.mapRadius =  levelData.mapRadius;
        //Asteroid Settings
        set.asteroidCount =         levelData.asteroidCount;
        set.asteroidMinScale =      levelData.asteroidMinScale;
        set.asteroidMaxScale =      levelData.asteroidMaxScale;
        set.asteroidMinGap =        levelData.asteroidMinGap;
        set.fluffyAsteroidBounds =  levelData.fluffyAsteroidBounds;
        set.fluffyFactor =          levelData.fluffyFactor;
        set.spawnOutsideAsteroids = levelData.spawnOutsideAsteroids;
        //Background Settings
        set.backgroundSettings =       levelData.backgroundSettings;
        set.backgroundObject =         levelData.backgroundObject;
        //Enemy Settings
        set.numOfWaves =                 levelData.numOfWaves;
        //Completion Values
        set.initialReward =   levelData.initialReward;
        set.secondaryReward = levelData.secondaryReward;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(gameObject);
        }
        if (scene.buildIndex == 2)
        {
            timer = GameObject.Find("CountDownTimer");
            Results = GameObject.Find("GaneOver");
        }
    }

    public bool isMulti()
    {
        return isMultiplayer;
    }

    public void toggleDebug_showAICollision()
    {
        debug_showAICollisionAvoidance = !debug_showAICollisionAvoidance;
    }

    public bool getDebug_showAICollision()
    {
        return debug_showAICollisionAvoidance;
    }

    public void toggleDebug_showAIStates()
    {
        debug_showAIStates = !debug_showAIStates;
    }
    public void toggleDebug_showColiders()
    {
        debug_showColiders = !debug_showColiders;
    }

    public bool getDebug_showAIStates()
    {
        return debug_showAIStates;
    }

    void setTimer(float time)
    {
        freeForAllTimer = time;
    }

    public float getTimer()
    {
        return freeForAllTimer;
    }


    public PlayerManager[] GetPlayerManagers()
    {
        return _playermangers;
    }


}
