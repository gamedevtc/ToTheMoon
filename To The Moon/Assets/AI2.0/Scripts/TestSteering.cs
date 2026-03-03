using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class TestSteering : MonoBehaviour
{
    public enum AIState
    {
        Pursuit = 0,
        Flee,
        Wander,
        Follow,
        AttackPath
    }

    [SerializeField] GameObject currTarget;

    [SerializeField] AIState currState = AIState.Wander;


    [Header("Values")]
    [SerializeField] float minSpeed = 15;
    [SerializeField] float maxSpeed = 100;
    [SerializeField] float shipMass = 200;
    [SerializeField] float normSteerForceCap = 10;
    [SerializeField] float fleeSteerForceCap = 10;

    [SerializeField] float arrivalRadius = 15;
    [SerializeField] float fleeRadius = 3;
    [SerializeField] float exitFleeRadius = 25;

    [Header("Distance to target")]
    [SerializeField] float distance = 0;

    [Header("Wander values")]
    [SerializeField] float circleDistance = 5;
    [SerializeField] float circleRadius = 10;
    [SerializeField] float angleChange = 15;


    [Header("Leader Fields")]
    [SerializeField] GameObject leader;
    [SerializeField] List<GameObject> followers;
    [SerializeField] float leaderFollowDistance;

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 forward;
    private Transform cachedTransform;

    public Vector3 avgAvoidNeighborHeading;
    public int neighborShips = 0;

    //steering
    private Vector3 desiredVelocity;

    private void Start()
    {
        cachedTransform = transform;
        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (minSpeed + maxSpeed) * 0.5f;

        velocity = forward * startSpeed;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 steering = Vector3.zero;
        Vector3 desiredVelocity = Vector3.zero;

        switch (currState)
        {
            case AIState.Pursuit:
                desiredVelocity = Pursuit(currTarget.transform);
                distance = Vector3.Distance(currTarget.transform.position, position);
                if (distance < fleeRadius)
                {
                    currState = AIState.Flee;
                }
                steering = ClampSteerForce(desiredVelocity);
                break;
            case AIState.Flee:
                desiredVelocity = Evade(currTarget.transform);
                distance = Vector3.Distance(currTarget.transform.position, position);
                if (distance > exitFleeRadius)
                {
                    currState = AIState.Pursuit;
                }
                steering = ClampSteerForce(desiredVelocity);
                break;
            case AIState.Wander:
                desiredVelocity = Wander();
                steering = ClampSteerForce(desiredVelocity);
                break;
        }
        //add steering forces to velocity
        velocity += Vector3.ClampMagnitude(steering, maxSpeed);

        //Ensure speed is capped
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        velocity = dir * speed;

        //add velocity to position
        cachedTransform.position += velocity * Time.deltaTime;
        //correct forward
        cachedTransform.forward = velocity.normalized;

        //update caches
        position = cachedTransform.position;
        forward = velocity.normalized;
    }

    Vector3 ClampSteerForce(Vector3 desiredVelocity)
    {
        Vector3 steerForce = Vector3.ClampMagnitude(desiredVelocity, normSteerForceCap);

        return steerForce;
    }

    Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = Vector3.Normalize(targetPos - position);
        //arrival stuff
        float distance = Vector3.Distance(targetPos, position);
        if (distance < arrivalRadius)
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed * (distance / arrivalRadius);
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }

        return desiredVelocity.normalized - velocity;
    }
    Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = Vector3.Normalize(position - targetPos) * maxSpeed;

        return desiredVelocity.normalized - velocity;
    }
    Vector3 Wander()
    {
        Vector3 circleCenter = desiredVelocity;
        circleCenter = circleCenter.normalized * circleDistance;

        Vector3 displacementForce = new Vector3(Random.Range(-angleChange, angleChange), Random.Range(-angleChange, angleChange), Random.Range(-angleChange, angleChange));
        displacementForce = displacementForce * circleRadius;

        Vector3 outVec = circleCenter + displacementForce;
        return outVec;
    }
    Vector3 Pursuit(Transform target)
    {
        float distance = Vector3.Distance(target.position, position);
        float T = distance / maxSpeed;
        Vector3 futurePosition = target.position + target.GetComponent<Rigidbody>().velocity * T;
        return Seek(futurePosition);
    }
    Vector3 Evade(Transform target)
    {
        float distance = Vector3.Distance(target.position, position);
        float T = distance / maxSpeed;
        Vector3 futurePosition = target.position + target.GetComponent<Rigidbody>().velocity * T;
        return Flee(futurePosition);
    }

    //Vector3 LeaderFollow(Transform Leader)
    //{

    //}

    Vector3 modifyForArrival(Vector3 desiredVelocity, float distance)
    {
        Vector3 outVec = Vector3.zero;

        outVec = (desiredVelocity.normalized * maxSpeed) * (distance / arrivalRadius);

        return outVec;
    }

    public void setNeighborShipData(int neighbors, Vector3 avoidHeading)
    {
        neighborShips = neighbors;
        avgAvoidNeighborHeading = avoidHeading;
    }
}
