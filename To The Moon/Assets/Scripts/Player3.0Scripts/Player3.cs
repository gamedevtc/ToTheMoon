using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player3 : MonoBehaviour
{
    #region Enums
    public enum ShipColor
    {
        Red = 0,
        DarkBlue,
        LightBlue,
        Green,
        Orange,
        Purple,
        Black,
        Yellow,
        COUNT
    }
    public enum Body //-(ShipColor.COUNT)
    {
        Body1 = (ShipColor.COUNT),//8
        Body2,//9
        Body3,//10
        Body4,//11
        COUNT
    }
    public enum Primary //-(Body.COUNT)
    {
        FusionBlaster = (Body.COUNT),//12
        FusionBlasterMkII,//13
        PlasmaAccelerator,//14
        BurstLaser,//15
        COUNT
    }
    public enum Secondary //-(Primary.COUNT)
    {
        Missiles = (Primary.COUNT),//16
        ThermalMissiles,//17
        PositronBallista,//18
        PulseCannon,//19
        FusionBlasterHeavy,//20
        FusionBlasterMkIIHeavy,//21
        PlasmaAcceleratorHeavy,//22
        BurstLaserHeavy,//23
        COUNT
    }
    public enum Ultimate //-(Secondary.COUNT)
    {
        MineLauncher = (Secondary.COUNT),//24
        MissileSilo,//25
        DevastatorBeam,//26
        EMP,//27
        COUNT
    }
    #endregion

    public struct playerShipSettings
    {
        public ShipColor activeColor;
        public Body activeBody;
        public Primary activePrimary;
        public Secondary activeSecondary;
        public Ultimate activeUltimate;
    }

    public struct HUDData
    {
        public float currHealth;
        public float maxHealth;
        public float currBoost;
        public float maxBoost;
        public float currUlt;
        public float maxUlt;
        public bool cheatMenuIsOpen;
        public bool GodMode;
        public bool InfiniteBoost;
        public bool canEscape;
        public bool escaping;
        public float escapeTime;
    }

    #region Active Settings
    [Header("Active Settings")]
    [SerializeField] ShipColor activeColor;
    [SerializeField] Body activeBody;
    [SerializeField] Primary activePrimary;
    [SerializeField] Secondary activeSecondary;
    [SerializeField] Ultimate activeUltimate;
    [SerializeField] string playSceneName = "SampleScene";
    [SerializeField] float normSpeed;
    [SerializeField] float accSpeed;
    [SerializeField] float decSpeed;
    [SerializeField] float zRotationSpeed;
    [SerializeField] float handlingSpeed;
    [SerializeField] float maxHealth;
    [SerializeField] float maxBoost;
    [SerializeField] float boostStrength;
    [SerializeField] float boostDecreaseModifier;
    [SerializeField] bool inShop = false;
    [SerializeField] bool debug = false;
    [SerializeField] bool debugUseShipSettings = false;
    #endregion

    #region Multiplayer Variables
    [Header("Multiplayer")]
    [SerializeField] PhotonView mView;
    [SerializeField] Canvas mUI;
    [SerializeField] string mName;
    [SerializeField] AudioSource m_audioSource;
    #endregion

    #region Camera
    [Header("Camera Settings")]
    [SerializeField] Transform cameraPos;
    [SerializeField] Transform cameraPosUp;
    [SerializeField] Transform cameraPosDown;
    [SerializeField] Camera myCamera;
    [SerializeField] private float cameraSmoother = 4.0f;
    #endregion

    #region Stuff found automatically by script (Remove from serialization if working)
    [Header("Stuff this script finds (Debug)")]
    [SerializeField] Dictionary<int, bool> unlockedDictionary;
    [SerializeField] playerShipSettings activeSettings;
    [SerializeField] int credits;
    [SerializeField] Rigidbody rb;


    #endregion

    #region Timers and internal data (Only Serialized for debugging)
    [Header("Debug")]
    [SerializeField] float currSpeed;
    [SerializeField] float currHealth;
    [SerializeField] float currBoost;
    [SerializeField] float currBoostStrength;
    [SerializeField] bool paused = false;
    [SerializeField] Vector3 cachedVelocity;
    [SerializeField] bool chased = false;
    [SerializeField] bool boosting = false;
    [SerializeField] int invertYVal = -1;
    [SerializeField] int invertXVal = 1;
    [SerializeField] bool escaping = false;
    [SerializeField] float escapeTime = 30.0f;
    [SerializeField] float escapeTimeDef = 30.0f;
     //Shop
    [SerializeField] ShipColor shopColor;
    [SerializeField] Body shopBody;
    [SerializeField] Primary shopPrimary;
    [SerializeField] Secondary shopSecondary;
    [SerializeField] Ultimate shopUltimate;
    #endregion

    #region Controls
    [Header("Controls")]
    [SerializeField] bool ControllerEnabled = false;
    //Firing Weapons
    [SerializeField] KeyCode UltimateFireKey = KeyCode.E;
    [SerializeField] string PrimaryFireControl = "Fire1";
    [SerializeField] string SecondaryFireControl = "Fire2";
    [SerializeField] string UltimateFireControl = "Fire3";
    //WASD
    [SerializeField] KeyCode AccelerateKey = KeyCode.W;
    [SerializeField] KeyCode DecelerateKey = KeyCode.S;
    [SerializeField] KeyCode LeftKey = KeyCode.A;
    [SerializeField] KeyCode RightKey = KeyCode.D;
    [SerializeField] string AccelerateControl;
    [SerializeField] string DecelerateControl;
    [SerializeField] string LeftControl;
    [SerializeField] string RightControl;
    //Other Controls
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] KeyCode boostKey = KeyCode.LeftShift;
    [SerializeField] string pauseControl;
    [SerializeField] string boostControl;
    [SerializeField] bool invertYaxis = false;
    [SerializeField] bool invertXaxis = false;

    #endregion

    #region Cheat Variables
    [Header("Cheats/Debug keys")]
    [SerializeField] bool cheatMenuIsOpen = false;
    [SerializeField] bool GodMode = false;
    [SerializeField] bool InfiniteBoost = false;
    [SerializeField] bool canEscape = false;
    [SerializeField] KeyCode cheatMenuKey = KeyCode.BackQuote;
    [SerializeField] KeyCode godModeKey = KeyCode.Alpha1;
    [SerializeField] KeyCode infinBoostKey = KeyCode.Alpha2;
    [SerializeField] KeyCode skipWaveKey = KeyCode.Alpha3;
    [SerializeField] KeyCode damagePlayerKey = KeyCode.Alpha4;
    [SerializeField] KeyCode fillUltimateKey = KeyCode.Alpha5;
    [SerializeField] KeyCode GiveMoneyKey = KeyCode.Alpha6;
    [SerializeField] KeyCode disableEscapeCheck = KeyCode.Alpha7;
    //[SerializeField] KeyCode unusedCheatKey8 = KeyCode.Alpha8;
    //[SerializeField] KeyCode unusedCheatKey9 = KeyCode.Alpha9;
    #endregion

    #region Stuff Set in Prefab
    [Header("Stuff Set in Prefab")]
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] HealthBar HUDscript;
    [SerializeField] SaveData saveDataScript;
    [SerializeField] RectTransform crosshair;
    [SerializeField] GameObject AIChaser;
    [SerializeField] GameObject deathEffect;
    [SerializeField] Player3BodyHolder bodyHolder;
    [SerializeField] WeaponHolder primaryHolder;
    [SerializeField] WeaponHolder secondaryHolder;
    [SerializeField] WeaponHolder ultimateHolder;
    [SerializeField] List<Material> StarSparrowMaterials; 
    [SerializeField] List<GameObject> BodyObj;
    [SerializeField] List<GameObject> PrimaryObj;
    [SerializeField] List<GameObject> SecondaryObj;
    [SerializeField] List<GameObject> UltimateObj;
    
    #endregion

    #region extras added
    [Header("Extras added as ideas or for future use")]
    [SerializeField] float boostGainMultiplier = 1;
    #endregion


    private void Start()
    {
        if (GameManagerBase.Instance.isMulti() == true)
        {
            if (mView != null)
            {
                m_audioSource.gameObject.SetActive(false);
                crosshair.gameObject.SetActive(false);
                myCamera.gameObject.SetActive(false);
            }
        }

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        if (SceneManager.GetActiveScene().name == playSceneName || debug)
        {
            inShop = false;
        }
        else
        {
            HUDscript.gameObject.SetActive(false);
            crosshair.gameObject.SetActive(false);
        }

        if (!debugUseShipSettings)
        {
            //saveDataScript.getActiveSettings(out activeSettings);
            setBodyActive(activeSettings.activeBody);
            setPrimaryActive(activeSettings.activePrimary);
            setSecondaryActive(activeSettings.activeSecondary);
            setUltimateActive(activeSettings.activeUltimate);
            setActiveColor(activeSettings.activeColor);
        }
        else
        {
            setBodyActive(activeBody);
            setPrimaryActive(activePrimary);
            setSecondaryActive(activeSecondary);
            setUltimateActive(activeUltimate);
            setActiveColor(activeColor);
        }
        

        if ((SceneManager.GetActiveScene().name == playSceneName || debug))
        {
            GameManagerBase.Instance.setState(GameManagerBase.gameState.Running);
            Transform newPos = EventManager.getSpawnPoint();//THIS MAY NEED TO CHANGE IN MULTIPLAYER
            if (newPos != null)
            {
                this.transform.position = newPos.position;
                this.transform.rotation = newPos.rotation;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //add pause and unpause to event
        EventManager.pauseEvent += Pause;
        EventManager.unPauseEvent += unPause;

        currHealth = bodyHolder.getMaxHealth();
        currBoostStrength = 1;
        boostGainMultiplier = 1;
    }

    private void OnDisable()
    {
        //remove from event
        EventManager.pauseEvent -= Pause;
        EventManager.unPauseEvent -= unPause;
    }

    private void Update()
    {
        #region returnChecks
        if (mView != null)
        {
            if (!mView.IsMine)
            {
                return;
            }
        }
        if (inShop)
        {
            if (Input.GetKeyDown(cheatMenuKey))
            {
                cheatMenuIsOpen = !cheatMenuIsOpen;
            }
            if (cheatMenuIsOpen)
            {
                if (Input.GetKeyDown(GiveMoneyKey))
                {
                    setCredits(500);
                }
            }
            return;
        }
        if (paused)
        {
            if (Input.GetKeyDown(pauseKey) || (ControllerEnabled && Input.GetButton(pauseControl)))
            {
                EventManager.unPause();
            }
            return;
        }
        #endregion

        TimerTick();

        if (!canEscape)
        {
            EscapeCheck();

            if (escapeTime <= 0)
            {
                TakeDamage(bodyHolder.getMaxHealth());
            }
        }
        

        #region Controls
        if (Input.GetButton(PrimaryFireControl))
        {
            shootPrimary();
        }
        if (Input.GetButton(SecondaryFireControl))
        {
            shootSecondary();
        }
        if (Input.GetKeyDown(UltimateFireKey) || (ControllerEnabled && Input.GetButton(UltimateFireControl)))
        {
            shootUltimate();
        }
        if (Input.GetKey(boostKey) || (ControllerEnabled && Input.GetButton(boostControl)))
        {
            boosting = true;
            if (currBoost > 0)
            {
                currBoostStrength = bodyHolder.getBoostStrength();
                if (!InfiniteBoost)
                {
                    currBoost -= Time.deltaTime * bodyHolder.getBoostDecrease();
                }
            }
            else
            {
                currBoostStrength = 1;
            }
        }
        else
        {
            currBoostStrength = 1;
            boosting = false;
        }
        if (Input.GetKeyDown(pauseKey) || (ControllerEnabled && Input.GetButton(pauseControl)))
        {
            if (paused)
            {
                EventManager.unPause();
            }
            else
            {
                EventManager.pause();
            }
        }
        #endregion
    
        #region CheatControls
        {
            if (Input.GetKeyDown(cheatMenuKey))
            {
                cheatMenuIsOpen = !cheatMenuIsOpen;
            }
            if (cheatMenuIsOpen)
            {
                if (Input.GetKeyDown(godModeKey))
                {
                    GodMode = !GodMode;
                    if (GodMode)
                    {
                        currHealth = bodyHolder.getMaxHealth();
                    }
                }
                if (Input.GetKeyDown(infinBoostKey))
                {
                    InfiniteBoost = !InfiniteBoost;
                    currBoost = bodyHolder.getMaxBoost();
                }
                if (Input.GetKeyDown(skipWaveKey))
                {
                    //Skip Wave operation here
                }
                if (Input.GetKeyDown(damagePlayerKey))
                {
                    TakeDamage(bodyHolder.getMaxHealth() * 0.1f);
                }
                if (Input.GetKeyDown(fillUltimateKey))
                {
                    ultimateHolder.FillUlt();
                }
                if (Input.GetKeyDown(disableEscapeCheck))
                {
                    canEscape = !canEscape;
                    if (escaping == true)
                    {
                        escaping = false;
                        escapeTime = escapeTimeDef;
                    }
                }
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        #region returnChecks
        
        if (mView != null)
        {
            if (!mView.IsMine)
            {
                return;
            }
        }
        if (inShop)
        {
            return;
        }
        if (paused && !GameManagerBase.Instance.isMulti())
        {
            return;
        }
        #endregion

        #region Movement
        float speedMod = 0;
        if (Input.GetKey(AccelerateKey) || (ControllerEnabled && Input.GetButton(AccelerateControl)))
        {
            speedMod = Mathf.Lerp(currSpeed, bodyHolder.getAccSpeed() * currBoostStrength, Time.deltaTime * 3);
        }
        else if (Input.GetKey(DecelerateKey) || (ControllerEnabled && Input.GetButton(DecelerateControl)))
        {
            speedMod = Mathf.Lerp(currSpeed, -bodyHolder.getDecSpeed(), Time.deltaTime * 3);
        }
        else
        {
            speedMod = Mathf.Lerp(currSpeed, bodyHolder.getSpeed() * currBoostStrength, Time.deltaTime * 3);
        }
        currSpeed = speedMod;
        Vector3 moveDirection = new Vector3(0, 0, speedMod);
        moveDirection = transform.TransformDirection(moveDirection);
        rb.velocity = moveDirection;
        #endregion

        #region CameraUpdate
        float camDistance = Vector3.Distance(this.transform.position, myCamera.transform.position);
        cameraPos.position = Vector3.Lerp(cameraPosDown.position, cameraPosUp.position, camDistance * Time.deltaTime);

        myCamera.transform.position = Vector3.Lerp(myCamera.transform.position, cameraPos.position, Time.deltaTime * cameraSmoother);
        myCamera.transform.rotation = Quaternion.Lerp(myCamera.transform.rotation, cameraPos.rotation, Time.deltaTime * cameraSmoother);
        #endregion

        #region Rotation
        float rotationTemp = 0;
        if (Input.GetKey(LeftKey) || (ControllerEnabled && Input.GetButton(LeftControl)))
        {
            rotationTemp = bodyHolder.getZRotationSpeed();
        }
        else if (Input.GetKey(RightKey) || (ControllerEnabled && Input.GetButton(RightControl)))
        {
            rotationTemp = -bodyHolder.getZRotationSpeed();
        }

        if (invertYaxis)
        {
            invertYVal = 1;
        }
        else
        {
            invertYVal = -1;
        }
        if (invertXaxis)
        {
            invertXVal = -1;
        }
        else
        {
            invertXVal = 1;
        }

        transform.Rotate(invertYVal * Input.GetAxis("Mouse Y") * bodyHolder.getHandlingSpeed(), 0.0f, 0.0f);
        transform.Rotate(0.0f, invertXVal * Input.GetAxis("Mouse X") * bodyHolder.getHandlingSpeed(), 0.0f);
        transform.Rotate(0.0f, 0.0f, rotationTemp * bodyHolder.getZRotationSpeed());
        #endregion

        if (crosshair)
        {
            crosshair.position = myCamera.WorldToScreenPoint(transform.position + transform.forward * 100);
        }
    }
    void TimerTick()
    {
        if (currBoost <= bodyHolder.getMaxBoost() && !boosting)
        {
            currBoost += (Time.deltaTime * boostGainMultiplier);
        }
        else if (currBoost > bodyHolder.getMaxBoost())
        {
            currBoost = bodyHolder.getMaxBoost();
        }

        if (escaping == true)
        {
            escapeTime -= Time.deltaTime;
        }
        else
        {
            escapeTime = escapeTimeDef;
        }
    }

    void EscapeCheck()
    {
        LevelInfo.LevelSettings lev = new LevelInfo.LevelSettings();
        GameManagerBase.Instance.getLevel(out lev);
        float distance = Vector3.Distance(this.transform.position, Vector3.zero);
        if (distance > lev.mapRadius * 2.0f)
        {
            escaping = true;
        }
        else
        {
            escaping = false;
        }
    }

    public float GetHealth()
    {
        return currHealth;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(bodyHolder.getMaxHealth());
            other.GetComponent<AIController>().onDamage((int)bodyHolder.getMaxHealth(), this.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            TakeDamage(bodyHolder.getMaxHealth());
            other.GetComponent<Player3>().TakeDamage((int)bodyHolder.getMaxHealth());
        }

        if (other.CompareTag("Aster"))
        {
            TakeDamage(bodyHolder.getMaxHealth());
        }
    }

    public void updateHUD(out HUDData dat)
    {
        float currUltValue = ultimateHolder.getCurrUltValue();
        float ultChargeTime = ultimateHolder.getChargeTime();
        dat.currHealth = currHealth;
        dat.maxHealth = bodyHolder.getMaxHealth();
        dat.currBoost = currBoost;
        dat.maxBoost = bodyHolder.getMaxBoost();
        dat.currUlt = currUltValue;
        dat.maxUlt = ultChargeTime;
        dat.cheatMenuIsOpen = cheatMenuIsOpen;
        dat.GodMode = GodMode;
        dat.InfiniteBoost = InfiniteBoost;
        dat.canEscape = canEscape;
        dat.escaping = escaping;
        dat.escapeTime = escapeTime;
    }

    public void TakeDamage(float damage)
    {
        if (!GodMode)
        {
            currHealth -= damage;
        }
        if (!GodMode && currHealth <= 0)
        {
            OnDeath();
        }
    }
    public void OnDeath()
    {
        paused = true;
        Instantiate(deathEffect, transform.position, transform.rotation);
        deactivateAllBodies();
        deactivateAllPrimaries();
        deactivateAllSecondaries();
        deactivateAllUltimates();
        cachedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void OnWin(int reward)
    {
        GodMode = true;
        setCredits(reward);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        saveDataScript.saveData();
    }

    void shootPrimary()
    {
        primaryHolder.Shoot(this.gameObject);
    }
    void shootSecondary()
    {
        secondaryHolder.Shoot(this.gameObject);
    }
    void shootUltimate()
    {
        ultimateHolder.Shoot(this.gameObject);
    }

    public void setCredits(int num)
    {
        credits += num;
        saveDataScript.SetCurrency(num);
    }
    public void resetCredits()
    {
        credits = 0;
        saveDataScript.SetCurrency(-saveDataScript.GetCurrency());
    }


    #region PauseLogic
    void Pause()
    {
        paused = true;
        cachedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (SP_GameManager.Instance)
        {
            SP_GameManager.Instance.setState(GameManagerBase.gameState.Pause);
        }
        
    }
    void unPause()
    {
        paused = false;
        rb.velocity = cachedVelocity;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb.velocity = cachedVelocity;
        if (SP_GameManager.Instance)
        {
            SP_GameManager.Instance.setState(GameManagerBase.gameState.Running);
        }
    }
    #endregion

    #region AI Communication
    public bool getChaseKey(out GameObject chaser, out Player3 script)
    {
        if (!chased)
        {
            chased = true;
            chaser = AIChaser;
            script = this;
            return true;
        }
        else
        {
            chaser = null;
            script = null;
            return false;
        }
    }

    public void returnChaseKey(out GameObject chaser, out Player3 script)
    {
        chaser = null;
        script = null;
        chased = false;
    }
    #endregion

    #region CUSTOMIZATION

    void updateStats()
    {
        normSpeed = bodyHolder.getSpeed();
        accSpeed = bodyHolder.getAccSpeed();
        decSpeed = bodyHolder.getDecSpeed();
        zRotationSpeed = bodyHolder.getZRotationSpeed();
        handlingSpeed = bodyHolder.getHandlingSpeed();
        maxHealth = bodyHolder.getMaxHealth();
        maxBoost = bodyHolder.getMaxBoost();
        boostStrength = bodyHolder.getBoostStrength();
        boostDecreaseModifier = bodyHolder.getBoostDecrease();
    }

    public Material getActiveColor()
    {
        return StarSparrowMaterials[(int)activeColor];
    }

    #region setPartsActive Functions

    public void setActiveColor(ShipColor setting)
    {
        activeColor = setting;
        bodyHolder.updateColor(StarSparrowMaterials[(int)activeColor]);
        primaryHolder.updateColor(StarSparrowMaterials[(int)activeColor]);
        secondaryHolder.updateColor(StarSparrowMaterials[(int)activeColor]);
        ultimateHolder.updateColor(StarSparrowMaterials[(int)activeColor]);
        //saveDataScript.saveShipPrefs();
    }

    public void setBodyActive(Body setting)
    {
        activeBody = setting;
        //BodyObj[(int)(activeBody - (int)(ShipColor.COUNT))].SetActive(false);
        BodyObj[(int)(setting - (int)(ShipColor.COUNT))].SetActive(true);
        bodyHolder.updateActiveBody();
        meshCollider.sharedMesh = bodyHolder.getMesh();
        meshCollider.convex = true;
        updateStats();
        //saveDataScript.saveShipPrefs();
    }

    public void setPrimaryActive(Primary setting)
    {
        activePrimary = setting;
        //PrimaryObj[(int)(activePrimary - (int)(Body.COUNT))].SetActive(false);
        PrimaryObj[(int)(setting - (int)(Body.COUNT))].SetActive(true);
        //primaryHolder.updateActiveWeapon();
        //saveDataScript.saveShipPrefs();
    }

    public void setSecondaryActive(Secondary setting)
    {
        activeSecondary = setting;
        //SecondaryObj[(int)(activeSecondary - (int)(Primary.COUNT))].SetActive(false);
        SecondaryObj[(int)(setting - (int)(Primary.COUNT))].SetActive(true);
        //secondaryHolder.updateActiveWeapon();
        //saveDataScript.saveShipPrefs();
    }

    public void setUltimateActive(Ultimate setting)
    {
        activeUltimate = setting;
        //UltimateObj[(int)(activeUltimate - (int)(Secondary.COUNT))].SetActive(false);
        UltimateObj[(int)(setting - (int)(Secondary.COUNT))].SetActive(true);
        //ultimateHolder.updateActiveWeapon();
        //saveDataScript.saveShipPrefs();
    }

    #endregion

    playerShipSettings createSaveStruct()
    {
        playerShipSettings temp;
        temp.activeColor = activeColor;
        temp.activeBody = activeBody;
        temp.activePrimary = activePrimary;
        temp.activeSecondary = activeSecondary;
        temp.activeUltimate = activeUltimate;
        return temp;
    }

    #region previewParts Functions (Shop functions)
    //Color
    public void previewColor(ShipColor preview)
    {
        shopColor = preview;
        bodyHolder.updateColor(StarSparrowMaterials[(int)shopColor]);
        primaryHolder.updateColor(StarSparrowMaterials[(int)activeColor]);
        secondaryHolder.updateColor(StarSparrowMaterials[(int)activeColor]);
        ultimateHolder.updateColor(StarSparrowMaterials[(int)activeColor]);
    }
    public void confirmColor()
    {
        activeColor = shopColor;
        //saveDataScript.saveShipPrefs(createSaveStruct());
    }
    public void resetToActiveColor()
    {
        if (shopColor != activeColor)
        {
            shopColor = 0;
            setActiveColor(activeColor);
        }
    }
    //Body
    public void deactivateAllBodies()
    {
        for (int i = 0; i < BodyObj.Count; i++)
        {
            if (BodyObj[i].activeInHierarchy)
            {
                BodyObj[i].SetActive(false);
            }
        }
    }
    public void previewBody(Body preview)
    {
        deactivateAllBodies();
        BodyObj[(int)(preview - (int)(ShipColor.COUNT))].SetActive(true);
        shopBody = preview;
    }
    public void confirmBody()
    {
        activeBody = shopBody;
        bodyHolder.updateActiveBody();
        //updateStats();
        //saveDataScript.saveShipPrefs(createSaveStruct());
    }
    public void resetToActiveBody()
    {
        deactivateAllBodies();
        BodyObj[(int)(activeBody - (int)(ShipColor.COUNT))].SetActive(true);
        bodyHolder.updateActiveBody();
    }


    //Primary
    public void deactivateAllPrimaries()
    {
        for (int i = 0; i < PrimaryObj.Count; i++)
        {
            if (PrimaryObj[i].activeInHierarchy)
            {
                PrimaryObj[i].SetActive(false);
            }
        }
    }
    public void previewPrimary(Primary preview)
    {
        deactivateAllPrimaries();
        PrimaryObj[(int)(preview - (int)(Body.COUNT))].SetActive(true);
        shopPrimary = preview;
    }
    public void confirmPrimary()
    {
        activePrimary = shopPrimary;
        //primaryHolder.updateActiveWeapon();
        //saveDataScript.saveShipPrefs(createSaveStruct());
    }
    public void resetToActivePrimary()
    {
        deactivateAllPrimaries();
        PrimaryObj[(int)(activePrimary - (int)(Body.COUNT))].SetActive(true);
    }



    //Secondary
    public void deactivateAllSecondaries()
    {
        for (int i = 0; i < SecondaryObj.Count; i++)
        {
            if (SecondaryObj[i].activeInHierarchy)
            {
                SecondaryObj[i].SetActive(false);
            }
        }
    }
    public void previewSecondary(Secondary preview)
    {
        deactivateAllSecondaries();
        SecondaryObj[(int)(preview - (int)(Primary.COUNT))].SetActive(true);
        shopSecondary = preview;
    }
    public void confirmSecondary()
    {
        activeSecondary = shopSecondary;
        //secondaryHolder.updateActiveWeapon();
        //saveDataScript.saveShipPrefs(createSaveStruct());
    }
    public void resetToActiveSecondary()
    {
        deactivateAllSecondaries();
        SecondaryObj[(int)(activeSecondary - (int)(Primary.COUNT))].SetActive(true);
    }

    //Ultimate
    public void deactivateAllUltimates()
    {
        for (int i = 0; i < UltimateObj.Count; i++)
        {
            if (UltimateObj[i].activeInHierarchy)
            {
                UltimateObj[i].SetActive(false);
            }
        }
    }
    public void previewUltimate(Ultimate preview)
    {
        deactivateAllUltimates();
        UltimateObj[(int)(preview - (int)(Secondary.COUNT))].SetActive(true);
        shopUltimate = preview;
    }
    public void confirmUltimate()
    {
        activeSecondary = shopSecondary;
        //ultimateHolder.updateActiveWeapon();
        //saveDataScript.saveShipPrefs(createSaveStruct());
    }
    public void resetToActiveUltimate()
    {
        deactivateAllUltimates();
        UltimateObj[(int)(activeUltimate - (int)(Secondary.COUNT))].SetActive(true);
    }

    #endregion

    #endregion
}
