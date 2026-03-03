using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
   // public enum gameState
   // {
   //     MainMenu,
   //     LevelSelect,
   //     Running,
   //     Pause,
   //     Win,
   //     Lose
   // }

   // [Header("Game Fields")]
   // [SerializeField] public GameObject player;
   // [SerializeField] public Player3 playerScript;
   // [SerializeField] gameState currState;

   // //Wave/Enemy Data for HUD--------------
   // [Header("HUD Data")]
   // [SerializeField] int currWaveIndex;
   // [SerializeField] int currWaveMax;
   // [SerializeField] int currWaveRemaining;

   // //-------------------------------------
   // [Header("Multiplayer")]
   // [SerializeField]public bool  MultiplayerGame = false;
   // [SerializeField]public List<PlayerScript> players;
   //// [SerializeField] public bool Sync = true;
   // //-------------------------------------
   // [Header("Debug Options")]
   // [SerializeField] bool DebugLevel = false;
   // //Level Variables---------------------
   // //[Header("Debug Level Variables")]
   // //[SerializeField] public float mapRadius;
   // //[SerializeField] public int asteroidCount;
   // //[SerializeField] public int asteroidMinScale;
   // //[SerializeField] public int asteroidMaxScale;
   // //[SerializeField] public int asteroidMinGap;
   // //[SerializeField] public bool fluffyAsteroidBounds;
   // //[SerializeField] public float fluffyFactor;
   // //[SerializeField] public bool spawnOutsideAsteroids;
   // //[SerializeField] public LevelBuilder.levelBackground backgroundSettings;
   // //[SerializeField] LevelBuilder.largeObject backgroundObject;
   // //[SerializeField] public int numOfWaves;
   // //[SerializeField] public List<int> numOfEnemies;
   // //[SerializeField] public int levelReward;

   // //-------------------------------------
   // private float playerhealth;
   // public LevelBuilder.LevelSettings levelData;
   
    

   // // Start is called before the first frame update
   // void Start()
   // {
   //     GameManager otherManager = FindObjectOfType<GameManager>();
   //     if (otherManager != this && otherManager != null)
   //     {
   //         Destroy(this.gameObject);
   //         return;
   //     }
   //     else
   //     {
   //         DontDestroyOnLoad(this);
   //     }

   //     if(playerScript)
   //     {
   //         playerhealth = playerScript.GetHealth();
   //     }
   //     if (DebugLevel)
   //     {
   //         buildLevel();
   //     }

   //     if (MultiplayerGame == true)
   //     {
   //         int countOfPlayers = PhotonNetwork.CountOfPlayers;
   //         players = new List<PlayerScript>(countOfPlayers);
   //     }
   // }

   // // Update is called once per frame
   // void Update()
   // {
   //     if (playerScript)
   //     {
   //         playerhealth = playerScript.GetHealth();
   //     }
   //     if (MultiplayerGame == true)
   //     {
   //         for (int i = 0; i < players.Count; i++)
   //         {
   //             //area to access variables of players
   //         }
   //         return;
   //     }
   //     if (currState == gameState.Running)
   //     {
   //         Lose();
   //         Win();
   //     }


   //     //debug
   //     if (Input.GetKeyDown(KeyCode.I))
   //     {
   //         setWin();
   //     }
   // }

   // public void setLevel(LevelBuilder.LevelSettings fromSettings)
   // {
   //     levelData = fromSettings;
        
   //     mapRadius = fromSettings.mapRadius;
   //     asteroidCount = fromSettings.asteroidCount;
   //     asteroidMinScale = fromSettings.asteroidMinScale;
   //     asteroidMaxScale = fromSettings.asteroidMaxScale;
   //     asteroidMinGap = fromSettings.asteroidMinGap;
   //     fluffyAsteroidBounds = fromSettings.fluffyAsteroidBounds;
   //     fluffyFactor = fromSettings.fluffyFactor;
   //     backgroundSettings = fromSettings.backgroundSettings;
   //     backgroundObject = fromSettings.backgroundObject;
   //     numOfWaves = fromSettings.numOfWaves;
   //     numOfEnemies = new List<int>(fromSettings.numOfEnemies);
   //     levelReward = fromSettings.reward;
   // }

   // public void setPlayer(GameObject play)
   // {
   //     player = play;
   //     playerScript = player.GetComponent<Player3>();
   // }

   // //Setters
   // public void setState(gameState State)
   // {
   //     currState = State;
   // }

   // public void setEnemyCount(int waveIndex, int count)
   // {
   //     numOfEnemies[waveIndex] = count;
   // }

   // public void setWaveCount(int count)
   // {
   //     numOfWaves = count;
   // }

   // public bool setMulti(bool mult)
   // {
   //     return MultiplayerGame = mult;
   // }

   // //Getters
   // public gameState getState()
   // {
   //     return currState;
   // }

   // public int getEnemyCount()
   // {
   //     return currWaveRemaining;
   // }

   // public int getWaveMax()
   // {
   //     return currWaveMax;
   // }

   // public int getCurrWave()
   // {
   //     return currWaveIndex + 1;
   // }

   // public List<int> getAllEnemyCount()
   // {
   //     return numOfEnemies;
   // }

   // public int getWaveCount()
   // {
   //     return numOfWaves;
   // }

   // public bool getMulti()
   // {
   //     return MultiplayerGame;
   // }

   // public GameObject getPlayer()
   // {
   //     return player;
   // }

   // public void getLevel(out LevelBuilder.LevelSettings set)
   // {
   //     set.backgroundSettings = levelData.backgroundSettings;

   //     if (!getMulti() || PhotonNetwork.IsMasterClient)
   //     {
   //         set.mapRadius = levelData.mapRadius;
   //         set.asteroidCount = levelData.asteroidCount;
   //         set.asteroidMinScale = levelData.asteroidMinScale;
   //         set.asteroidMaxScale = levelData.asteroidMaxScale;
   //         set.asteroidMinGap = levelData.asteroidMinGap;
   //         set.fluffyAsteroidBounds = levelData.fluffyAsteroidBounds;
   //         set.fluffyFactor = levelData.fluffyFactor;
   //         set.backgroundObject = levelData.backgroundObject;
   //         set.spawnOutsideAsteroids = levelData.spawnOutsideAsteroids;
   //         set.numOfWaves = levelData.numOfWaves;
   //         set.numOfEnemies = new List<int>(levelData.numOfEnemies);
   //         set.reward = levelData.reward;
   //     }
   //     else
   //     {
   //         set.mapRadius = 0;
   //         set.asteroidCount = 0;
   //         set.asteroidMinScale = 0;
   //         set.asteroidMaxScale = 0;
   //         set.asteroidMinGap = 0;
   //         set.fluffyAsteroidBounds = false;
   //         set.fluffyFactor = 0;
   //         set.backgroundObject = 0;
   //         set.spawnOutsideAsteroids = false;
   //         set.numOfWaves = 0;
   //         set.numOfEnemies = new List<int>();
   //         set.reward = 0;
   //     }
        
   // }

   // ////Public Methods
   // //public void buildLevel()
   // //{
   // //    levelData = new LevelBuilder.LevelSettings();
   // //    levelData.mapRadius = mapRadius;
   // //    levelData.asteroidCount = asteroidCount;
   // //    levelData.asteroidMaxScale = asteroidMaxScale;
   // //    levelData.asteroidMinScale = asteroidMinScale;
   // //    levelData.asteroidMinGap = asteroidMinGap;
   // //    levelData.fluffyAsteroidBounds = fluffyAsteroidBounds;
   // //    levelData.fluffyFactor = fluffyFactor;
   // //    levelData.spawnOutsideAsteroids = spawnOutsideAsteroids;
   // //    levelData.backgroundSettings = backgroundSettings;
   // //    levelData.backgroundObject = backgroundObject;
   // //    levelData.numOfWaves = numOfWaves;
   // //    levelData.numOfEnemies = new List<int>(numOfEnemies);
   // //    levelData.reward = levelReward;
   // //}


   // //Private Methods
   // void Lose()
   // {
   //     if (playerhealth <= 0)
   //     {
   //         currState = gameState.Lose;
   //     }
   // }

   // public void Win()
   // {
   //     if (currWaveIndex >= levelData.numOfWaves)
   //     {
   //         currState = gameState.Win;
   //         playerScript.OnWin(levelData.reward);
   //     }
   // }

   // public void setHUDData(int wave, int waveMax, int remaining)
   // {
   //     currWaveIndex = wave;
   //     currWaveMax = waveMax;
   //     currWaveRemaining = remaining;
   // }

   // public void setWin()
   // {
   //     currState = gameState.Win;
   // }
}
