using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Missile : MonoBehaviour
{
    [Header("Set in Prefab")]
    [SerializeField] int damage;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float steerForceCap;
    [SerializeField] float targetWeight;
    [SerializeField] float aliveTime;

    [Header("Set in script")]
    [SerializeField] GameObject target;
    [SerializeField] GameObject shooter;
    [SerializeField] string LockTag;
    Vector3 position;
    Vector3 velocity;
    Transform cachedTransform;

    bool paused = false;

    // Start is called before the first frame update
    void Awake()
    {
        cachedTransform = transform;
        position = cachedTransform.position;
        float startSpeed = (minSpeed + maxSpeed) * 0.5f;
        velocity = transform.forward * startSpeed;
        Destroy(this.gameObject, aliveTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (paused)
        {
            return;
        }
        
        Vector3 acceleration = Vector3.zero;
        if (target != null)
        {
            Vector3 offsetToTarget = (target.transform.position - position);
            Vector3 outVec = offsetToTarget.normalized * maxSpeed - velocity;
            outVec = Vector3.ClampMagnitude(outVec, steerForceCap);

            acceleration += outVec * targetWeight;
        }
        else
        {
            acceleration += minSpeed * transform.forward;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;

        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
    }

    public void setTarget(GameObject targ, string tag, GameObject shotby)
    {
        target = targ;
        LockTag = tag;
        shooter = shotby;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Missile hit " + collision.tag);
        if (collision.transform.CompareTag(LockTag) && LockTag == "Enemy")
        {
            Debug.Log("Collision check good");
            //other.gameObject.GetComponent<Script>().TakeDamage(bulletDamage);
            //other.gameObject.transform.SetPositionAndRotation(new Vector3(1000, 1000, 1000), new Quaternion(0, 0, 0, 0));
            collision.gameObject.GetComponent<AIController>().onDamage(damage, shooter);
            Destroy(gameObject);
        }
    }
}
