using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Used for an EnemyAI - Has four states - Patroling - Following - Shooting - Fleeing

public class EnemyAIController : MonoBehaviour
{
    [Header("Radius")]
    [SerializeField] float LookRadius = 10;
    [SerializeField] float ShootRadius;
    [SerializeField] float AgentPathDelayTime;


    [Header("GameObjects")]
    [SerializeField] GameObject AIWeapon;

    [Header("Floats")]

    [SerializeField] float MinHealth;
    [SerializeField] float ObstacleIterationNumber;
    [SerializeField] float TimeBetweenObstaclePathing;
    [SerializeField] float MinVelocity;


    private Transform Target;
    public NavMeshAgent Agent {get; set;}

    public bool Moving { get; set; }

    private float AgentPathDelayCounter;
    private float TimeBetweenObstaclePathingCounter;
    private bool MovingToObstacle;
    private void Start()
    {
        TimeBetweenObstaclePathingCounter = TimeBetweenObstaclePathing;
        Agent = GetComponent<NavMeshAgent>();
        //Singleton refrence to limit refrene issues
        Target = PlayerManager.instance.Player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        VelocityCheck();

        MoveToObstacle();
        if (!MovingToObstacle)
        {
            if (MoveToPlayer())
            {
                UpdateAIMovement();
            }
        }
    }

    private void VelocityCheck()
    {
        //Add time to AgentPathDelayCounter
        AgentPathDelayCounter +=  Time.deltaTime;
        TimeBetweenObstaclePathingCounter += Time.deltaTime;

        //Check if the Agent velocity is greater than the minimum velocity
        if (Agent.velocity.magnitude > MinVelocity)
        {
            //If it is greater than velocity the reset the counter
            AgentPathDelayCounter = 0;
            Moving = true;
        }
        else
        {
            Moving = false;
        }
    }

    private void MoveToObstacle()
    {
        float distance = Vector3.Distance(Target.position, transform.position);

        if (distance < LookRadius && gameObject.GetComponent<ObjectHealth>().health < MinHealth && TimeBetweenObstaclePathingCounter > TimeBetweenObstaclePathing)
        {
            for (int i = 0; i < ObstacleIterationNumber; i++) {
                //Gets a direction and multiplies it by a scalar
                Vector2 randomDirection = Random.insideUnitCircle * ObstacleIterationNumber;
                //Sets the Vector2 into a Vector3 with the Y axis being refrenced from the gameObject
                Vector3 randomPosition = new Vector3(randomDirection.x + Agent.transform.position.x, Agent.transform.position.y, randomDirection.y + Agent.transform.position.z);

                RaycastHit rayHit;

                Physics.Linecast(randomPosition, Target.position, out rayHit);

                if (rayHit.collider.tag == "Ground")
                {
                    TimeBetweenObstaclePathingCounter = 0;
                    //Creates a varable to store the hit value
                    NavMeshHit navHit;
                    //NavMesh.SamplePosition takes a transform and finds the closes transform on the NavMeshSurface
                    NavMesh.SamplePosition(randomPosition, out navHit, LookRadius, 1);
                    MovingToObstacle = true;
                    Agent.SetDestination(navHit.position);
                }
            }
        }
        else if(Agent.remainingDistance < Agent.stoppingDistance && MovingToObstacle)
        {
            MovingToObstacle = false;
        }
    }

    private bool MoveToPlayer()
    {
        //Gets the distance between the Agent and the Target
        float distance = Vector3.Distance(Target.position, transform.position);

        //Checks if the Target is within LookRadius
        if (distance <= LookRadius)
        {
            AIWeapon.transform.LookAt(Target.position);

             //Draw line
            Debug.DrawRay(Target.transform.position,
            Vector3.up,
            Color.red,
            1f
            );
            
            NavMeshHit navHit;
            //NavMesh.SamplePosition takes a transform and finds the closes transform on the NavMeshSurface
            NavMesh.SamplePosition(Target.transform.position, out navHit, LookRadius, 1);
            //Sets the Destination to Target
            Agent.SetDestination(navHit.position);

            if (distance < ShootRadius)
            {
                //Attack target
                //Sets Shoot's hasShot to true (Note: This acts as a Mouse0 for the bots)
                gameObject.GetComponentInChildren<Shoot>().hasShot = true;
                //Face target even though it is not pathing
                FaceTarget();
            }
            else
            {
                //Sets Shoot's hasShot to false (Note: This acts as a Mouse0 for the bots)
                gameObject.GetComponentInChildren<Shoot>().hasShot = false;
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    void UpdateAIMovement()
    {
        //If the Agent's velocity is under 0.25f and the NavMeshDelayCounter is higher than NavMeshDelay it will path a new transform
        if (Agent.remainingDistance <= Agent.stoppingDistance && AgentPathDelayCounter >= AgentPathDelayTime)
        {
            //Gets a direction and multiplies it by a scalar
            Vector2 randomDirection = Random.insideUnitCircle * LookRadius;
            //Sets the Vector2 into a Vector3 with the Y axis being refrenced from the gameObject
            Vector3 randomPosition = new Vector3(randomDirection.x + Agent.transform.position.x, Agent.transform.position.y, randomDirection.y + Agent.transform.position.z);
            //Creates a varable to store the hit value
            NavMeshHit navHit;
            //NavMesh.SamplePosition takes a transform and finds the closes transform on the NavMeshSurface
            NavMesh.SamplePosition(randomPosition, out navHit, LookRadius, 1);

            //Draw line
            Debug.DrawRay(navHit.position,
            Vector3.up,
            Color.red,
            1f
            );
            
            //Sets the destination to the NavMesh.SamplePosition output
            Agent.SetDestination(navHit.position);
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (Target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = lookRotation;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, LookRadius);
        Gizmos.DrawWireSphere(transform.position, Agent.stoppingDistance);
        Gizmos.DrawWireSphere(transform.position, ShootRadius);
    }
}
