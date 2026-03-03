using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;

public class MultiPlayer4 : Player4Base
{
    #region Multiplayer Variables
    [Header("Multiplayer")]
    [SerializeField] PhotonView PV;
    [SerializeField] MatchManager P_Manager;
    [SerializeField] ShipManager _shipManager;
    [SerializeField] GameObject mUI;

    [SerializeField] GameObject lastHit;


    #endregion

    private void OnEnable()
    {
        currHealth = ship.stats().maxHealth;
        currBoost = ship.stats().maxBoost;
    }


    public void Reset()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
    }



    protected override void Start()
    {
        if (!PV.IsMine)
        {
            cameraScript.setCrosshairActive(false);
            cameraScript.setCameraActive(false);
            HUDscript.gameObject.SetActive(false);
        }
        else
        {
            P_Manager = FindObjectOfType<MatchManager>();
            _shipManager = FindObjectOfType <ShipManager>();
        }

        
        

        base.Start();
    }


    protected override void controls()
    {
        if (P_Manager.currState == MatchManager.gameState.PreGame)
        {
            return;
        }
        base.controls();
    }

    protected override void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

       // Debug.Log(PV.Owner + " Kills: " + PV.Owner.GetKills() + " Deaths: " + PV.Owner.GetDeaths());

        TimerTick();

        if (!canEscape)
        {
            EscapeCheck();

            if (escapeTime <= 0)
            {
                TakeDamage(ship.stats().maxHealth, null);
            }
        }

        if (paused)
        {
            if (Input.GetKeyDown(pauseKey) || (ControllerEnabled && Input.GetButton(pauseControl)))
            {
                unPause();
            }
            return;
        }

        base.Update();

        if (Input.GetKeyDown(pauseKey) || (ControllerEnabled && Input.GetButton(pauseControl)))
        {
            if (!paused)
            {
                Pause();
            }
        }
        if (PV.IsMine)
        {
        if (Input.GetKey(KeyCode.Tab))
        {
            mUI.gameObject.SetActive(true);
        }
        else
        {
            mUI.gameObject.SetActive(false);
        }

        }

    }

    protected override void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        if (P_Manager.currState == MatchManager.gameState.PreGame)
        {
            return;
        }
        base.FixedUpdate();
    }

    public override void TakeDamage(float damage, GameObject shooter)
    {
        if (shooter)
        {
            PV.RPC("RPC_TakeDamageShooter", RpcTarget.All, damage, shooter.GetComponentInParent<PhotonView>().ViewID);
        }
        else
        {
            PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        }
        
        killedBy = shooter;
    }

    public void M_TakeDamage(float damage, int viewIDShooter)
    {
        PV.RPC("RPC_TakeDamageShooter", RpcTarget.All, damage, viewIDShooter);
    }

    [PunRPC]
    void RPC_TakeDamageShooter(float damage, int viewID)
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (!GodMode)
        {
            currHealth -= damage;
        }
        if (!GodMode && currHealth <= 0)
        {
            if (viewID != 0)
            {
                m_OnDeath(viewID);
            }
            else
            {
                OnDeath();
            }
        }
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
        {
            return;
        }

        if (!GodMode)
        {
            currHealth -= damage;
        }
        if (!GodMode && currHealth <= 0)
        {
            OnDeath();
        }
    }

    public void addDeath()
    {
        if (!PV.IsMine)
        {
            return;
        }
        PV.Owner.AddDeath();
    }

    public void m_OnDeath(int killerViewID)
    {
        PhotonView killerView = PhotonNetwork.GetPhotonView(killerViewID);
        killerView.Owner.AddKill();
        OnDeath();
    }

    public override void OnDeath()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", deathEffect.name), transform.position, transform.rotation);
        addDeath();
        GameObject deathCam = Instantiate(deathCamera, Vector3.zero, transform.rotation);
        if (killedBy)
        {
            deathCam.GetComponent<DeathCameraScript>().setTarget(killedBy);
            _shipManager.setDeathCam(deathCam);
        }
        else
        {
            deathCam.GetComponent<DeathCameraScript>().setPosition(transform.position);
        }
        _shipManager.Death();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(ship.stats().maxHealth, null);
            other.GetComponent<AIUnit>().TakeDamage((int)ship.stats().maxHealth, this.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            TakeDamage(ship.stats().maxHealth, null);
            other.GetComponent<Player4Base>().TakeDamage((int)ship.stats().maxHealth, null);
        }

        if (other.CompareTag("Aster"))
        {
            TakeDamage(ship.stats().maxHealth, null);
        }
    }

    public override void Pause()
    {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void unPause()
    {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


}
