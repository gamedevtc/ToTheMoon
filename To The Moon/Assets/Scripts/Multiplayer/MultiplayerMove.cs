using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MultiplayerMove : MonoBehaviour
{
    private int maxHealth = 100;
    private int maxBoost = 250;
    private int ultGoal = 150;

    [Header("Player Variables")]
    [SerializeField] private int health = 100;
    [SerializeField] private int boostTime = 0;
    [SerializeField] private int boostIncriment = 25;
    [SerializeField] private int boostDecriment = 5;
    [SerializeField] private int ultCharge = 0;
    [SerializeField] private int ultIncriment = 25;

    [Header("Projectile Variables")]
    [SerializeField] private GameObject gunOrigin;
    [SerializeField] private AudioSource blaster;
    [SerializeField] private GameObject bulletOG;
    [SerializeField] private float cooldownPrimary = 0.0f;
    [SerializeField] private float blasterCooldown = 0.15f;

    [SerializeField] private GameObject missileOG;
    [SerializeField] private float cooldownSecondary = 0.0f;
    [SerializeField] private GameObject AIChaser;

    [SerializeField] private GameObject ultOrigin;
    [SerializeField] private GameObject UltOG;
    private float ultTimer = 0;


    private bool boosting = false;
    private float defaultAccel = 0;
    private float defaultNorm = 0;
    private float boostTimer = 0.0f;
    private float boostDecrimentTimer = 0.0f;

    [Header("Player Camera")]
    [SerializeField] private float normSpeed = 25f;
    [SerializeField] private float accSpeed = 455f;
    [SerializeField] private float decSpeed = 15f;
    [SerializeField] private Transform camPos;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private float cameraSmoother = 4.0f;
    [SerializeField] private float speed;
    [SerializeField] private RectTransform crosshairText;
    [SerializeField] private AudioSource audioSource;

    [Header("Multiplayer")]
    [SerializeField] private PhotonView view;
    [SerializeField] private GameObject _ui;
    [SerializeField] private string _name;
    PlayerManager P_Manager;

    bool p = false;
    Rigidbody rb;

    private void Awake()
    {
        if (GameManagerBase.Instance.isMulti() == true)
        {
            P_Manager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        GameManagerBase.Instance.setState(GameManagerBase.gameState.Running);
        if (view != null)
        {
            if (!view.IsMine)
            {
               
                audioSource.gameObject.SetActive(false);
                crosshairText.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(false);
            }
            

        }

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultAccel = accSpeed;
        defaultNorm = normSpeed;

        if (GameManagerBase.Instance.isMulti() == true)
        {
            _ui = GameObject.Find("PlayerList");

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view != null)
        {
            if (!view.IsMine)
            {
                return;
            }

        }
        if (GameManagerBase.Instance.isMulti() == true)
        {

            if (Input.GetKey(KeyCode.Tab))
            {
                _ui.gameObject.SetActive(true);
            }
            else
            {
                _ui.gameObject.SetActive(false);
            }

            if (health <= 0)
            {


                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
                p = true;


                P_Manager.Death();
            }


        }


        if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Running)
        {
            cooldownPrimary += Time.deltaTime;
            cooldownSecondary += Time.deltaTime;
            boostTimer += Time.deltaTime;
            ultTimer += Time.deltaTime;

            if (boostTime < maxBoost && boosting == false && boostTimer > 0.5f)
            {
                boostTime += boostIncriment;
                boostTimer -= boostTimer;
                if (boostTime > maxBoost)
                {
                    boostTime = maxBoost;
                }
            }
            if (ultCharge < ultGoal && ultTimer > 0.5f)
            {
                ultCharge += ultIncriment;
                ultTimer -= ultTimer;
            }
            if (cooldownPrimary >= blasterCooldown)
            {
                if (Input.GetButton("Fire1"))
                {
                    ShootPrimary();
                    cooldownPrimary -= cooldownPrimary;
                }
            }
            if (cooldownSecondary >= 1.0f)
            {
                if (Input.GetButton("Fire2"))
                {
                    ShootSecondary();
                    cooldownSecondary -= cooldownSecondary;
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Pause hud open or close here
            }

           


            if (Input.GetKeyDown(KeyCode.E) && ultCharge == ultGoal)
            {
                Ult();

            }

            if (Input.GetKey(KeyCode.LeftShift) && boostTime > 0)
            {
                Boost();
            }
            else
            {
                boosting = false;
                accSpeed = defaultAccel;
                normSpeed = defaultNorm;
            }

            //debug
            if (Input.GetKeyDown(KeyCode.O))
            {
                TakeDamage(10);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                ultCharge = ultGoal;
            }

            if (Input.GetKey(KeyCode.P))
            {
                P_Manager.Death();
               // ShootPrimary();
                // cooldownPrimary -= cooldownPrimary;
            }

        }
    }

    private void FixedUpdate()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camPos.position, Time.deltaTime * cameraSmoother);
        mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, camPos.rotation, Time.deltaTime * cameraSmoother);
        if (view != null)
        {
            if (!view.IsMine)
            {
                return;
            }
        }
        if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Win)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            p = true;
            return;
        }

        if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Running)
        {
            if (p)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                p = false;
            }
            if (Input.GetKey(KeyCode.W))
            {
                speed = Mathf.Lerp(speed, accSpeed, Time.deltaTime * 3);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                speed = Mathf.Lerp(speed, -decSpeed, Time.deltaTime * 3);
            }
            else
            {
                speed = Mathf.Lerp(speed, normSpeed, Time.deltaTime * 3);
            }
            Vector3 moveDirection = new Vector3(0, 0, speed);
            moveDirection = transform.TransformDirection(moveDirection);
            rb.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);


            //CameraFollow
        //    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camPos.position, Time.deltaTime * cameraSmoother);
       //     mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, camPos.rotation, Time.deltaTime * cameraSmoother);


            float roatationTemp = 0;
            if (Input.GetKey(KeyCode.A))
            {
                roatationTemp = 1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                roatationTemp = -1;
            }

            transform.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed, 0.0f, 0.0f);
            transform.Rotate(0.0f, Input.GetAxis("Mouse X") * rotateSpeed, 0.0f);
            transform.Rotate(0.0f, 0.0f, roatationTemp * rotateSpeed);

            if (crosshairText)
            {
                crosshairText.position = mainCamera.WorldToScreenPoint(transform.position + transform.forward * 100);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(maxHealth);
            other.GetComponent<AIController>().onDamage(maxHealth, this.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            TakeDamage(maxHealth);
            other.GetComponent<MultiplayerMove>().TakeDamage(maxHealth);
        }

        if (other.CompareTag("Aster"))
        {
            TakeDamage(maxHealth);
        }
    }

    void ShootPrimary()
    {
        blaster.Play();
        if (GameManagerBase.Instance.isMulti() == true)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "M_bullet"), gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
        }
        else
        {
            GameObject bullet = Instantiate(bulletOG, gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));

        }




    }

    void ShootSecondary()
    {
        blaster.Play();
        //GameObject bullet = Instantiate(missileOG, gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));


        if (GameManagerBase.Instance.isMulti() == true)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "M_missile"), gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
        }
        else
        {
            GameObject bullet = Instantiate(missileOG, gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));

        }

    }

    void Ult()
    {
        if (GameManagerBase.Instance.isMulti() == true)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "M_Bomb"), ultOrigin.transform.position, Quaternion.LookRotation(ultOrigin.transform.forward));
        }
        else
        {
            Instantiate(UltOG, ultOrigin.transform.position, Quaternion.LookRotation(ultOrigin.transform.forward));

        }

        ultCharge -= ultCharge;
    }

    void Boost()
    {
        boosting = true;
        boostDecrimentTimer += Time.deltaTime;
        if (boostTime <= maxBoost && boostTime != 0)
        {
            if (boostDecrimentTimer >= 0.05f)
            {
                boostTime -= boostDecriment;
                boostDecrimentTimer -= boostDecrimentTimer;
            }
            accSpeed = defaultAccel * 1.5f;
            normSpeed = defaultNorm * 1.5f;
        }
    }


    //Getters
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetMaxBoost()
    {
        return maxBoost;
    }
    public int GetHealth()
    {
        return health;
    }

    public int GetUltCharge()
    {
        return ultCharge;
    }

    public int GetBoost()
    {
        return boostTime;
    }

    public int GetMaxUlt()
    {
        return ultGoal;
    }

    //Setters
    void SetMaxHealth(int maxHP)
    {
        maxHealth = maxHP;
    }

    void SetMaxBoost(int maxBooster)
    {
        maxBoost = maxBooster;
    }

    void SetMaxUlt(int maxUltGoal)
    {
        ultGoal = maxUltGoal;
    }

    void SetHealth(int HP)
    {
        health = HP;
    }

    void SetBoost(int boost)
    {
        boostTime = boost;
    }

    void SetUltCharge(int charge)
    {
        ultCharge = charge;
    }

    //Public Methods
    public void TakeDamage(int damage)
    {

        if (GameManagerBase.Instance.isMulti() == true)
        {
            Debug.Log("RPC EVENT");
            view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        }
        else
        {

        health -= damage;
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        if (!view.IsMine)
        {
            return;
        }
        else
        {

            health -= damage;
            Debug.Log("Took " + damage+ ", Health is " + health);
        }
    }


    public GameObject getChaser()
    {
        return AIChaser;
    }

    public void Die()
    {


    }



}
