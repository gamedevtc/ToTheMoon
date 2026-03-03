using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance;

    [Header("Set These in level")]
    [SerializeField] Volume volume;

    [Header("Stuff this gets from GameManager (Debug)")]
    [SerializeField] bool debug;
    [SerializeField] int mapRadius;
    [SerializeField] int asteroidCount;
    [SerializeField] int asteroidMinScale;
    [SerializeField] int asteroidMaxScale;
    [SerializeField] int asteroidMinGap;
    [SerializeField] bool fluffyAsteroidBounds;
    [SerializeField] int fluffyFactor;
    [SerializeField] bool spawnOutsideAsteroids;
    [SerializeField] LevelInfo.levelBackground backgroundSettings;
    [SerializeField] LevelInfo.largeObject backgroundObject;
    [SerializeField] int backgroundObjectSpawnPos;

    [Header("Stuff this script finds itself (Debug)")]
    [SerializeField] Transform[] bigObjectSpawnPos;
    [SerializeField] List<Transform> bigObjectAnchors;
    [SerializeField] Transform[] playerSpawnPos;
    [SerializeField] List<Transform> playerSpawnAnchors = new List<Transform>();
    [SerializeField] List<int> takenSpawnPoints;

    [Header("These will be set in the prefab")]
    [SerializeField] float playerSpawnDistance = 1.2f;
    [SerializeField] float bigObjectSpawnMinModifier = 2;
    [SerializeField] float bigObjectSpawnMaxModifier = 3;
    [SerializeField] int maxAsteroidPlacementAttempts = 15;
    [SerializeField] int AsteroidsSuccessFullyCreated = 0;
    [SerializeField] int maxSpawnAttempts = 15;
    [SerializeField] List<GameObject> AsteroidPrefabs;
    [SerializeField] List<VolumeProfile> VolumeProfiles;
    [SerializeField] List<GameObject> DirectionalLights;
    [SerializeField] List<GameObject> LargeObjects;
    [SerializeField] GameObject outsideBarrier;
    [SerializeField] List<GameObject> outsideBarrierRings;
    [SerializeField] GameObject bigObjectContainer;
    [SerializeField] GameObject playerSpawnContainer;
    [SerializeField] GameObject asteroidContainer;
    [SerializeField] GameObject directionalLightContainer;
    //Defaults
    [SerializeField] VolumeProfile defaultVolumeProfile;
    [SerializeField] GameObject defaultDirectionalLight;
    [SerializeField] GameObject defaultLargeObject;

    [SerializeField] PhotonView PV;

    //non-serializable
    LevelInfo.LevelSettings levelData;

    bool run = false;
    float fluffyFloat = 1;

    private void Awake()
    {
        Instance = this;
        if ((GameManagerBase.Instance.isMulti() && PhotonNetwork.IsMasterClient) || !GameManagerBase.Instance.isMulti())
        {
            run = true;
        }
        levelData = new LevelInfo.LevelSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameManagerBase.Instance.isMulti())
        {
            PV = GetComponent<PhotonView>();
        }

        initializePlayerSpawns();
        randomizeBigObjectSpawns();

        if (!debug)
        {
            GameManagerBase.Instance.getLevel(out levelData);
            mapRadius = levelData.mapRadius;
            asteroidCount = levelData.asteroidCount;
            asteroidMinScale = levelData.asteroidMinScale;
            asteroidMaxScale = levelData.asteroidMaxScale;
            asteroidMinGap = levelData.asteroidMinGap;
            fluffyAsteroidBounds = levelData.fluffyAsteroidBounds;
            fluffyFactor = levelData.fluffyFactor;
            backgroundSettings = levelData.backgroundSettings;
            backgroundObject = levelData.backgroundObject;

            if (GameManagerBase.Instance.isMulti())
            {
                mapRadius = 1000;
            }
            fluffyFloat += ((float)fluffyFactor / 100.0f);

            //if (GameManagerBase.Instance.isMulti())
            //{
            //    Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            //    backgroundSettings = (LevelInfo.levelBackground)hash["Skybox"];
            //    Debug.Log("got backgroudn settings from hashtable: " + backgroundSettings.ToString());
            //}
        }

        setSkyBox(backgroundSettings);
        
        setDirectionalLight(backgroundSettings);

        //Stop here if multiplayer and not hosting
        if (!run)
        {
            return;
        }


        //Spawn Big object and randomize positions
        setBigObject(backgroundObject);   

        //modify outside barrier scale
        setOutsideBarrier();

        //Spawn Asteroids
        setAsteroids();

        GameManagerBase.Instance.setState(GameManagerBase.gameState.Running);
    }

    //directional light and skybox functions run on every client no matter single or multiplayer
    void setDirectionalLight(LevelInfo.levelBackground settings)
    {

        //if chosen option is default or count, or if the chosen setting is outside the range of the list of objects, spawn the default
        if (settings == LevelInfo.levelBackground.Default || settings == LevelInfo.levelBackground.COUNT || (int)settings > DirectionalLights.Count || (int)settings > VolumeProfiles.Count)
        {

            Instantiate(defaultDirectionalLight, directionalLightContainer.transform);
        }
        else //everything is good, spawn the correct light
        {

            Instantiate(DirectionalLights[(int)settings], directionalLightContainer.transform);
        }
    }
    void setSkyBox(LevelInfo.levelBackground settings)
    {

        //if chosen option is default or count, or if the chosen setting is outside the range of the list of objects, spawn the default
        if (settings == LevelInfo.levelBackground.Default || settings == LevelInfo.levelBackground.COUNT || (int)settings > DirectionalLights.Count || (int)settings > VolumeProfiles.Count)
        {

            volume.profile = defaultVolumeProfile;
        }
        else //everything is good, assign the correct volume profile
        {

            volume.profile = VolumeProfiles[(int)settings];
        }
    }

    //big objects and asteroids only run on host client or singleplayer
    void setBigObject(LevelInfo.largeObject settings)
    {
        int randSpot = Random.Range(0, bigObjectAnchors.Count);
        GameObject bigObj;
        //if chosen option is higher than count, or if the chosen setting is outside the range of the list of objects, spawn the default
        if (settings >= LevelInfo.largeObject.COUNT || (int)settings > LargeObjects.Count)
        {
            switch (settings)
            {
                case LevelInfo.largeObject.Random:
                    int rand = Random.Range(0, LargeObjects.Count);
                    if (GameManagerBase.Instance.isMulti())
                    {
                        bigObj = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", LargeObjects[rand].name), bigObjectAnchors[randSpot].position, Quaternion.identity);
                    }
                    else
                    {
                        bigObj = Instantiate(LargeObjects[rand], bigObjectAnchors[randSpot]);
                        bigObj.transform.position = bigObjectAnchors[randSpot].position;
                    }
                    
                    break;
                case LevelInfo.largeObject.None:
                    bigObj = null;
                    break;
                case LevelInfo.largeObject.COUNT:
                    if (GameManagerBase.Instance.isMulti())
                    {
                        bigObj = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", defaultLargeObject.name), bigObjectAnchors[randSpot].position, Quaternion.identity);
                    }
                    else
                    {
                        bigObj = Instantiate(defaultLargeObject, bigObjectAnchors[randSpot]);
                        bigObj.transform.position = bigObjectAnchors[randSpot].position;
                    }
                    break;
                case LevelInfo.largeObject.Default:
                    if (GameManagerBase.Instance.isMulti())
                    {
                        bigObj = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", defaultLargeObject.name), bigObjectAnchors[randSpot].position, Quaternion.identity);
                    }
                    else
                    {
                        bigObj = Instantiate(defaultLargeObject, bigObjectAnchors[randSpot]);
                        bigObj.transform.position = bigObjectAnchors[randSpot].position;
                    }
                    break;
            }
        }
        else
        {
            if (GameManagerBase.Instance.isMulti())
            {
                bigObj = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", LargeObjects[(int)settings].name), bigObjectAnchors[randSpot].position, Quaternion.identity);
            }
            else
            {
                bigObj = Instantiate(LargeObjects[(int)settings], bigObjectAnchors[randSpot]);
                bigObj.transform.position = bigObjectAnchors[randSpot].position;
            }
        }
    }

    void randomizeBigObjectSpawns()
    {
        bigObjectSpawnPos = bigObjectContainer.GetComponentsInChildren<Transform>();
        for (int i = 1; i < bigObjectSpawnPos.Length; i++)
        {
            bigObjectAnchors.Add(bigObjectSpawnPos[i]);
        }
        for (int i = 0; i < bigObjectAnchors.Count; i++)
        {
            float doubleMapRadius = mapRadius * 2;
            float xPlus = Random.Range((doubleMapRadius * bigObjectSpawnMinModifier), (doubleMapRadius * bigObjectSpawnMaxModifier));
            float xMinus = Random.Range(-(doubleMapRadius * bigObjectSpawnMinModifier), -(doubleMapRadius * bigObjectSpawnMaxModifier));

            float yPlus = Random.Range((doubleMapRadius * bigObjectSpawnMinModifier), (doubleMapRadius * bigObjectSpawnMaxModifier));
            float yMinus = Random.Range(-(doubleMapRadius * bigObjectSpawnMinModifier), -(doubleMapRadius * bigObjectSpawnMaxModifier));

            float zPlus = Random.Range((doubleMapRadius * bigObjectSpawnMinModifier), (doubleMapRadius * bigObjectSpawnMaxModifier));
            float zMinus = Random.Range(-(doubleMapRadius * bigObjectSpawnMinModifier), -(doubleMapRadius * bigObjectSpawnMaxModifier));

            int xRand = Random.Range(0, 2);
            int yRand = Random.Range(0, 2);
            int zRand = Random.Range(0, 2);

            float xFinal = 0;
            float yFinal = 0;
            float zFinal = 0;

            if (xRand == 0) { xFinal = xMinus; } else { xFinal = xPlus; }
            if (yRand == 0) { yFinal = yMinus; } else { yFinal = yPlus; }
            if (zRand == 0) { zFinal = zMinus; } else { zFinal = zPlus; }

            bigObjectAnchors[i].position = new Vector3(xFinal, yFinal, zFinal);
        }
    }

    void initializePlayerSpawns()
    {
        playerSpawnPos = playerSpawnContainer.GetComponentsInChildren<Transform>();
        for (int i = 1; i < playerSpawnPos.Length; i++)
        {
            playerSpawnAnchors.Add(playerSpawnPos[i]);
        }
        float outsideMapRadius = mapRadius * playerSpawnDistance;
        float cir = 2 * Mathf.PI * outsideMapRadius;
        float angleChange = cir / (float)(playerSpawnAnchors.Count * playerSpawnAnchors.Count);
        float angle = 0;
        for (int i = 0; i < playerSpawnAnchors.Count; i++)
        {
            Vector2 posXY = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle))*outsideMapRadius;
            Vector3 newPos = new Vector3(posXY.x, 0, posXY.y);
            playerSpawnAnchors[i].position = newPos;
            playerSpawnAnchors[i].forward = (Vector3.zero - newPos).normalized;
            angle += angleChange;
        }
        for (int i = 0; i < playerSpawnAnchors.Count; i++)
        {
            takenSpawnPoints.Add(0);
        }
    }

    public float getMapRadius()
    {
        return mapRadius;
    }

    public List<Transform> getSpawnPoints()
    {
        return playerSpawnAnchors;
    }

    public Transform getOpenSpawn()
    {
        int spawnIndex = 0;
        bool foundSpot = false;
        int spawnAttempts = 0;
        while(true)
        {
            spawnIndex = Random.Range(0, playerSpawnAnchors.Count-1);
            if (takenSpawnPoints[spawnIndex] == 0)
            {
                takenSpawnPoints[spawnIndex] = 1;
                foundSpot = true;
                break;
            }
            spawnAttempts += 1;
            if (spawnAttempts >= maxSpawnAttempts)
            {
                break;
            }
            
        }
        if (!foundSpot)
        {
            for (int i = 0; i < takenSpawnPoints.Count; i++)
            {
                if (takenSpawnPoints[i] == 0)
                {
                    spawnIndex = i;
                    takenSpawnPoints[i] = 1;
                    break;
                }
            }
        }
        if (PV)
        {
            PV.RPC("RPC_ClaimSpawn", RpcTarget.All, spawnIndex);
        }
        return playerSpawnAnchors[spawnIndex];
    }

    [PunRPC]
    void RPC_ClaimSpawn(int index)
    {
        takenSpawnPoints[index] = 1;
    }

    void setOutsideBarrier()
    {
        Vector3 barrierSize = new Vector3(mapRadius * 2, mapRadius * 2, mapRadius * 2);
        Vector3 ringSize = new Vector3(mapRadius, mapRadius, mapRadius);
        outsideBarrier.transform.localScale = barrierSize;
        for (int i = 0; i < outsideBarrierRings.Count; i++)
        {
            outsideBarrierRings[i].transform.localScale = ringSize;

        }
        outsideBarrier.GetComponent<InvertMeshCollider>().Invert();
    }

    void setAsteroids()
    {
        List<Transform> createdAsteroids = new List<Transform>(playerSpawnAnchors);
        //create asteroids for each partition
        for (int i = 0; i < asteroidCount; i++)
        {
            int scaleX = Random.Range(asteroidMinScale, asteroidMaxScale);
            int scaleY = Random.Range(scaleX - 10, scaleX + 10);
            int scaleZ = Random.Range(scaleX - 10, scaleX + 10);
            Vector3 scaleVec = new Vector3(scaleX, scaleY, scaleZ);
            Vector3 posVec = Vector3.zero;
            int attempts = 0;
            {//Randomize and distance check method using while loop (could get stuck)
                while (true)
                {
                    bool foundPos = true;
                    float Xposition = 0;
                    float Yposition = 0;
                    float Zposition = 0;
                    if (fluffyAsteroidBounds)
                    {
                        Xposition = Random.Range(-mapRadius*fluffyFloat, mapRadius*fluffyFloat);
                        Yposition = Random.Range(-mapRadius*fluffyFloat, mapRadius*fluffyFloat);
                        Zposition = Random.Range(-mapRadius*fluffyFloat, mapRadius*fluffyFloat);
                    }
                    else
                    {
                        Xposition = Random.Range(-mapRadius, mapRadius);
                        Yposition = Random.Range(-mapRadius, mapRadius);
                        Zposition = Random.Range(-mapRadius, mapRadius);
                    }
                    
                    Vector3 randPos = new Vector3(Xposition, Yposition, Zposition);

                    for (int f = 0; f < createdAsteroids.Count; f++)
                    {
                        Transform currAst = createdAsteroids[f];
                        float dist = Vector3.Distance(randPos, currAst.position);
                        float Xdist = dist - scaleX - currAst.localScale.x;
                        float Ydist = dist - scaleY - currAst.localScale.y;
                        float Zdist = dist - scaleZ - currAst.localScale.z;
                        if (Xdist < asteroidMinGap || Ydist < asteroidMinGap || Zdist < asteroidMinGap)
                        {
                            foundPos = false;
                            attempts++;
                            break;
                        }
                    }
                    if (foundPos)
                    {
                        posVec = randPos;
                        break;
                    }
                    if (attempts >= maxAsteroidPlacementAttempts)
                    {
                        break;
                    }
                }
            }

            if (attempts < maxAsteroidPlacementAttempts)
            {
                int asteroidIndex = Random.Range(0, AsteroidPrefabs.Count);
                GameObject aster;
                if (GameManagerBase.Instance.isMulti())
                {
                    aster = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", AsteroidPrefabs[asteroidIndex].name), posVec, Quaternion.identity);
                }
                else
                {
                    aster = Instantiate(AsteroidPrefabs[asteroidIndex], asteroidContainer.transform);
                    aster.transform.position = posVec;
                }
                aster.transform.localScale = scaleVec;
                createdAsteroids.Add(aster.transform);
                AsteroidsSuccessFullyCreated += 1;
            }
        }
    }

    void createSpatialPartition()
    {

    }

    public void OnEnable()
    {
        EventManager.getpoint += getOpenSpawn;
    }

    public void OnDisable()
    {
        EventManager.getpoint -= getOpenSpawn;
    }
}
