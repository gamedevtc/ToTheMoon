using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected ProjectileStats stats;
    [SerializeField] protected GameObject shooter;
    [SerializeField] protected int m_shooter;
    [SerializeField] protected bool paused;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Vector3 cachedVelocity;
    [SerializeField] protected string EnemyLockTag = "Enemy";
    [SerializeField] protected string PlayerLockTag = "Player";
    [SerializeField] protected string AsterLockTag = "Aster";
    [SerializeField] protected string FlareLockTag = "Flare";
    [SerializeField] protected GameObject target;
    [SerializeField] protected PhotonView PV;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    public bool isSelf(GameObject hit)
    {
        return hit == shooter;
    }

   

    
    public void setShooter(GameObject shoot)
    {
        shooter = shoot;

    }

    public void setTarget(GameObject targ)
    {
        target = targ;
    }

    private void OnEnable()
    {
        if (!GameManagerBase.Instance.isMulti())
        {
            //add pause and unpause to event
            EventManager.pauseEvent += Pause;
            EventManager.unPauseEvent += unPause;
        }
    }

    private void OnDisable()
    {
        if (!GameManagerBase.Instance.isMulti())
        {
            //remove from event
            EventManager.pauseEvent -= Pause;
            EventManager.unPauseEvent -= unPause;
        }
    }

    protected virtual void Pause()
    {
        paused = true;
        cachedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
    }

    protected virtual void unPause()
    {
        paused = false;
        rb.velocity = cachedVelocity;
    }


    [PunRPC]
    public void M_setShooter(int viewid)
    {
        m_shooter = viewid;
    }

    public void callRPC(Ship shipMain)
    {
        PV.RPC("M_setShooter", RpcTarget.All, shipMain.getPlayerMain().gameObject.GetComponent<PhotonView>().ViewID);
    }

    public bool M_isSelf(int viewIdHit)
    {
        return viewIdHit == m_shooter;
    }
}
