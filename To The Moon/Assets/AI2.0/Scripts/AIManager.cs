using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    public enum MatchType
    {
        SoloVsWaves = 0,
        SmallTeamVsWaves,
        TeamVsTeam
    }

    public enum Teams
    {
        Team1 = 1,
        Team2,
        Team3
    }

    public struct HUDWaveData
    {
        public int totalWaves;
        public int currentWave;
        public int currentWaveTotalEnemies;
        public int currentWaveRemainingEnemies;
    }

    public struct ShipData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 avoidHeading;
        public int neighbors;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 3 + sizeof(int);
            }
        }
    }

    [Header("Debug")]
    [SerializeField] bool debug_NoGameManager = false;

    [Header("Match Details")]
    [SerializeField] public MatchType gameTypeSetting = MatchType.SoloVsWaves;
    [SerializeField] LevelInfo.levelDifficulty difficulty = LevelInfo.levelDifficulty.Medium;
    [SerializeField] LevelInfo.LevelSettings settings = new LevelInfo.LevelSettings();
    [SerializeField] List<int> waves = new List<int>();
    [SerializeField] float debugMapRadius = 1000;

    [Header("Prefab Set")]
    [SerializeField] List<GameObject> AIprefabs;
    [SerializeField] GameObject AIprefabMulti;
    [SerializeField] Transform Team1Container;
    [SerializeField] Transform Team2Container;
    [SerializeField] Transform Team3Container;
    [Header("Slot 0 = Team 1, Enemy waves are always team 1")]
    [SerializeField] List<Material> teamColors;
    [SerializeField] Queue<GameObject> team1Stored = new Queue<GameObject>();
    [SerializeField] Queue<GameObject> team2Stored = new Queue<GameObject>();
    [SerializeField] Queue<GameObject> team3Stored = new Queue<GameObject>();
    [SerializeField] List<GameObject> team1deployed = new List<GameObject>();
    [SerializeField] List<GameObject> team2deployed = new List<GameObject>();
    [SerializeField] List<GameObject> team3deployed = new List<GameObject>();
    
    [Header("Difficulty/Spawn numbers")]
    [SerializeField] public int easyWaveMin = 3;
    [SerializeField] public int easyWaveMax = 7;
    [SerializeField] public int mediumWaveMin = 5;
    [SerializeField] public int mediumWaveMax = 9;
    [SerializeField] public int hardWaveMin = 8;
    [SerializeField] public int hardWaveMax = 12;
    [SerializeField] public int extremeWaveMin = 12;
    [SerializeField] public int extremeWaveMax = 20;

    [SerializeField] public int easyTeamMin = 3;
    [SerializeField] public int easyTeamMax = 7;
    [SerializeField] public int mediumTeamMin = 5;
    [SerializeField] public int mediumTeamMax = 9;
    [SerializeField] public int hardTeamMin = 8;
    [SerializeField] public int hardTeamMax = 15;
    [SerializeField] public int extremeTeamMin = 14;
    [SerializeField] public int extremeTeamMax = 25;

    [SerializeField] int globalTeamSize = 0;
    [SerializeField] int allySmallTeamSize = 5;
    [SerializeField] float spawnDistanceFromCamera = 400;
    [SerializeField] float spawnRadiusFromObjects = 50;
    [SerializeField] LayerMask spawnAvoidLayer;
    [SerializeField] public ComputeShader compute;
    [SerializeField] public float SeparationVisionRadius = 50;
    [SerializeField] public float SeparationAvoidRadius = 15;
    const int TGS = 1024;

    List<int> difficultyWaveSpawnMin = new List<int>();
    List<int> difficultyWaveSpawnMax = new List<int>();

    List<int> difficultyTeamSpawnMin = new List<int>();
    List<int> difficultyTeamSpawnMax = new List<int>();

    private int currentWaveTeam1;
    private bool waveSpawnedTeam1 = false;
    private int waveSpawnRemainingTeam1 = 0;

    private int currentWaveTeam2;
    private bool waveSpawnedTeam2 = false;
    private int waveSpawnRemainingTeam2 = 0;

    private int currentWaveTeam3;
    private bool waveSpawnedTeam3 = false;
    private int waveSpawnRemainingTeam3 = 0;

    private void Awake()
    {
        if (debug_NoGameManager)
        {
            Instance = this;
            return;

        }
        if (GameManagerBase.Instance.isMulti() && !PhotonNetwork.IsMasterClient)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (debug_NoGameManager)
        {
            settings.mapRadius = (int)debugMapRadius;
            settings.difficulty = difficulty;
        }
        else
        {
            GameManagerBase.Instance.getLevel(out settings);
            difficulty = settings.difficulty;
        }
        
        initSizeLists();
        switch (gameTypeSetting)
        {
            case MatchType.SoloVsWaves:
                InitializeSoloVsWavesGame();
                break;
            case MatchType.SmallTeamVsWaves:
                InitializeSmallTeamVsWavesGame();
                break;
            case MatchType.TeamVsTeam:
                InitializeTeamVsTeamGame();
                break;
            default:
                break;
        }
        currentWaveTeam1 = 0;
    }

    private void Update()
    {
        if (GameManagerBase.Instance)
        {
            if (GameManagerBase.Instance.getState() != GameManagerBase.gameState.Running)
            {
                return;
            }
        }
        switch (gameTypeSetting)
        {
            case MatchType.SoloVsWaves:
                if (currentWaveTeam1 < waves.Count)//checks to make sure there are waves left
                {
                    if (!waveSpawnedTeam1)//checks if the wave is finished spawning
                    {
                        if (waveSpawnRemainingTeam1 == 0)//if the wave hasn't spawned but the remaining count is 0, we havent started spawning
                        {
                            waveSpawnRemainingTeam1 = waves[currentWaveTeam1];
                        }
                        else if (waveSpawnRemainingTeam1 > 0)//if we still have units to spawn
                        {
                            if (spawnAIUnit(1))//try to spawn 1 every frame
                            {
                                waveSpawnRemainingTeam1--;
                            }
                            if (waveSpawnRemainingTeam1 == 0)//if there are none left to spawn after successfully spawning, wave is done spawning
                            {
                                waveSpawnedTeam1 = true;
                            }
                        }
                    }
                    else if(team1deployed.Count == 0)//if the wave has finished spawning, check to deployed count every frame to determine if the enemies are all dead
                    {
                        currentWaveTeam1++;//if so, increment the wave and reset the spawned bool
                        waveSpawnedTeam1 = false;
                    }
                }
                else
                {
                    if (GameManagerBase.Instance)
                    {
                        GameManagerBase.Instance.setState(GameManagerBase.gameState.Win);
                    }
                }
                EscapeCheck(1);
                break;
            case MatchType.SmallTeamVsWaves:
                
                break;
            case MatchType.TeamVsTeam:
                if (currentWaveTeam1 < waves.Count)//checks to make sure there are waves left
                {
                    if (!waveSpawnedTeam1)//checks if the wave is finished spawning
                    {
                        if (waveSpawnRemainingTeam1 == 0)//if the wave hasn't spawned but the remaining count is 0, we havent started spawning
                        {
                            waveSpawnRemainingTeam1 = waves[currentWaveTeam1];
                        }
                        else if (waveSpawnRemainingTeam1 > 0)//if we still have units to spawn
                        {
                            if (spawnAIUnit(1))//try to spawn 1 every frame
                            {
                                waveSpawnRemainingTeam1--;
                            }
                            if (waveSpawnRemainingTeam1 == 0)//if there are none left to spawn after successfully spawning, wave is done spawning
                            {
                                waveSpawnedTeam1 = true;
                            }
                        }
                    }
                    else if (team1deployed.Count == 0)//if the wave has finished spawning, check to deployed count every frame to determine if the enemies are all dead
                    {
                        currentWaveTeam1++;//if so, increment the wave and reset the spawned bool
                        waveSpawnedTeam1 = false;
                    }
                }
                if (currentWaveTeam2 < waves.Count)//checks to make sure there are waves left
                {
                    if (!waveSpawnedTeam2)//checks if the wave is finished spawning
                    {
                        if (waveSpawnRemainingTeam2 == 0)//if the wave hasn't spawned but the remaining count is 0, we havent started spawning
                        {
                            waveSpawnRemainingTeam2 = waves[currentWaveTeam2];
                        }
                        else if (waveSpawnRemainingTeam2 > 0)//if we still have units to spawn
                        {
                            if (spawnAIUnit(2))//try to spawn 1 every frame
                            {
                                waveSpawnRemainingTeam2--;
                            }
                            if (waveSpawnRemainingTeam2 == 0)//if there are none left to spawn after successfully spawning, wave is done spawning
                            {
                                waveSpawnedTeam2 = true;
                            }
                        }
                    }
                    else if (team2deployed.Count == 0)//if the wave has finished spawning, check to deployed count every frame to determine if the enemies are all dead
                    {
                        currentWaveTeam2++;//if so, increment the wave and reset the spawned bool
                        waveSpawnedTeam2 = false;
                    }
                }
                EscapeCheck(1);
                EscapeCheck(2);
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        if (team1deployed.Count > 0)
        {
            deployCS(1);
            //for (int i = 0; i < team1deployed.Count; i++)
            //{
            //    team1deployed[i].GetComponent<AIUnit>().calledUpdate();
            //}
        }
        if (team2deployed.Count > 0)
        {
            deployCS(2);
            //for (int i = 0; i < team2deployed.Count; i++)
            //{
            //    team2deployed[i].GetComponent<AIUnit>().calledUpdate();
            //}
        }
        if (team3deployed.Count > 0)
        {
            deployCS(3);
            //for (int i = 0; i < team3deployed.Count; i++)
            //{
            //    team3deployed[i].GetComponent<AIUnit>().calledUpdate();
            //}
        }
    }

    void EscapeCheck(int teamNumber)
    {
        switch (teamNumber)
        {
            case 1:
                for (int i = 0; i < team1deployed.Count; i++)
                {
                    if (Vector3.Distance(team1deployed[i].transform.position, Vector3.zero) > settings.mapRadius * 1.25f)
                    {
                        Vector3 newPos = team1deployed[i].transform.position;
                        if (getOnScreenPoint(out newPos))
                        {
                            team1deployed[i].GetComponent<AIUnit>().WarpIn(newPos);
                        }
                    }
                }
                break;
            case 2:
                for (int i = 0; i < team2deployed.Count; i++)
                {
                    if (Vector3.Distance(team2deployed[i].transform.position, Vector3.zero) > settings.mapRadius * 1.25f)
                    {
                        Vector3 newPos = team2deployed[i].transform.position;
                        if (getOnScreenPoint(out newPos))
                        {
                            team2deployed[i].GetComponent<AIUnit>().WarpIn(newPos);
                        }
                    }
                }
                break;
            case 3:
                for (int i = 0; i < team3deployed.Count; i++)
                {
                    if (Vector3.Distance(team3deployed[i].transform.position, Vector3.zero) > settings.mapRadius * 1.25f)
                    {
                        Vector3 newPos = team3deployed[i].transform.position;
                        if (getOnScreenPoint(out newPos))
                        {
                            team3deployed[i].GetComponent<AIUnit>().WarpIn(newPos);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    void deployCS(int teamNumber)
    {
        int numShips = 0;
        switch (teamNumber)
        {
            case 1:
                numShips = team1deployed.Count;
                break;
            case 2:
                numShips = team2deployed.Count;
                break;
            case 3:
                numShips = team3deployed.Count;
                break;
            default:
                break;
        }
        var shipData = new ShipData[numShips];
        switch (teamNumber)
        {
            case 1:
                for (int i = 0; i < numShips; i++)
                {
                    shipData[i].position = team1deployed[i].transform.position;
                    shipData[i].direction = team1deployed[i].transform.forward;
                }
                break;
            case 2:
                for (int i = 0; i < numShips; i++)
                {
                    shipData[i].position = team2deployed[i].transform.position;
                    shipData[i].direction = team2deployed[i].transform.forward;
                }
                break;
            case 3:
                for (int i = 0; i < numShips; i++)
                {
                    shipData[i].position = team3deployed[i].transform.position;
                    shipData[i].direction = team3deployed[i].transform.forward;
                }
                break;
            default:
                break;
        }
        

        var shipDataBuffer = new ComputeBuffer(numShips, ShipData.Size);
        shipDataBuffer.SetData(shipData);

        compute.SetBuffer(0, "ships", shipDataBuffer);
        compute.SetInt("numShips", numShips);
        compute.SetFloat("viewRadius", SeparationVisionRadius);
        compute.SetFloat("avoidRadius", SeparationAvoidRadius);

        int threadGroups = Mathf.CeilToInt(numShips / (float)TGS);
        compute.Dispatch(0, TGS, 1, 1);

        shipDataBuffer.GetData(shipData);

        switch (teamNumber)
        {
            case 1:
                for (int i = 0; i < numShips; i++)
                {
                    team1deployed[i].GetComponent<AIUnit>().setNeighborShipData(shipData[i].neighbors, shipData[i].avoidHeading);
                }
                break;
            case 2:
                for (int i = 0; i < numShips; i++)
                {
                    team2deployed[i].GetComponent<AIUnit>().setNeighborShipData(shipData[i].neighbors, shipData[i].avoidHeading);
                }
                break;
            case 3:
                for (int i = 0; i < numShips; i++)
                {
                    team3deployed[i].GetComponent<AIUnit>().setNeighborShipData(shipData[i].neighbors, shipData[i].avoidHeading);
                }
                break;
            default:
                break;
        }
        
        shipDataBuffer.Release();
    }

    public void killWave()
    {
        for (int i = 0; i < team1deployed.Count; i++)
        {
            team1deployed[i].GetComponent<AIUnit>().onDeath();
        }
    }

    bool spawnAIUnit(int teamNumber)
    {
        Vector3 spawnPos = Vector3.zero;
        if (!getOnScreenPoint(out spawnPos))
        {
            return false;
        }
        if (debug_NoGameManager)
        {
            if (Vector3.Distance(spawnPos, Vector3.zero) > debugMapRadius)
            {

                return false;
            }
        }
        else
        {
            if (Vector3.Distance(spawnPos, Vector3.zero) > settings.mapRadius)
            {
                return false;
            }
        }
        if (spawnPos != Vector3.zero)
        {
            switch (teamNumber)
            {
                case 1:
                    GameObject storedShip = team1Stored.Dequeue();
                    storedShip.SetActive(true);
                    AIUnit script = storedShip.GetComponent<AIUnit>();
                    script.WarpIn(spawnPos);
                    script.InitAI(teamNumber, teamColors[0]);
                    team1deployed.Add(storedShip);
                    break;
                case 2:
                    GameObject storedShip2 = team2Stored.Dequeue();
                    storedShip2.SetActive(true);
                    AIUnit script2 = storedShip2.GetComponent<AIUnit>();
                    script2.WarpIn(spawnPos);
                    script2.InitAI(teamNumber, teamColors[1]);
                    team2deployed.Add(storedShip2);
                    break;
                case 3:
                    GameObject storedShip3 = team3Stored.Dequeue();
                    storedShip3.SetActive(true);
                    AIUnit script3 = storedShip3.GetComponent<AIUnit>();
                    script3.WarpIn(spawnPos);
                    script3.InitAI(teamNumber, teamColors[2]);
                    team3deployed.Add(storedShip3);
                    break;
                default:
                    return false;
            }
        }
        return true;
    }

    bool getOnScreenPoint(out Vector3 newPos)
    {
        Vector3 screenPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Screen.width),
            Random.Range(0, Screen.height), Camera.main.nearClipPlane + spawnDistanceFromCamera));

        if (Physics.SphereCast(Camera.main.ScreenPointToRay(screenPosition), spawnRadiusFromObjects, spawnDistanceFromCamera, spawnAvoidLayer))
        {
            newPos = Vector3.zero;
            return false;
        }
        newPos = screenPosition;
        return true;
    }

    public void ReturnShipToPool(GameObject ship)
    {
        int teamNumber = ship.GetComponent<ITeamMember>().getTeamNumber();
        switch (teamNumber)
        {
            case 1:
                if (!team1Stored.Contains(ship))
                {
                    team1deployed.Remove(ship);
                    team1Stored.Enqueue(ship);
                    ship.SetActive(false);
                    ship.transform.position = Team1Container.position;
                }
                break;
            case 2:
                if (!team2Stored.Contains(ship))
                {
                    team2deployed.Remove(ship);
                    team2Stored.Enqueue(ship);
                    ship.SetActive(false);
                    ship.transform.position = Team2Container.position;
                }
                break;
            case 3:
                if (!team3Stored.Contains(ship))
                {
                    team3deployed.Remove(ship);
                    team3Stored.Enqueue(ship);
                    ship.SetActive(false);
                    ship.transform.position = Team3Container.position;
                }
                break;
            default:
                break;
        }
    }

    public void updateHUDData(out HUDWaveData dat)
    {
        dat.currentWave = currentWaveTeam1 + 1;
        dat.currentWaveRemainingEnemies = team1deployed.Count;
        dat.currentWaveTotalEnemies = waves[currentWaveTeam1];
        dat.totalWaves = waves.Count;
    }

    #region initializers
    void InitializeSoloVsWavesGame()
    {
        for (int i = 0; i < settings.numOfWaves; i++)
        {
            int numEnemies = UnityEngine.Random.Range(difficultyWaveSpawnMin[(int)difficulty], difficultyWaveSpawnMax[(int)difficulty]);
            
            waves.Add(numEnemies);
        }

        int biggestWave = 0;
        for (int i = 0; i < waves.Count; i++)
        {
            if (waves[i] > biggestWave)
            {
                biggestWave = waves[i];
            }
        }

        for (int i = 0; i < biggestWave; i++)
        {
            int shipIndex = 0;
            float rarityChoice = Random.Range(0, 1f);
            for (int f = 0; f < AIprefabs.Count; f++)
            {
                if (AIprefabs[f].GetComponent<AIUnit>().stats.rarity >= rarityChoice)
                {
                    shipIndex = f;
                }
            }
            GameObject temp = Instantiate(AIprefabs[shipIndex], Team1Container);
            temp.transform.position = Team1Container.position;
            temp.SetActive(false);
            team1Stored.Enqueue(temp);
        }
    }

    void InitializeSmallTeamVsWavesGame()
    {
        InitializeSoloVsWavesGame();
        InitializeTeam(2, allySmallTeamSize);
    }

    void InitializeTeamVsTeamGame()
    {
        if (globalTeamSize == 0)
        {
            globalTeamSize = UnityEngine.Random.Range(difficultyTeamSpawnMin[(int)difficulty], difficultyTeamSpawnMax[(int)difficulty]);
        }
        InitializeTeam(1, globalTeamSize);
        InitializeTeam(2, globalTeamSize);
    }

    void InitializeTeam(int teamNumber, int teamSize)
    {
        for (int i = 0; i < teamSize; i++)
        {
            int shipIndex = 0;
            float rarityChoice = Random.Range(0, 1f);
            for (int f = 0; f < AIprefabs.Count; f++)
            {
                if (AIprefabs[f].GetComponent<AIUnit>().stats.rarity >= rarityChoice)
                {
                    shipIndex = f;
                }
            }
            switch (teamNumber)
            {
                case 1:
                    GameObject temp = Instantiate(AIprefabs[shipIndex], Team1Container);
                    temp.transform.position = Team1Container.position;
                    temp.SetActive(false);
                    team1Stored.Enqueue(temp);
                    break;
                case 2:
                    GameObject temp2 = Instantiate(AIprefabs[shipIndex], Team2Container);
                    temp2.transform.position = Team2Container.position;
                    temp2.SetActive(false);
                    team2Stored.Enqueue(temp2);
                    break;
                case 3:
                    GameObject temp3 = Instantiate(AIprefabs[shipIndex], Team2Container);
                    temp3.transform.position = Team2Container.position;
                    temp3.SetActive(false);
                    team2Stored.Enqueue(temp3);
                    break;
                default:
                    break;
            }
        }
    }

    void initSizeLists()
    {
        difficultyWaveSpawnMin = new List<int>();
        difficultyWaveSpawnMax = new List<int>();

        difficultyTeamSpawnMin = new List<int>();
        difficultyTeamSpawnMax = new List<int>();

        difficultyWaveSpawnMin.Add(easyWaveMin);
        difficultyWaveSpawnMin.Add(mediumWaveMin);
        difficultyWaveSpawnMin.Add(hardWaveMin);
        difficultyWaveSpawnMin.Add(extremeWaveMin);

        difficultyWaveSpawnMax.Add(easyWaveMax);
        difficultyWaveSpawnMax.Add(mediumWaveMax);
        difficultyWaveSpawnMax.Add(hardWaveMax);
        difficultyWaveSpawnMax.Add(extremeWaveMax);

        difficultyTeamSpawnMin.Add(easyWaveMin);
        difficultyTeamSpawnMin.Add(mediumWaveMin);
        difficultyTeamSpawnMin.Add(hardWaveMin);
        difficultyTeamSpawnMin.Add(extremeWaveMin);

        difficultyTeamSpawnMax.Add(easyWaveMax);
        difficultyTeamSpawnMax.Add(mediumWaveMax);
        difficultyTeamSpawnMax.Add(hardWaveMax);
        difficultyTeamSpawnMax.Add(extremeWaveMax);
    }
    #endregion
}
