using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : CharacterControl
{
    //Movement
    public NavMeshAgent agent;
    public Transform target;

    private float visionRange;
    private float rotateSpeed = 5.0f;
    private float minDistance;
    public float distance;

    private float idleTime = 5.0f;
    private float idleCounter = 0.0f;
    private int idleDistance = 10;

    System.Random prng = new System.Random();

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }
    protected override void Start()
    {
        base.Start();
        //Set nevMeshAgent and speed
        agent.speed = runningSpeed;

        //Set enemy's target to player
        target = PlayerManager.instance.player.transform;
        minDistance = characterCombat.myStats.statData.attackRange.GetValue() * 0.5f;
        visionRange = characterCombat.myStats.statData.visionRange.GetValue();
    }

    protected override void Update()
    {
        base.Update();
        distance = Vector3.Distance(transform.position, target.position);

        Behaviour();
           
    }

    protected virtual void Behaviour()
    {
        //Movement Follow and Attack


        if (!isKnockBack)
        {
            if (!isGround)
            {
                controller.Move(new Vector3(controller.velocity.x, -yVelocity, controller.velocity.z) * Time.deltaTime);
                agent.enabled = false;
            }
            else
            {
                agent.enabled = true;
                if (distance < visionRange)
                {
                    ChasePlayer();
                }
                else
                {
                    Idle();
                }
            }

        }
    }

    private void ChasePlayer()
    {
        FacePlayer();
        if (isAttacking)
        {
            agent.speed = 0;
        }
        else
        {
            agent.destination = target.position + Vector3.Normalize(transform.position - target.position) * minDistance;
            agent.speed = runningSpeed;
        }
       
    }

    private void Idle()
    {
        if (idleCounter > 0)
        {
            idleCounter -= Time.deltaTime;
        }
        else
        {
            idleCounter = idleTime;
            agent.speed = walkingSpeed;
            Vector3 newDestination = new Vector3(controller.transform.position.x + prng.Next(-idleDistance, idleDistance), 0, controller.transform.position.z + prng.Next(-idleDistance, idleDistance));
            agent.destination = newDestination;
        }
    }

    public void FacePlayer()
    {
        Vector3 lookDirection = target.position - transform.position;
        Quaternion rotateDirection = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateDirection, rotateSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }

    public void Stop()
    {
        agent.speed = 0;
    }

}
