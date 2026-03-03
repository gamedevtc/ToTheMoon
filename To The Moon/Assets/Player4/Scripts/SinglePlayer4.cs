using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer4 : Player4Base
{
    protected override void Start()
    {
        base.Start();

        //This should probably be in Player Manager for multiplayer
        Transform newPos = LevelBuilder.Instance.getOpenSpawn();
        if (newPos != null)
        {
            transform.position = newPos.position;
            transform.rotation = newPos.rotation;
        }
        
    }

    protected override void Update()
    {
        if (paused)
        {
            if (Input.GetKeyDown(pauseKey) || (ControllerEnabled && Input.GetButton(pauseControl)))
            {
                EventManager.unPause();
            }
            return;
        }
        
        TimerTick();

        if (!canEscape)
        {
            EscapeCheck();

            if (escapeTime <= 0)
            {
                TakeDamage(ship.stats().maxHealth, null);
            }
        }

        base.Update();

        if (Input.GetKeyDown(pauseKey) || (ControllerEnabled && Input.GetButton(pauseControl)))
        {
            if (!paused)
            {
                EventManager.pause();
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (paused)
        {
            return;
        }
        base.FixedUpdate();
    }

    public override void TakeDamage(float damage, GameObject shooter)
    {
        if (!GodMode)
        {
            currHealth -= damage;
            if (CameraShaker.Instance)
            {
                CameraShaker.Instance.Shake();
            }
        }
        if (!GodMode && currHealth <= 0 && !dead)
        {
            dead = true;
            if (shooter)
            {
                killedBy = shooter;
            }
            OnDeath();
        }
    }

    public override void OnDeath()
    {
        paused = true;
        GameManagerBase.Instance.setState(GameManagerBase.gameState.Lose);
        Instantiate(deathEffect, transform.position, transform.rotation);
        cameraScript.setCameraActive(false);
        GameObject deathCam = Instantiate(deathCamera, LevelBuilder.Instance.getOpenSpawn().position, transform.rotation);
        if (killedBy)
        {
            deathCam.GetComponent<DeathCameraScript>().setTarget(killedBy);
        }
        cachedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(ship.stats().maxHealth, null);
            other.GetComponent<AIUnit>().TakeDamage((int)ship.stats().maxHealth, this.gameObject);
        }

        if (other.CompareTag("Aster"))
        {
            TakeDamage(ship.stats().maxHealth, null);
        }
    }

    private void OnEnable()
    {
        //add pause and unpause to event
        EventManager.pauseEvent += Pause;
        EventManager.unPauseEvent += unPause;
    }
    private void OnDisable()
    {
        //remove from event
        EventManager.pauseEvent -= Pause;
        EventManager.unPauseEvent -= unPause;
    }
    public override void Pause()
    {
        paused = true;
        cachedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameManagerBase.Instance.setState(GameManagerBase.gameState.Pause);
    }

    public override void unPause()
    {
        paused = false;
        rb.velocity = cachedVelocity;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManagerBase.Instance.setState(GameManagerBase.gameState.Running);
    }
}
