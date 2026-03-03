using Microsoft.Win32.SafeHandles;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AIUnit : MonoBehaviour, ITeamMember
{
    public enum AIState
    {
        Pursuit = 0,
        Flee,
        Wander,
        Follow,
        AttackPath,
        AttackChase,
        Dead
    }

    [Header("Stuff Set in Prefab")]
    [SerializeField] List<AttackPath> attackPaths;
    [SerializeField] GameObject DeathEffect;
    [SerializeField] List<GameObject> coinPrefabs;
    [SerializeField] float coinInitialVelocity = 15;
    [SerializeField] float coinBlastDirectionalStrength = 5;
    [SerializeField] int minCoinDrop = 3;
    [SerializeField] int maxCoinDrop = 8;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Collider myCollider;
    [SerializeField] GameObject warpSphere;
    [SerializeField] Animation warpAnim;
    [SerializeField] GameObject trailRendererNormal;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] Transform gunOrigin1;
    [SerializeField] Transform gunOrigin2;
    [SerializeField] Rigidbody myRB;
    [SerializeField] public BehaviourSettings stats;
    [SerializeField] public PathingVisualizer pather;

    [Header("Debug only values")]
    [SerializeField] private AIState currState = AIState.Wander;
    [SerializeField] private int myTeam;
    [SerializeField] private GameObject currTarget;
    [SerializeField] private Rigidbody currTargetRB;
    [SerializeField] private AttackPath currAttackPath;
    [SerializeField] private int currAttackPathPointIndex;
    [SerializeField] private int attackPathIndex;
    [SerializeField] private float currHealth;
                     
    [SerializeField] private GameObject currLeader;
    [SerializeField] private Rigidbody currLeaderRB;
    [SerializeField] private int followerIndex;
    [SerializeField] private List<GameObject> followers;
    [SerializeField] private List<int> takenAttackPaths = new List<int>();
    [SerializeField] private bool paused = false;
    [SerializeField] private bool debugIgnoreEnable = false;
    [SerializeField] private bool debugNoManager = false;

    //internal numbers
    [SerializeField] private List<float> minSpeeds = new List<float>();
    [SerializeField] private List<float> maxSpeeds = new List<float>();
    [SerializeField] private List<float> steerForceCaps = new List<float>();
    [SerializeField] private List<float> collisionAvoidForceWeights = new List<float>();
    [SerializeField] private List<float> separationForceWeights = new List<float>();
    [SerializeField] private List<float> BehaviourForceWeights = new List<float>();

    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private Vector3 velocity = Vector3.zero;
    [SerializeField] private Vector3 forward = Vector3.zero;

    [SerializeField] private Vector3 avgAvoidNeighborHeading;
    [SerializeField] private int neighborShips = 0;

    [SerializeField] private float fleeTime = 0;
    [SerializeField] private float underAttackTime = 0;
    [SerializeField] private float pursueTime = 0;
    [SerializeField] private float chaseTime = 0;
    [SerializeField] private float wanderTime = 0;
    [SerializeField] private float followFriendTime = 0;
    [SerializeField] private float fireCooldownTime = 0;
    [SerializeField] private float stunnedTime = 0;
    [SerializeField] private bool pathIsComplete = false;
    [SerializeField] private float wanderInterval = 0;

    public void FixedUpdate()
    {
        if (paused)
        {
            return;
        }
        if (stunnedTime > 0)
        {
            stunnedTime -= Time.deltaTime;
            return;
        }
        timerTick();
        pather.clearAll();

        //velocity = myRB.velocity;
        switch (currState)
        {
            case AIState.Pursuit:
                checkToFire();
                break;
            case AIState.Flee:
                break;
            case AIState.Wander:
                break;
            case AIState.Follow:
                break;
            case AIState.AttackPath:
                checkToFire();
                break;
            case AIState.AttackChase:
                checkToFire();
                break;
            case AIState.Dead:
                break;
            default:
                break;
        }
        
        Vector3 steering = Vector3.zero;

        //Find collision check force
        Vector3 collisionAvoidForce = Vector3.zero;
        float collisionCheckDistance = 0;
        //float collisionAvoidModifier = 0;
        //float otherCollisionAvoidModifier = 0;
        bool headingForCollision = isHeadingForCollision(out collisionCheckDistance);
        if (headingForCollision)
        {
            //Dynamically Modify collision avoid weight based on importance (this may need to be more involved in the future)
            collisionAvoidForce = FindAvoidDir();
            //otherCollisionAvoidModifier = (collisionCheckDistance / stats.CollisionAvoidRayDistance);
            //collisionAvoidModifier = 1 - (collisionCheckDistance / stats.CollisionAvoidRayDistance * 2);
        }

        //Find separation force
        Vector3 separationForce = avgAvoidNeighborHeading;

        checkState();
        //Set behavior steering force
        Vector3 behaviorForce = BehaviourForce();

        //Create steering Force from other forces
        if (headingForCollision)
        {
            steering = ClampSteerForce(collisionAvoidForce * collisionAvoidForceWeights[(int)currState] + separationForce * separationForceWeights[(int)currState]);
        }
        else
        {
            steering = ClampSteerForce(behaviorForce * BehaviourForceWeights[(int)currState] + separationForce * separationForceWeights[(int)currState]);
        }
        
        //steering = ClampSteerForce((otherCollisionAvoidModifier * behaviorForce * BehaviourForceWeights[(int)currState])
        //    + (collisionAvoidForce * (collisionAvoidForceWeights[(int)currState] * collisionAvoidModifier))
        //    + (otherCollisionAvoidModifier * separationForce * separationForceWeights[(int)currState]));

        //add steering forces to velocity
        velocity += Vector3.ClampMagnitude(velocity + steering, maxSpeeds[(int)currState]);

        pather.drawRed();
        pather.drawGreen();
        //Ensure speed is capped
        float speed = velocity.magnitude;
        if (speed > 0)
        {
            Vector3 dir = velocity / speed;
            speed = Mathf.Clamp(speed, minSpeeds[(int)currState], maxSpeeds[(int)currState]);
            velocity = dir * speed;

            //add velocity to position
            transform.position += velocity * Time.deltaTime;
            //correct forward
            transform.forward = dir;

            //update caches
            position = transform.position;
            forward = dir;
        }
        else
        {
            float startSpeed = (minSpeeds[(int)currState] + maxSpeeds[(int)currState]) * 0.5f;

            velocity = forward * startSpeed;
        }
    }

    void checkState()
    {
        float distanceToTarget = 0;
        bool switchingStates = false;
        if (currTarget)
        {
            distanceToTarget = Vector3.Distance(currTarget.transform.position, position);
        }
        //check state changing conditions and switch here
        switch (currState)
        {
            case AIState.Pursuit:
                if (distanceToTarget > stats.TargetForgetDistance)//switch to wander if target gets too far away
                {
                    DropTarget();

                    currState = AIState.Wander;
                    wanderTime = stats.WanderBeforeSearchingTime;
                    switchingStates = true;
                }
                if (distanceToTarget < stats.TooCloseRadius)//switch to flee if too close to target
                {
                    currState = AIState.Flee;
                    fleeTime = stats.TooCloseFleeTime;
                    switchingStates = true;
                }
                if (pursueTime <= 0)//switch to wander if we've been pursuing for too long
                {
                    currState = AIState.Wander;
                    wanderTime = stats.WanderBeforeSearchingTime;
                    switchingStates = true;
                }
                if (switchingStates)
                {
                    pursueTime = 0;
                    return;
                }
                break;
            case AIState.Flee:
                if (fleeTime <= 0)
                {
                    if (distanceToTarget > stats.TargetForgetDistance)//switch to wander if we're done fleeing and target is too far away
                    {
                        DropTarget();

                        currState = AIState.Wander;
                        wanderTime = stats.WanderBeforeSearchingTime;
                        switchingStates = true;
                    }
                    else//switch back to pursuit if still within range
                    {
                        currState = AIState.Pursuit;
                        pursueTime = stats.GiveUpPursuitTime;
                        switchingStates = true;
                    }
                }
                if (switchingStates)
                {
                    fleeTime = 0;
                    return;
                }
                break;
            case AIState.Wander:
                if (wanderTime <= 0)//if we've wandered long enough
                {
                    if (CheckForTargetOrLeader())//look for a target around us
                    {
                        if (currTarget && currAttackPath)//make sure we got a path from the target
                        {
                            if (currAttackPath.isPursuitKey)//switch to pursuing
                            {
                                currState = AIState.Pursuit;
                                pursueTime = stats.GiveUpPursuitTime;
                                switchingStates = true;
                            }
                            else if (currAttackPath.isChaseKey)//switch to chasing
                            {
                                currState = AIState.AttackChase;
                                chaseTime = stats.GiveUpChaseTime;
                                switchingStates = true;
                            }
                            else if (currAttackPath.isPathKey)
                            {
                                currState = AIState.AttackPath;
                                switchingStates = true;
                            }
                        }
                        else if (currLeader)
                        {
                            currState = AIState.Follow;
                            followFriendTime = stats.leaderFollowTime;
                            switchingStates = true;
                        }
                    }
                }
                if (switchingStates)
                {
                    wanderTime = 0;
                    return;
                }
                break;
            case AIState.Follow:
                if (followFriendTime <= 0)
                {
                    DropLeader();

                    currState = AIState.Wander;
                    wanderTime = stats.WanderBeforeSearchingTime;
                    switchingStates = true;
                }
                if (switchingStates)
                {
                    followFriendTime = 0;
                    return;
                }
                break;
            case AIState.AttackPath:
                if (pathIsComplete)//return to wandering once attack path is executed
                {
                    DropTarget();
                    currState = AIState.Wander;
                    wanderTime = stats.WanderBeforeSearchingTime;
                    switchingStates = true;
                }
                if (switchingStates)
                {
                    return;
                }
                break;
            case AIState.AttackChase:
                if (distanceToTarget > stats.TargetForgetDistance)//switch to wander if target gets too far away
                {
                    DropTarget();
                    currState = AIState.Wander;
                    wanderTime = stats.WanderBeforeSearchingTime;
                    switchingStates = true;
                }
                if (distanceToTarget < stats.TooCloseRadius)//switch to flee if too close to target
                {
                    currState = AIState.Flee;
                    fleeTime = stats.TooCloseFleeTime;
                    switchingStates = true;
                }
                if (chaseTime <= 0)//give up chase if we've been chasing too long
                {
                    DropTarget();
                    currState = AIState.Wander;
                    wanderTime = stats.WanderBeforeSearchingTime;
                    switchingStates = true;
                }
                if (switchingStates)
                {
                    chaseTime = 0;
                    return;
                }
                break;
            case AIState.Dead:
                break;
            default:
                break;
        }
    }

    Vector3 BehaviourForce()
    {
        switch (currState)
        {
            case AIState.Pursuit:
                if (currTarget)
                {
                    return Pursuit(currTarget.transform, currTarget.GetComponent<Rigidbody>());
                }
                else
                {
                    currState = AIState.Wander;
                }
                break;
            case AIState.Flee:
                if (currTarget)
                {
                    return Evade(currTarget.transform, currTarget.GetComponent<Rigidbody>());
                }
                else
                {
                    currState = AIState.Wander;
                }
                break;
            case AIState.Wander:
                if (wanderInterval <= 0)
                {
                    wanderInterval = stats.wanderChangeTime;
                    return Wander();
                }
                return Vector3.zero;
            case AIState.Follow:
                if (currLeader)
                {
                    return Follow(currLeader.transform, currLeaderRB);
                }
                else
                {
                    currState = AIState.Wander;
                }
                break;
            case AIState.AttackPath:
                if (currAttackPath)
                {
                    return AttackPath(currAttackPath);
                }
                else
                {
                    currState = AIState.Wander;
                }
                break;
            case AIState.AttackChase:
                if (currTarget)
                {
                    return Follow(currTarget.transform, currTargetRB);
                }
                else
                {
                    currState = AIState.Wander;
                }
                break;
            case AIState.Dead:
                break;
            default:
                break;
        }
        return Vector3.zero;
    }

    bool CheckForTargetOrLeader()
    {
        Collider[] colliders;
        colliders = Physics.OverlapSphere(position, stats.TargetDetectRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != myCollider)
            {
                ITeamMember targettable = null;
                if (colliders[i].gameObject.TryGetComponent<ITeamMember>(out targettable))
                {
                    if (targettable.getTeamNumber() != myTeam)
                    {
                        if (targettable.isAttackable(out currAttackPath, out attackPathIndex))
                        {
                            return AddTarget(colliders[i].gameObject);
                        }
                    }
                    else if (targettable.getTeamNumber() == myTeam)
                    {
                        if (!stats.IsLeader && targettable.canHaveFollowers(this.gameObject, out followerIndex))
                        {
                            return AddLeader(colliders[i].gameObject);
                        }
                    }
                }
            }
        }
        return false;
    }

    #region ITeamMember Functions
    public int getTeamNumber()
    {
        return myTeam;
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

    public bool canHaveFollowers(GameObject follower, out int index)
    {
        int returnIndex = 0;
        bool canFollow = false;
        if (!stats.IsLeader)
        {
            index = 0;
            return false;
        }
        else
        {
            for (int i = 0; i < followers.Count; i++)
            {
                if (followers[i] == null)
                {
                    returnIndex = i;
                    canFollow = true;
                    break;
                }
            }
            if (!canFollow)
            {
                index = 0;
                return false;
            }
            else
            {
                followers[returnIndex] = follower;
            }
        }
        index = returnIndex;
        return true;
    }

    public void returnAttackIndex(int index)
    {
        takenAttackPaths.Remove(index);
    }

    public void returnFollowIndex(int index)
    {
        followers[index] = null;
    }
    #endregion

    #region Add/Drop Target Functions
    bool AddTarget(GameObject target)
    {
        currTarget = target;
        currTargetRB = target.GetComponent<Rigidbody>();
        if (!currTargetRB)
        {
            return false;
        }
        currAttackPathPointIndex = 0;
        pathIsComplete = false;
        return true;
    }
    void DropTarget()
    {
        currTarget.GetComponent<ITeamMember>().returnAttackIndex(attackPathIndex);
        currTarget = null;
        currTargetRB = null;
        currAttackPath = null;
        pathIsComplete = false;
        attackPathIndex = 0;
        currAttackPathPointIndex = 0;
    }
    bool AddLeader(GameObject target)
    {
        currLeader = target.gameObject;
        currLeaderRB = target.GetComponent<Rigidbody>();
        if (!currLeaderRB)
        {
            return false;
        }
        return true;
    }
    void DropLeader()
    {
        currLeader.GetComponent<ITeamMember>().returnFollowIndex(followerIndex);
        currLeader = null;
        currLeaderRB = null;
    }
    #endregion

    void checkToFire()
    {
        bool canFire = false;
        switch (currState)
        {
            case AIState.Pursuit:
                canFire = true;
                break;
            case AIState.Flee:
                break;
            case AIState.Wander:
                break;
            case AIState.Follow:
                break;
            case AIState.AttackPath:
                canFire = true;
                break;
            case AIState.AttackChase:
                canFire = true;
                break;
            case AIState.Dead:
                break;
            default:
                break;
        }

        if (canFire)
        {
            if (fireCooldownTime <= 0)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, stats.fireSightRadius, forward, out hit, stats.fireSightDistance))
                {
                    if (hit.transform.gameObject == currTarget)
                    {
                        Debug.Log("Ship " + transform.GetInstanceID() + " saw ship " + currTarget.GetInstanceID() + "And would fire");
                        fireCommand();
                        if (stats.IsLeader)
                        {
                            for (int i = 0; i < followers.Count; i++)
                            {
                                followers[i].GetComponent<AIUnit>().fireCommand();
                            }
                        }
                        fireCooldownTime = stats.BulletCooldown;
                    }
                }
            }
        }
    }

    public void fireCommand()
    {
        if (GameManagerBase.Instance.isMulti())
        {
            GameObject bull = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.bulletPrefabMulti.name), gunOrigin1.transform.position, Quaternion.LookRotation(gunOrigin1.transform.forward));
            bull.GetComponent<ProjectileBase>().setShooter(gameObject);
            GameObject bull2 = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", stats.bulletPrefabMulti.name), gunOrigin2.transform.position, Quaternion.LookRotation(gunOrigin2.transform.forward));
            bull2.GetComponent<ProjectileBase>().setShooter(gameObject);
        }
        else
        {
            GameObject bull = Instantiate(stats.bulletPrefabSingle, gunOrigin1.transform.position, Quaternion.LookRotation(gunOrigin1.transform.forward));
            bull.GetComponent<ProjectileBase>().setShooter(gameObject);
            GameObject bull2 = Instantiate(stats.bulletPrefabSingle, gunOrigin2.transform.position, Quaternion.LookRotation(gunOrigin2.transform.forward));
            bull2.GetComponent<ProjectileBase>().setShooter(gameObject);
        }
    }

    void timerTick()
    {
        if (fleeTime > 0)
        {
            fleeTime -= Time.deltaTime;
        }
        if (pursueTime > 0)
        {
            pursueTime -= Time.deltaTime;
        }
        if (wanderTime > 0)
        {
            wanderTime -= Time.deltaTime;
        }
        if (underAttackTime > 0)
        {
            underAttackTime -= Time.deltaTime;
        }
        if (chaseTime > 0)
        {
            chaseTime -= Time.deltaTime;
        }
        if (followFriendTime > 0)
        {
            followFriendTime -= Time.deltaTime;
        }
        if (fireCooldownTime > 0)
        {
            fireCooldownTime -= Time.deltaTime;
        }
        if (wanderInterval > 0)
        {
            wanderInterval -= Time.deltaTime;
        }
    }

    Vector3 ClampSteerForce(Vector3 desiredVelocity)
    {
        Vector3 steerForce = Vector3.ClampMagnitude(desiredVelocity, steerForceCaps[(int)currState]);

        return steerForce;
    }

    Vector3 Seek(Vector3 targetPos)
    {
        Vector3 desiredVelocity = Vector3.Normalize(targetPos - position);
        //arrival stuff
        float distance = Vector3.Distance(targetPos, position);
        if (distance < stats.ArrivalRadius)
        {
            desiredVelocity = desiredVelocity.normalized * (maxSpeeds[(int)currState] * (distance / (stats.ArrivalRadius+stats.ArrivalMaxCloseness)));
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeeds[(int)currState];
        }

        return desiredVelocity - velocity;
    }

    Vector3 Pursuit(Transform target, Rigidbody targetRigidBody)
    {
        float distance = Vector3.Distance(target.position, position);
        float T = distance / maxSpeeds[(int)currState];
        Vector3 futurePosition = target.position + targetRigidBody.velocity * T;
        return Seek(futurePosition);
    }

    Vector3 Flee(Vector3 targetPos)
    {
        Vector3 desiredVelocity = Vector3.Normalize(position - targetPos) * maxSpeeds[(int)currState];

        return desiredVelocity - velocity;
    }
    Vector3 Evade(Transform target, Rigidbody targetRigidBody)
    {
        float distance = Vector3.Distance(target.position, position);
        float T = distance / maxSpeeds[(int)currState];
        Vector3 futurePosition = target.position + (targetRigidBody.velocity * T);
        return Flee(futurePosition);
    }
    Vector3 Wander()
    {
        Vector3 circleCenter = velocity;
        circleCenter = circleCenter.normalized * stats.WanderCircleDistance;

        Vector3 displacementForce = new Vector3(Random.Range(-stats.WanderAngleChange, stats.WanderAngleChange), Random.Range(-stats.WanderAngleChange, stats.WanderAngleChange), Random.Range(-stats.WanderAngleChange, stats.WanderAngleChange));
        displacementForce = displacementForce * stats.WanderCircleRadius;

        Vector3 outVec = circleCenter + displacementForce;
        return outVec;
    }
    Vector3 Follow(Transform followTarget, Rigidbody targetRigidBody)
    {
        Vector3 tv = targetRigidBody.velocity * -1;
        switch (currState)
        {
            case AIState.Follow:
                tv = tv.normalized * stats.leaderFollowDistance;
                break;
            case AIState.AttackChase:
                tv = tv.normalized * currAttackPath.chaseDistance;
                break;
            default:
                return Vector3.zero;
        }
        Vector3 behindPos = followTarget.position + tv;

        return Seek(behindPos);
    }

    Vector3 AttackPath(AttackPath path)
    {
        if (path == null)
        {
            return Vector3.zero;
        }
        Vector3 currNode = path.nodes[currAttackPathPointIndex].position;
        if (Vector3.Distance(currNode, position) < path.nodeRadius)
        {
            currAttackPathPointIndex += 1;
        }
        else
        {
            return Seek(currNode);
        }
        if (currAttackPathPointIndex >= path.nodes.Count)
        {
            pathIsComplete = true;
            return Vector3.zero;
        }
        else
        {
            currNode = path.nodes[currAttackPathPointIndex].position;
        }
        return Seek(currNode);
    }

    bool isHeadingForCollision(out float distance)
    {
        RaycastHit hit;

        if (Physics.SphereCast(position, stats.CollisionAvoidRayRadius, forward, out hit, stats.CollisionAvoidRayDistance, obstacleMask))
        {
            //debug ray draw
            if (GameManagerBase.Instance.getDebug_showAICollision())
            {
                
                pather.addLine(PathingVisualizer.visColors.Red, transform.position, transform.position + transform.forward * stats.CollisionAvoidRayDistance);
                Debug.DrawRay(transform.position, transform.forward * stats.CollisionAvoidRayDistance, Color.red);
            }
            distance = hit.distance;
            return true;
        }
        else
        {
            //debug ray draw
            if (GameManagerBase.Instance.getDebug_showAICollision())
            {
                pather.addLine(PathingVisualizer.visColors.Green, transform.position, transform.position + transform.forward * stats.CollisionAvoidRayDistance);
                Debug.DrawRay(transform.position, transform.forward * stats.CollisionAvoidRayDistance, Color.green);
            }
            distance = 0;
            return false;
        }
    }

    Vector3 FindAvoidDir()
    {
        Vector3[] rayDirections = AIAssistant.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 newDir = transform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, newDir);
            //Debug
            if (GameManagerBase.Instance.getDebug_showAICollision())
            {
                pather.addLine(PathingVisualizer.visColors.Red, transform.position, transform.position + newDir * stats.CollisionAvoidRayDistance);
                Debug.DrawRay(transform.position, newDir * stats.CollisionAvoidRayDistance, Color.red);
            }
            if (!Physics.SphereCast(ray, stats.CollisionAvoidRayRadius, stats.CollisionAvoidRayDistance, obstacleMask))
            {
                if (GameManagerBase.Instance.getDebug_showAICollision())
                {
                    pather.addLine(PathingVisualizer.visColors.Green, transform.position, transform.position + newDir * stats.CollisionAvoidRayDistance);
                    Debug.DrawRay(transform.position, newDir * stats.CollisionAvoidRayDistance, Color.green);
                }
                return newDir;
            }
        }
        return forward;
    }

    public AIState getState()
    {
        return currState;
    }

    public void setNeighborShipData(int neighbors, Vector3 avoidHeading)
    {
        neighborShips = neighbors;
        avgAvoidNeighborHeading = avoidHeading;
    }

    public void InitAI(int team, Material teamColor)
    {
        myTeam = team;
        currHealth = stats.Health;
        followers = new List<GameObject>(stats.NumOfAllowedFollowers);
        InitStatLists();
        setShipColor(teamColor);

        currState = AIState.Wander;

        position = transform.position;
        forward = transform.forward;

        float startSpeed = (minSpeeds[(int)currState] + maxSpeeds[(int)currState]) * 0.5f;

        velocity = forward * startSpeed;
    }

    void setShipColor(Material teamColor)
    {
        meshRenderer.sharedMaterial = teamColor;
    }

    void InitStatLists()
    {
        minSpeeds = new List<float>();
        maxSpeeds = new List<float>();
        steerForceCaps = new List<float>();
        collisionAvoidForceWeights = new List<float>();
        separationForceWeights = new List<float>();
        BehaviourForceWeights = new List<float>();

        minSpeeds.Add(stats.PursuitMinSpeed);
        minSpeeds.Add(stats.FleeMinSpeed);
        minSpeeds.Add(stats.WanderMinSpeed);
        minSpeeds.Add(stats.FollowMinSpeed);
        minSpeeds.Add(stats.AttackMinSpeed);
        minSpeeds.Add(stats.ChaseMinSpeed);
        minSpeeds.Add(stats.EscapeMinSpeed);

        maxSpeeds.Add(stats.PursuitMaxSpeed);
        maxSpeeds.Add(stats.FleeMaxSpeed);
        maxSpeeds.Add(stats.WanderMaxSpeed);
        maxSpeeds.Add(stats.FollowMaxSpeed);
        maxSpeeds.Add(stats.AttackMaxSpeed);
        maxSpeeds.Add(stats.ChaseMaxSpeed);
        maxSpeeds.Add(stats.EscapeMaxSpeed);

        steerForceCaps.Add(stats.PursuitSteerForceCap);
        steerForceCaps.Add(stats.FleeSteerForceCap);
        steerForceCaps.Add(stats.WanderSteerForceCap);
        steerForceCaps.Add(stats.FollowSteerForceCap);
        steerForceCaps.Add(stats.AttackSteerForceCap);
        steerForceCaps.Add(stats.ChaseSteerForceCap);
        steerForceCaps.Add(stats.EscapeSteerForceCap);

        collisionAvoidForceWeights.Add(stats.CollisionAvoidForceWeightPursuit);
        collisionAvoidForceWeights.Add(stats.CollisionAvoidForceWeightFlee);
        collisionAvoidForceWeights.Add(stats.CollisionAvoidForceWeightWander);
        collisionAvoidForceWeights.Add(stats.CollisionAvoidForceWeightFollow);
        collisionAvoidForceWeights.Add(stats.CollisionAvoidForceWeightPursuit);
        collisionAvoidForceWeights.Add(stats.CollisionAvoidForceWeightChase);
        collisionAvoidForceWeights.Add(stats.CollisionAvoidForceWeightEscape);

        separationForceWeights.Add(stats.SeparationForceWeightPursuit);
        separationForceWeights.Add(stats.SeparationForceWeightFlee);
        separationForceWeights.Add(stats.SeparationForceWeightWander);
        separationForceWeights.Add(stats.SeparationForceWeightFollow);
        separationForceWeights.Add(stats.SeparationForceWeightPursuit);
        separationForceWeights.Add(stats.SeparationForceWeightChase);
        separationForceWeights.Add(stats.SeparationForceWeightEscape);

        BehaviourForceWeights.Add(stats.BehaviourForceWeightPursuit);
        BehaviourForceWeights.Add(stats.BehaviourForceWeightFlee);
        BehaviourForceWeights.Add(stats.BehaviourForceWeightWander);
        BehaviourForceWeights.Add(stats.BehaviourForceWeightFollow);
        BehaviourForceWeights.Add(stats.BehaviourForceWeightPursuit);
        BehaviourForceWeights.Add(stats.BehaviourForceWeightChase);
        BehaviourForceWeights.Add(stats.BehaviourForceWeightEscape);
    }

    public void TakeDamage(float damage, GameObject shooter)
    {
        currHealth -= damage;

        if (currHealth <= 0 && currState != AIState.Dead)
        {
            currState = AIState.Dead;
            onDeath();
        }

        if (currState == AIState.Wander)
        {
            AddTarget(shooter);
            currState = AIState.Flee;
        }
    }

    public void onDeath()
    {
        if (currTarget)
        {
            DropTarget();
        }
        if (currLeader)
        {
            DropLeader();
        }

        Instantiate(DeathEffect, this.transform.position, this.transform.rotation);

        int coinCount = Random.Range(minCoinDrop, maxCoinDrop);
        for (int i = 0; i < coinCount; i++)
        {
            int coinIndex = Random.Range(0, coinPrefabs.Count);
            Vector3 Direction = new Vector3(Random.Range(-coinBlastDirectionalStrength, coinBlastDirectionalStrength), Random.Range(-coinBlastDirectionalStrength, coinBlastDirectionalStrength), Random.Range(-coinBlastDirectionalStrength, coinBlastDirectionalStrength));
            Direction *= coinInitialVelocity;
            GameObject coin = Instantiate(coinPrefabs[coinIndex], transform.position, transform.rotation);
            coin.GetComponent<Rigidbody>().velocity = Direction;
        }

        AIManager.Instance.ReturnShipToPool(this.gameObject);
    }

    public void WarpIn(Vector3 pos)
    {
        trailRendererNormal.SetActive(false);
        warpSphere.SetActive(true);
        position = pos;
        transform.position = pos;
        warpAnim.Play();
        trailRendererNormal.SetActive(true);
        unPause();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Aster")
        {
            this.onDeath();
        }
    }

    public void setStunnedTime(float stun)
    {
        stunnedTime = stun;
    }

    #region pausestuff
    private void OnEnable()
    {
        if (!debugIgnoreEnable)
        {
            //if (!GameManagerBase.Instance.isMulti())
            //{
            //    //add pause and unpause to event
            EventManager.pauseEvent += Pause;
            EventManager.unPauseEvent += unPause;
            //}
        }
    }
    private void OnDisable()
    {
        if (!debugIgnoreEnable)
        {
         //   if (!GameManagerBase.Instance.isMulti())
          //  {
                //remove from event
                EventManager.pauseEvent -= Pause;
                EventManager.unPauseEvent -= unPause;
          //  }
        }
    }

    public void Pause()
    {
        paused = true;
    }

    public void unPause()
    {
        paused = false;
    }
    #endregion
}
