using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class Player4Base : MonoBehaviour, ITeamMember
{
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
        public bool pausedState;//added paused state to HUDData for brevity
        
    }

    #region Controls
    [Header("Controls")]
    [SerializeField] protected bool ControllerEnabled = false;
    //Firing Weapons 
    [SerializeField] protected KeyCode UltimateFireKey = KeyCode.E;
    [SerializeField] protected string PrimaryFireControl = "Fire1";
    [SerializeField] protected string SecondaryFireControl = "Fire2";
    [SerializeField] protected string UltimateFireControl = "Fire3";
    //WASD           
    [SerializeField] protected KeyCode AccelerateKey = KeyCode.W;
    [SerializeField] protected KeyCode DecelerateKey = KeyCode.S;
    [SerializeField] protected KeyCode LeftKey = KeyCode.A;
    [SerializeField] protected KeyCode RightKey = KeyCode.D;
    [SerializeField] protected string AccelerateControl;
    [SerializeField] protected string DecelerateControl;
    [SerializeField] protected string LeftControl;
    [SerializeField] protected string RightControl;
    //Other Controls 
    [SerializeField] protected KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] protected KeyCode boostKey = KeyCode.LeftShift;
    [SerializeField] protected KeyCode flareKey = KeyCode.Space;
    [SerializeField] protected string flareControl = "Fire4";
    [SerializeField] protected string pauseControl;
    [SerializeField] protected string boostControl;
    [SerializeField] protected bool invertYaxis = false;
    [SerializeField] protected bool invertXaxis = false;
    [SerializeField] protected float mouseXclamp = 0.5f;
    [SerializeField] protected float mouseYclamp = 0.5f;
    #endregion

    #region Cheat Variables
    [Header("Cheats/Debug keys")]
    [SerializeField] protected bool cheatMenuIsOpen = false;
    [SerializeField] protected bool GodMode = false;
    [SerializeField] protected bool InfiniteBoost = false;
    [SerializeField] protected bool canEscape = false;
    [SerializeField] protected KeyCode cheatMenuKey = KeyCode.BackQuote;
    [SerializeField] protected KeyCode godModeKey = KeyCode.Alpha1;
    [SerializeField] protected KeyCode infinBoostKey = KeyCode.Alpha2;
    [SerializeField] protected KeyCode skipWaveKey = KeyCode.Alpha3;
    [SerializeField] protected KeyCode damagePlayerKey = KeyCode.Alpha4;
    [SerializeField] protected KeyCode fillUltimateKey = KeyCode.Alpha5;
    [SerializeField] protected KeyCode disableEscapeCheck = KeyCode.Alpha6;
    [SerializeField] protected KeyCode ShowAIPathing = KeyCode.Alpha7;
    [SerializeField] protected KeyCode ShowAIStates = KeyCode.Alpha8;
    [SerializeField] protected KeyCode KillWaveKey = KeyCode.Alpha9;
    [SerializeField] protected KeyCode ShowColliders = KeyCode.Alpha0;
    #endregion

    #region Timers and internal data (Only Serialized for debugging)
    [Header("Debug")]
    [SerializeField] protected bool debug;
    [SerializeField] protected float currSpeed;
    [SerializeField] protected float currHealth;
    [SerializeField] protected float currBoost;
    [SerializeField] protected float currBoostStrength = 1;
    [SerializeField] protected bool paused = false;
    [SerializeField] protected Vector3 cachedVelocity;
    [SerializeField] protected bool chased = false;
    [SerializeField] protected bool boosting = false;
    [SerializeField] protected int invertYVal = -1;
    [SerializeField] protected int invertXVal = 1;
    [SerializeField] protected bool escaping = false;
    [SerializeField] protected float escapeTime = 15.0f;
    [SerializeField] protected float escapeTimeDef = 15.0f;
    #endregion

    #region Stuff Set in Prefab
    [Header("Stuff Set in Prefab")]
    [SerializeField] protected Ship ship;
    [SerializeField] protected Player4Camera cameraScript;
    [SerializeField] protected MeshCollider meshCollider;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Transform coinParent;
    [SerializeField] protected HealthBar HUDscript;
    [SerializeField] protected GameObject AIChaser;
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] protected GameObject deathCamera;
    [SerializeField] protected GameObject killedBy;
    [SerializeField] protected bool dead;
    [SerializeField] protected string playSceneName = "SampleScene";
    [SerializeField] int TeamNumber = 2;
    [SerializeField] List<AttackPath> attackPaths;
    [SerializeField] private List<int> takenAttackPaths = new List<int>();
    #endregion

    #region extras added
    [Header("Extras added as ideas or for future use")]
    [SerializeField] protected float boostGainMultiplier = 1;
    [SerializeField] protected float stunnedTime = 0;
    #endregion

    GameObject camera; 

    bool firstUpdate = true;

    protected virtual void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    protected virtual void controls()
    {
        if (Input.GetButton(PrimaryFireControl))
        {
            ship.firePrimary();
        }
        if (Input.GetButton(SecondaryFireControl))
        {
            ship.fireSecondary();
        }
        if (Input.GetKeyDown(UltimateFireKey) || (ControllerEnabled && Input.GetButton(UltimateFireControl)))
        {
            ship.fireUltimate();
        }
        if (Input.GetKeyDown(flareKey) || (ControllerEnabled && Input.GetButton(flareControl)))
        {
            ship.fireFlare();
        }
        if (Input.GetKey(boostKey) || (ControllerEnabled && Input.GetButton(boostControl)))
        {
            boosting = true;
            if (currBoost > 0)
            {
                currBoostStrength = ship.stats().boostStrength;
                if (!InfiniteBoost)
                {
                    currBoost -= Time.deltaTime * ship.stats().boostDecreaseModifier;
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
    }

    protected virtual void Update()
    {
        if (stunnedTime > 0)
        {
            return;
        }

        if (firstUpdate)
        {
            currHealth = ship.stats().maxHealth;
            if (!GameManagerBase.Instance.isMulti())
            {
                meshCollider.sharedMesh = ship.mesh();
            }
            firstUpdate = false;
        }

        #region Controls
        controls();
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
                        currHealth = ship.stats().maxHealth;
                    }
                }
                if (Input.GetKeyDown(infinBoostKey))
                {
                    InfiniteBoost = !InfiniteBoost;
                    currBoost = ship.stats().maxBoost;
                }
                if (Input.GetKeyDown(skipWaveKey))
                {
                    //Skip Wave operation here
                }
                if (Input.GetKeyDown(damagePlayerKey))
                {
                    TakeDamage(ship.stats().maxHealth * 0.1f, null);
                }
                if (Input.GetKeyDown(fillUltimateKey))
                {
                    ship.fillUlt();
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
                if (Input.GetKeyDown(ShowAIPathing))
                {
                    GameManagerBase.Instance.toggleDebug_showAICollision();
                }
                if (Input.GetKeyDown(ShowAIStates))
                {
                    GameManagerBase.Instance.toggleDebug_showAIStates();
                }
                if (Input.GetKeyDown(KillWaveKey))
                {
                    if (AIManager.Instance)
                    {
                        if (AIManager.Instance.gameTypeSetting == AIManager.MatchType.SoloVsWaves)
                        {
                            AIManager.Instance.killWave();
                        }

                    }
                }
                if (Input.GetKeyDown(ShowColliders))
                {
                    GameManagerBase.Instance.toggleDebug_showColiders();
                    

                }
                if (GameManagerBase.Instance.debug_showColiders == true)
                {
                    // debug
                    int layer = 1 << 12;

                    // Projectiles
                    int layer2 = 1 << 6;

                    // ui
                    int layer3 = 1 << 5;
                    camera.GetComponent<Camera>().cullingMask = layer + layer2 + layer3 + 1;
                }
                else
                {

                    int layer = 1 << 10;
                    int layer2 = 1 << 5;
                    int layer3 = 1 << 11;
                    camera.GetComponent<Camera>().cullingMask = layer+layer2+layer3 + 1;
                }
               
            }
        }
        #endregion
    }
    protected virtual void FixedUpdate()
    {
        if (stunnedTime > 0)
        {
            stunnedTime -= Time.deltaTime;
            return;
        }

        #region Movement
        float speedMod = 0;
        if (Input.GetKey(AccelerateKey) || (ControllerEnabled && Input.GetButton(AccelerateControl)))
        {
            speedMod = Mathf.Lerp(currSpeed, ship.stats().accSpeed * currBoostStrength, Time.deltaTime * 3);
        }
        else if (Input.GetKey(DecelerateKey) || (ControllerEnabled && Input.GetButton(DecelerateControl)))
        {
            speedMod = Mathf.Lerp(currSpeed, -ship.stats().decSpeed, Time.deltaTime * 3);
        }
        else
        {
            speedMod = Mathf.Lerp(currSpeed, ship.stats().speed * currBoostStrength, Time.deltaTime * 3);
        }
        currSpeed = speedMod;
        Vector3 moveDirection = new Vector3(0, 0, speedMod);
        moveDirection = transform.TransformDirection(moveDirection);
        rb.velocity = moveDirection;
        #endregion

        #region Rotation
        float rotationTemp = 0;
        if (Input.GetKey(LeftKey) || (ControllerEnabled && Input.GetButton(LeftControl)))
        {
            rotationTemp = ship.stats().zRotationSpeed;
        }
        else if (Input.GetKey(RightKey) || (ControllerEnabled && Input.GetButton(RightControl)))
        {
            rotationTemp = -ship.stats().zRotationSpeed;
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
        float inputX = 0;
        float inputY = 0;
        if (!paused)
        {
            inputY = Input.GetAxis("Mouse Y");
            inputX = Input.GetAxis("Mouse X");
            inputX = Mathf.Clamp(inputX, -mouseXclamp, mouseXclamp);
            inputY = Mathf.Clamp(inputY, -mouseYclamp, mouseYclamp);
        }
        //transform.Rotate(invertYVal * inputY * ship.stats().handlingSpeed, 0.0f, 0.0f);
        //transform.Rotate(0.0f, invertXVal * inputX * ship.stats().handlingSpeed, 0.0f);
        //transform.Rotate(0.0f, 0.0f, rotationTemp * ship.stats().zRotationSpeed);

        transform.Rotate(invertYVal * inputY * ship.stats().handlingSpeed, 
            invertXVal * inputX * ship.stats().handlingSpeed, 
            rotationTemp * ship.stats().zRotationSpeed);


        #endregion
    }

    protected virtual void TimerTick()
    {
        if (currBoost <= ship.stats().maxBoost && !boosting)
        {
            currBoost += (Time.deltaTime * boostGainMultiplier);
        }
        else if (currBoost > ship.stats().maxBoost)
        {
            currBoost = ship.stats().maxBoost;
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

    protected virtual void EscapeCheck()
    {
        float distance = Vector3.Distance(this.transform.position, Vector3.zero);
        if (LevelBuilder.Instance != null)
        {
        if (distance > LevelBuilder.Instance.getMapRadius())
        {
            escaping = true;
        }
        else
        {
            escaping = false;
        }

        }
        else
        {
            if (distance > 750)
            {
                escaping = true;
            }
            else
            {
                escaping = false;
            }
        }
    }

    public void updateHUD(out HUDData dat)
    {
        float currUltValue = ship.getCurrentUltValue();
        float ultChargeTime = ship.getUltChargeTime();
        dat.currHealth = currHealth;
        dat.maxHealth = ship.stats().maxHealth;
        dat.currBoost = currBoost;
        dat.maxBoost = ship.stats().maxBoost;
        dat.currUlt = currUltValue;
        dat.maxUlt = ultChargeTime;
        dat.cheatMenuIsOpen = cheatMenuIsOpen;
        dat.GodMode = GodMode;
        dat.InfiniteBoost = InfiniteBoost;
        dat.canEscape = canEscape;
        dat.escaping = escaping;
        dat.escapeTime = escapeTime;
        dat.pausedState = paused;
    }

    public virtual void TakeDamage(float damage) { Debug.Log("Wrong take dmg"); }

    public virtual void TakeDamage(float damage, GameObject shooter){}

    #region ITeamMember Functions
    public int getTeamNumber()
    {
        return TeamNumber;
    }

    public bool canHaveFollowers(GameObject follower, out int index)
    {
        follower = null;
        index = 0;
        return false;
    }

    public bool isAttackable(out AttackPath path, out int index)
    {
        int returnIndex = 0;
        AttackPath returnPath = null;
        if (takenAttackPaths.Count == attackPaths.Count)
        {
            index = 0;
            path = null;
            return false;
        }
        else
        {
            List<int> availableIndices = new List<int>();
            for (int i = 0; i < attackPaths.Count; i++)
            {
                bool taken = false;
                for (int f = 0; f < takenAttackPaths.Count; f++)
                {
                    if (i == takenAttackPaths[f])
                    {
                        taken = true;
                        break;
                    }
                }
                if (!taken)
                {
                    availableIndices.Add(i);
                }
            }
            int temp = Random.Range(0, availableIndices.Count);
            returnIndex = availableIndices[temp];
            takenAttackPaths.Add(returnIndex);
            returnPath = attackPaths[returnIndex];

            index = returnIndex;
            path = returnPath;
            return true;
        }
    }

    public void returnAttackIndex(int index)
    {
        takenAttackPaths.Remove(index);
    }

    public void returnFollowIndex(int index){}
    #endregion

    public virtual void OnDeath(){}

    [PunRPC]
    public virtual void Test(int t) { }

    public virtual void LastHit(int shooter) { Debug.Log("Wrong lasthit"); }

    public virtual void OnWin(int reward)
    {
        GodMode = true;
        ship.setCredits(reward);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ship.saveData();
    }

    public void setCredits(int val)
    {
        ship.setCredits(val);
    }
    public void setStunTime(float stun)
    {
        stunnedTime = stun;
    }
    public void swapInfiniteBoost()
    {
        InfiniteBoost = !InfiniteBoost;
    }
    public void swapGodBoost()
    {
        GodMode = !GodMode;
    }

    public Transform getCoinParent()
    {
        return coinParent;
    }


    public virtual void OnTriggerEnter(Collider other){}

    #region AI Communication
    public bool getChaseKey(out GameObject chaser, out Player4Base script)
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

    public void returnChaseKey(out GameObject chaser, out Player4Base script)
    {
        chaser = null;
        script = null;
        chased = false;
    }
    #endregion

    #region Enable/Disable and Pause Logic
    public virtual void Pause(){}
    public virtual void unPause(){}
    #endregion


}
