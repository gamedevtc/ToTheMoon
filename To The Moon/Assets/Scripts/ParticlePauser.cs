using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePauser : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.pauseEvent += pause;
        EventManager.unPauseEvent += unPause;
    }
    public void OnDisable()
    {
        EventManager.pauseEvent -= pause;
        EventManager.unPauseEvent -= unPause;
        GetComponent<ParticleSystem>().Stop();
    }

    private void pause()
    {
        GetComponent<ParticleSystem>().Pause();
    }
    private void unPause()
    {
        GetComponent<ParticleSystem>().Play();
    }
}
