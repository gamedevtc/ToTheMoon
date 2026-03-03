using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    //SerializeFields
    [SerializeField] GameObject AIShipPrefab;
    [SerializeField] public ComputeShader compute;

    //Object Ship Pool
    private Queue<GameObject> enemyShipPool;
    public List<GameObject> deployedShips;

    //Data from GameManager
    private int totalWaveCount = 0;
    private List<int> enemyCounts;
    private float mapRadii = 0;

    //Running data
    AIHelper settings;
    const int TGS = 1024;
    private int currWaveIndex = -1;
    private int currWaveRemaining = 0;
    private bool currWaveSpawned = false;

    LevelInfo.LevelSettings levelData;

    private int currentShipCount = 0;

    [SerializeField] int easyMin = 2;
    [SerializeField] int easyMax = 5;
    [SerializeField] int mediumMin = 4;
    [SerializeField] int mediumMax = 9;
    [SerializeField] int hardMin = 8;
    [SerializeField] int hardMax = 12;
    [SerializeField] int extremeMin = 12;
    [SerializeField] int extremeMax = 25;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        settings = FindObjectOfType<AIHelper>();

        //get Level Data from GameManager
        levelData = new LevelInfo.LevelSettings();
        GameManagerBase.Instance.getLevel(out levelData);
        totalWaveCount = levelData.numOfWaves;
        mapRadii = levelData.mapRadius;
        //Oh yeah, lists have to be Deep copied
        enemyCounts = new List<int>();
        switch (levelData.difficulty)
        {
            case LevelInfo.levelDifficulty.Easy:
                for (int i = 0; i < levelData.numOfWaves; i++)
                {
                    enemyCounts.Add(Random.Range(easyMin, easyMax));
                }
                break;
            case LevelInfo.levelDifficulty.Medium:
                for (int i = 0; i < levelData.numOfWaves; i++)
                {
                    enemyCounts.Add(Random.Range(mediumMin, mediumMax));
                }
                break;
            case LevelInfo.levelDifficulty.Hard:
                for (int i = 0; i < levelData.numOfWaves; i++)
                {
                    enemyCounts.Add(Random.Range(hardMin, hardMax));
                }
                break;
            case LevelInfo.levelDifficulty.Extreme:
                for (int i = 0; i < levelData.numOfWaves; i++)
                {
                    enemyCounts.Add(Random.Range(extremeMin, extremeMax));
                }
                break;
        }
        deployedShips = new List<GameObject>();
        //Double check Wave Count matches
        if (totalWaveCount != enemyCounts.Count)
        {
            Debug.LogWarning("LevelData corrupt: waveCount does not match entries in enemyCounts.");
        }
        currentShipCount = enemyCounts.Count;
        //Initialize Object pool
        enemyShipPool = new Queue<GameObject>();

        //find necessary max pool size
        int biggestWave = 0;
        for (int i = 0; i < totalWaveCount; i++)
        {
            if (biggestWave < enemyCounts[i])
            {
                biggestWave = enemyCounts[i];
            }
        }

        //instantiate as many inactive ships in pool as biggest wave will need
        for (int i = 0; i < biggestWave; i++)
        {
            if (GameManagerBase.Instance.isMulti() == true)
            {
                GameObject newShip = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "M_AI Fighter 2"), Vector3.zero, Quaternion.identity);

                newShip.SetActive(false);
                enemyShipPool.Enqueue(newShip);
            }
            else
            {
                GameObject newShip = Instantiate<GameObject>(AIShipPrefab);
                newShip.SetActive(false);
                newShip.transform.position = new Vector3(0, 0, 0);
                newShip.GetComponent<AIController>().setWaveManager(this);
                enemyShipPool.Enqueue(newShip);
            }
        }

        //put Player ship in Deployed Ship Pool for avoiding Collision
        //deployedShips.Add(gameManager.playerScript.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Lose || GameManagerBase.Instance.getState() == GameManagerBase.gameState.Win || GameManagerBase.Instance.getState() == GameManagerBase.gameState.Pause)
        {
            return;
        }
       
        //Check if all current wave enemies are dead (1st pass will always enter as currWaveRemaining is initialized to 0
        if (currWaveRemaining == 0)
        {
            //Increase current Wave counter
            currWaveIndex++;
            if (currWaveIndex < enemyCounts.Count)
            {
                //update remaining count to new wave Count and remember they haven't spawned
                currWaveRemaining = enemyCounts[currWaveIndex];
                currWaveSpawned = false;
            }
            else //This will only enter after completing final wave
            {
                currWaveRemaining = -1;
            }
        }

        //This generates data for every ship needed to spawn in the wave and spawns it
        if (!currWaveSpawned)
        {
            SpawnWave();
            currWaveSpawned = true;
        }

        if (deployedShips.Count > 1)
        {
            deployCS();
            for (int i = 0; i < deployedShips.Count; i++)
            {
                EscapeCheck(deployedShips[i]);
            }
        }
        
    }

    void SpawnWave()
    {
        for (int i = 0; i < currWaveRemaining; i++)
        {
            //Pull a ship from the pool and initialize it to a random location
            Vector3 newPos;
            newPos.x = Random.Range(-mapRadii, mapRadii);
            newPos.y = Random.Range(-mapRadii, mapRadii);
            newPos.z = Random.Range(-mapRadii, mapRadii);
            AIController.Behavior behav = AIController.Behavior.Aggressive;
            //For now set an even number of behaviors
            int behavNum = i % 3;
            switch (behavNum)
            {
                case 0:
                    behav = AIController.Behavior.Aggressive;
                    break;
                case 1:
                    behav = AIController.Behavior.Defensive;
                    break;
                case 2:
                    behav = AIController.Behavior.Cover;
                    break;
            }
            //Send this data to spawn function
            SpawnAIFromPool(newPos, behav);
        }
    }

    //Pulls an AI from the object pool and spawns at given location with given behavior
    void SpawnAIFromPool(Vector3 pos, AIController.Behavior behav)
    {
        GameObject ship = enemyShipPool.Peek();
        ship.transform.position = pos;
        ship.GetComponent<AIController>().SetBehavior(behav);
        ship.GetComponent<AIController>().setMaxHealth(behav);
        ship.GetComponent<AIController>().FillHealth();
        ship.SetActive(true);
        deployedShips.Add(ship);
        enemyShipPool.Dequeue();
    }

    //Hides ship and returns it to object pool
    public void ReturnShipToPool(GameObject ship)
    {
        ship.SetActive(false);
        ship.transform.position = new Vector3(0, 0, 0);
        deployedShips.Remove(ship);
        enemyShipPool.Enqueue(ship);
        currWaveRemaining--;
    }

    void calculateAvoidance()
    {
        //Other Ship avoidances: (This may need moved to compute shader if perfomance issues arise)
        for (int i = 0; i < deployedShips.Count; i++)
        {
            if (deployedShips[i].tag != "Player")
            {
                //Check to make sure ships havent escaped
                EscapeCheck(deployedShips[i]);

                int nearby = 0;

                AIController currShip = deployedShips[i].GetComponent<AIController>();
                Vector3 currShipPos = currShip.transform.position;
                Vector3 avgAvoidNeighborHeading = Vector3.zero;

                for (int f = 0; f < deployedShips.Count; f++)
                {
                    if (i == f) //skip calculating a ship with itself
                    {
                        continue;
                    }//Add functionality to remove followers/etc from calculation here

                    Vector3 otherShipPos = deployedShips[i].transform.position;
                    //float distance = Vector3.Distance(otherShipPos, currShipPos);
                    Vector3 diff = otherShipPos - currShipPos;

                    if (diff.magnitude < settings.AvoidRadius)//distance < settings.AvoidRadius)
                    {
                        avgAvoidNeighborHeading += diff.normalized / diff.magnitude;
                        nearby++;
                    }
                }

                avgAvoidNeighborHeading /= nearby;

                currShip.setNeighborShipData(nearby, avgAvoidNeighborHeading);
            }
        }
    }

    void deployCS()
    {
        int numShips = deployedShips.Count;
        var shipData = new ShipData[numShips];

        for (int i = 0; i < numShips; i++)
        {
            shipData[i].position = deployedShips[i].transform.position;
            shipData[i].direction = deployedShips[i].transform.forward;
        }

        var shipDataBuffer = new ComputeBuffer(numShips, ShipData.Size);
        shipDataBuffer.SetData(shipData);

        compute.SetBuffer(0, "ships", shipDataBuffer);
        compute.SetInt("numShips", numShips);
        compute.SetFloat("viewRadius", settings.NeighborVisionRadius);
        compute.SetFloat("avoidRadius", settings.AvoidRadius);

        int threadGroups = Mathf.CeilToInt(numShips / (float)TGS);
        compute.Dispatch(0, TGS, 1, 1);

        shipDataBuffer.GetData(shipData);

        for (int i = 0; i < numShips; i++)
        {
            deployedShips[i].GetComponent<AIController>().setNeighborShipData(shipData[i].neighbors, shipData[i].avoidHeading);
        }

        shipDataBuffer.Release();
    }


    void EscapeCheck(GameObject ship)
    {
        float distanceFromMapCenter = Vector3.Distance(Vector3.zero, ship.transform.position);
        if (distanceFromMapCenter > mapRadii * 3)
        {
            ship.transform.position = new Vector3(0, 0, 0);
        }
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

    public int getEnemyCount()
    {
        return currWaveRemaining;
    }

    public int getWaveMax()
    {
        return enemyCounts[currWaveIndex];
    }

    public int getCurrWave()
    {
        return currWaveIndex + 1;
    }
}
