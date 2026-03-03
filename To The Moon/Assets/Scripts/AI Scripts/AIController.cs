using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public enum Behavior { Aggressive = 0, Defensive = 1, Cover = 2 }

    public Behavior behavior;

    public List<AIController> followers;
    public AIController leader;
    public bool following = false;
    public bool leading = false;

    AIHelper settings;
    public WaveManager manager;
    public GameManager gameManager;

    [SerializeField] private GameObject gunOrigin;
    [SerializeField] private AudioSource blasterAudio;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletSpeed = 10;
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private GameObject DeathEffect;
    [SerializeField] private GameObject DeathDropCopper;
    [SerializeField] private GameObject DeathDropSilver;
    [SerializeField] private GameObject DeathDropGold;

    //current State
    public Vector3 position;
    public Vector3 forward;
    Vector3 velocity;
    Transform player;

    //update
    Vector3 accel;
    public Vector3 avgAvoidNeighborHeading;
    public Vector3 followHeading;
    public int neighborShips = 0;

    //running data
    Transform cachedTransform;
    [Header("Debug:")]
    [SerializeField] GameObject target;
    [SerializeField] Player3 targetScript;

    [SerializeField] private int Health;
    [SerializeField] private int MaxHealth;
    private int Shields;//?
    private float fireTimer = 0;

    private IEnumerator coroutine;


    // Start is called before the first frame update
    void Awake()
    {
        this.settings = FindObjectOfType<AIHelper>();
        gameManager = FindObjectOfType<GameManager>();
        manager = FindObjectOfType<WaveManager>();
        cachedTransform = transform;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = 0;
        switch (behavior)
        {
            case (Behavior.Aggressive):
                startSpeed = (settings.AggressiveMinSpeed + settings.AggressiveMaxSpeed) * 0.5f;
                break;
            case (Behavior.Defensive):
                startSpeed = (settings.DefensiveMinSpeed + settings.DefensiveMaxSpeed) * 0.5f;
                break;
            case (Behavior.Cover):
                startSpeed = (settings.CoverMinSpeed + settings.CoverMaxSpeed) * 0.5f;
                break;
        }
        velocity = forward * startSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManagerBase.Instance.isMulti() == false)
        {
            if (GameManagerBase.Instance.getState() == GameManagerBase.gameState.Pause)
            {
                return;
            }
        }

        timerTick();

        Vector3 acceleration = Vector3.zero;

        //basic ass targetting
        if (target == null && GameManagerBase.Instance.getState() == GameManagerBase.gameState.Running)//aggressive ships only add target for now
        {
            //check distance to player
            //float distance = Vector3.Distance(gameManager.playerScript.transform.position, this.cachedTransform.position);

            ////if close enough (in settings) set as target
            //switch (behavior)
            //{
            //    case (Behavior.Aggressive):
            //        if (distance < settings.AggressivePlayerDetectionRadius)
            //        {
            //            gameManager.playerScript.getChaseKey(out target, out targetScript);
            //        }
            //        break;
            //    case (Behavior.Defensive):
            //        if (distance < settings.DefensivePlayerDetectionRadius)
            //        {
            //            gameManager.playerScript.getChaseKey(out target, out targetScript);
            //        }
            //        break;
            //    case (Behavior.Cover):
            //        if (distance < settings.CoverPlayerDetectionRadius)
            //        {
            //            gameManager.playerScript.getChaseKey(out target, out targetScript);
            //        }
            //        break;
            //}
        }

        {//check if player is in front and burst fire if so
            RaycastHit hit;
            switch (behavior)
            {
                case (Behavior.Aggressive)://Check to shoot
                    Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.AggressiveTargetDistance, Color.blue);
                    if (Physics.SphereCast(position, settings.AggressiveTargetRadius, forward, out hit, settings.AggressiveTargetDistance, settings.playerTargetMask, QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            if (fireTimer <= 0)
                            {
                                coroutine = BurstFire();
                                StartCoroutine(coroutine);
                                //Fire();
                            }
                        }
                    } 
                    break;
                case (Behavior.Defensive):
                    Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.DefensiveTargetDistance, Color.blue);
                    if (Physics.SphereCast(position, settings.DefensiveTargetRadius, forward, out hit, settings.DefensiveTargetDistance, settings.playerTargetMask, QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            if (fireTimer <= 0)
                            {
                                coroutine = BurstFire();
                                StartCoroutine(coroutine);
                                //Fire();
                            }
                        }
                    }
                    break;
                case (Behavior.Cover):
                    Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.CoverTargetDistance, Color.blue);
                    if (Physics.SphereCast(position, settings.CoverTargetRadius, forward, out hit, settings.CoverTargetDistance, settings.playerTargetMask, QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            if (fireTimer <= 0)
                            {
                                coroutine = BurstFire();
                                StartCoroutine(coroutine);
                                //Fire();
                            }
                        }
                    }
                    break;
            }
        }

        if (target != null)//This is thrown together and might be garbage
        {
            Vector3 offsetToTarget = (target.transform.position - position);
            acceleration += SteerDirection(offsetToTarget) * settings.TargetWeight;
            
            RaycastHit hit;
            switch (behavior)
            {
                case (Behavior.Aggressive)://Check to shoot
                    Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.AggressiveTargetDistance, Color.blue);
                    if (Physics.SphereCast(position, settings.AggressiveTargetRadius, forward, out hit, settings.AggressiveTargetDistance, settings.playerTargetMask, QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            if (fireTimer <= 0)
                            {
                                //coroutine = BurstFire();
                                //StartCoroutine(coroutine);
                                Fire();
                            }
                        }
                    } //Check to forget Target
                    else if (Vector3.Distance(this.transform.position, target.transform.position) > settings.AggressiveTargetForgetDistance)
                    {
                        targetScript.returnChaseKey(out target, out targetScript);
                    }
                    break;
                case (Behavior.Defensive):
                    Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.DefensiveTargetDistance, Color.blue);
                    if (Physics.SphereCast(position, settings.DefensiveTargetRadius, forward, out hit, settings.DefensiveTargetDistance, settings.playerTargetMask, QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            if (fireTimer <= 0)
                            {
                                //coroutine = BurstFire();
                                //StartCoroutine(coroutine);
                                Fire();
                            }
                        }
                    }
                    else if (Vector3.Distance(this.transform.position, target.transform.position) > settings.DefensiveTargetForgetDistance)
                    {
                        targetScript.returnChaseKey(out target, out targetScript);
                    }
                    break;
                case (Behavior.Cover):
                    Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.CoverTargetDistance, Color.blue);
                    if (Physics.SphereCast(position, settings.CoverTargetRadius, forward, out hit, settings.CoverTargetDistance, settings.playerTargetMask, QueryTriggerInteraction.Collide))
                    {
                        if (hit.transform.tag == "Player")
                        {
                            if (fireTimer <= 0)
                            {
                                //coroutine = BurstFire();
                                //StartCoroutine(coroutine);
                                Fire();
                            }
                        }
                    }
                    else if (Vector3.Distance(this.transform.position, target.transform.position) > settings.CoverTargetForgetDistance)
                    {
                        targetScript.returnChaseKey(out target, out targetScript);
                    }
                    break;
            }
            
        }

        //Calculate force to avoid other nearby ships
        if (neighborShips > 0)
        {
            Vector3 seperation = SteerDirection(avgAvoidNeighborHeading) * settings.SeperationWeight;
            acceleration += seperation;
        }

        //if (//this is where follow and targetting logic comes in?)

        if (HeadingForCollision())
        {
            Vector3 avoidDir = FindAvoidDir();
            Vector3 avoidForce = Vector3.zero;
            avoidForce = SteerDirection(avoidDir) * settings.AvoidCollisionWeight;
            
            acceleration += avoidForce;
        }
        //else
        //{
        //    acceleration += forward * 2;
        //}

        //update velocity
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;

        switch (behavior)
        {
            case (Behavior.Aggressive):
                speed = Mathf.Clamp(speed, settings.AggressiveMinSpeed, settings.AggressiveMaxSpeed);
                break;
            case (Behavior.Defensive):
                speed = Mathf.Clamp(speed, settings.DefensiveMinSpeed, settings.DefensiveMaxSpeed);
                break;
            case (Behavior.Cover):
                speed = Mathf.Clamp(speed, settings.CoverMinSpeed, settings.CoverMaxSpeed);
                break;
        }

        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    public void SetBehavior(Behavior op)
    {
        this.behavior = op;
    }

    Vector3 SteerDirection(Vector3 inVec)
    {
        Vector3 outVec = Vector3.zero;

        switch (behavior)
        {
            case (Behavior.Aggressive):
                outVec = inVec.normalized * settings.AggressiveMaxSpeed - velocity;
                break;
            case (Behavior.Defensive):
                outVec = inVec.normalized * settings.DefensiveMaxSpeed - velocity;
                break;
            case (Behavior.Cover):
                outVec = inVec.normalized * settings.CoverMaxSpeed - velocity;
                break;
            
        }

        return Vector3.ClampMagnitude(outVec, settings.SteerForceCap);
    }

    bool HeadingForCollision()
    {
        RaycastHit hit;

        switch (behavior)
        {
            case (Behavior.Aggressive):
                if (Physics.SphereCast(position, settings.AggressiveCollisionBoundsRadius, forward, out hit, settings.AggressiveCollisionAvoidDistance, settings.obstacleMask))
                {
                    //debug ray draw
                    //Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.AggressiveCollisionAvoidDistance, Color.red);
                    return true;
                }
                else
                {
                    //debug ray draw
                    //Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.AggressiveCollisionAvoidDistance, Color.green);
                    return false;
                }
            case (Behavior.Defensive):
                if (Physics.SphereCast(position, settings.DefensiveCollisionBoundsRadius, forward, out hit, settings.DefensiveCollisionAvoidDistance, settings.obstacleMask))
                {
                    //debug ray draw
                    //Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.DefensiveCollisionAvoidDistance, Color.red);
                    return true;
                }
                else
                {
                    //debug ray draw
                    //Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.DefensiveCollisionAvoidDistance, Color.green);
                    return false;
                }
            case (Behavior.Cover):
                if (Physics.SphereCast(position, settings.CoverCollisionBoundsRadius, forward, out hit, settings.CoverCollisionAvoidDistance, settings.obstacleMask))
                {
                    //debug ray draw
                    //Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.CoverCollisionAvoidDistance, Color.red);
                    return true;
                }
                else
                {
                    //debug ray draw
                    //Debug.DrawRay(cachedTransform.position, cachedTransform.forward * settings.CoverCollisionAvoidDistance, Color.green);
                    return false;
                }
            default:
                return false;
        }
    }

    Vector3 FindAvoidDir()
    {
        Vector3[] rayDirections = AIHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 newDir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, newDir);
            //Debug
            switch (behavior)
            {
                case (Behavior.Aggressive):
                    Debug.DrawRay(cachedTransform.position, newDir * settings.AggressiveCollisionAvoidDistance, Color.red);
                    if (!Physics.SphereCast(ray, settings.AggressiveCollisionBoundsRadius, settings.AggressiveCollisionAvoidDistance, settings.obstacleMask))
                    {
                        Debug.DrawRay(cachedTransform.position, newDir * settings.AggressiveCollisionAvoidDistance, Color.green);
                        return newDir;
                    }
                    break;
                case (Behavior.Defensive):
                    Debug.DrawRay(cachedTransform.position, newDir * settings.DefensiveCollisionAvoidDistance, Color.red);
                    if (!Physics.SphereCast(ray, settings.DefensiveCollisionBoundsRadius, settings.DefensiveCollisionAvoidDistance, settings.obstacleMask))
                    {
                        Debug.DrawRay(cachedTransform.position, newDir * settings.DefensiveCollisionAvoidDistance, Color.green);
                        return newDir;
                    }
                    break;
                case (Behavior.Cover):
                    Debug.DrawRay(cachedTransform.position, newDir * settings.CoverCollisionAvoidDistance, Color.red);
                    if (!Physics.SphereCast(ray, settings.CoverCollisionBoundsRadius, settings.CoverCollisionAvoidDistance, settings.obstacleMask))
                    {
                        Debug.DrawRay(cachedTransform.position, newDir * settings.CoverCollisionAvoidDistance, Color.green);
                        return newDir;
                    }
                    break;
            }
        }

        return forward;
    }

    //Call this function from bullet/projectile collision function
    public void onDamage(int damage, GameObject shooter)
    {
        Health -= damage;

        if (shooter.tag == "Player")
        {
            //shooter.GetComponent<Player4Base>().getChaseKey(out target, out targetScript);
        }

        if (Health <= 0)
        {
            onDeath();
        }
    }

    public void onDeath()
    {
        if (targetScript)
        {
            targetScript.returnChaseKey(out target, out targetScript);
        }
        Instantiate(DeathEffect, this.transform.position,this.transform.rotation);
        int SpawnCount = UnityEngine.Random.Range(1, 5);
        int WhatCoin = UnityEngine.Random.Range(1, 3);
        if (WhatCoin == 1)
        {
            for (int i = 0; i < SpawnCount; i++)
            {
                Instantiate(DeathDropCopper, this.transform.position, this.transform.rotation);
            }
        }
        else if (WhatCoin == 2)
        {
            for (int i = 0; i < SpawnCount; i++)
            {
                Instantiate(DeathDropSilver, this.transform.position, this.transform.rotation);
            }
        }
        else
        {
            for (int i = 0; i < SpawnCount; i++)
            {
                Instantiate(DeathDropGold, this.transform.position, this.transform.rotation);
            }
        }
        manager.ReturnShipToPool(this.gameObject);
    }

    public void setGameManager(GameManager obj)
    {
        gameManager = obj;
    }

    public void setWaveManager(WaveManager obj)
    {
        manager = obj;
    }

    public void setMaxHealth(Behavior behav)
    {
        switch (behav)
        {
            case (Behavior.Aggressive):
                MaxHealth = settings.AggressiveHealth;
                break;
            case (Behavior.Defensive):
                MaxHealth = settings.DefensiveHealth;
                break;
            case (Behavior.Cover):
                MaxHealth = settings.CoverHealth;
                break;
        }
    }

    public void FillHealth()
    {
        Health = MaxHealth;
    }

    public void setNeighborShipData(int neighbors, Vector3 avoidHeading)
    {
        neighborShips = neighbors;
        avgAvoidNeighborHeading = avoidHeading;
    }

    public void Fire()
    {
        blasterAudio.Play();
        GameObject bullet = Instantiate(bulletPrefab, gunOrigin.transform.position, Quaternion.LookRotation(gunOrigin.transform.forward));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet bull = bullet.GetComponent<bullet>();
        bull.setDamage(bulletDamage);
        bullet.GetComponent<bullet>().setShooter(this.gameObject);
        Vector3 moveDirection = new Vector3(0, 0, bulletSpeed);
        moveDirection = transform.TransformDirection(moveDirection);
        rb.velocity = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
        Destroy(bullet, 5f);

        switch (behavior)
        {
            case (Behavior.Aggressive):
                fireTimer = settings.AggressiveBulletCooldown;
                break;
            case (Behavior.Defensive):
                fireTimer = settings.AggressiveBulletCooldown;
                break;
            case (Behavior.Cover):
                fireTimer = settings.AggressiveBulletCooldown;
                break;
        }
    }

    private IEnumerator BurstFire()
    {
        for (int i = 0; i < settings.bursts; i++)
        {
            yield return new WaitForSeconds(settings.bulletCoolDown);
            Fire();
        }
    }

    void timerTick()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }
    public int getHealth()
    {
        return Health;
    }
    public int getMaxHealth()
    {
        return MaxHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Aster")
        {
            this.onDeath();
        }
    }
}
