using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
    [SerializeField] private float tumble = 0.15f;
    [SerializeField] Vector3 velStore;
    [SerializeField] Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(Random.Range(-tumble, tumble), Random.Range(-tumble, tumble), Random.Range(-tumble, tumble));
    }

    private void OnEnable()
    {
        EventManager.pauseEvent += Pause;
        EventManager.unPauseEvent += unPause;
    }

    private void OnDisable()
    {
        //remove from event
        EventManager.pauseEvent -= Pause;
        EventManager.unPauseEvent -= unPause;
    }

    void Pause()
    {
        velStore = rb.angularVelocity;
        rb.angularVelocity = Vector3.zero;
    }
    void unPause()
    {
        rb.angularVelocity = velStore;
    }
}