using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class SimpleStateMachine : MonoBehaviour
{
    enum State { GoHome, Observe, Chase, SoundInvestigate }

    [Header("Scene References")]
    public Transform character;
   
    public TextMeshProUGUI stateText;
    public SimpleStateMachine[] npcs;
    public SoundTrigger soundAlert;
    public SoundRadius alertRadius;
    private Vector3 homePosition;

    [Header("Config")]
    public float waypointThreshold = 0.6f;
    public float rotationSpeed = 1f;
    public float playerDistThreshold = 2.0f;
    public float normalSpeed = 3.5f;
    public float chaseSpeed = 5.0f;

    public float soundCheckThreshold = 2f;
    public float soundCheckDistance = 1.5f;

    public float observeRotation = 0.1f;

    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 60f;


    State state;
    NavMeshAgent agent;
   

    public Vector3 soundTarget;

    
    
    bool canSeePlayer;

    
    public bool soundHeard;
    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        soundHeard = false;
    }

    void Start()
    {
        homePosition = transform.position;
        
        state = State.Observe;
    }

    void Update()
    {
        switch (state)
        {
            case State.GoHome:
                GoHome();
                break;
            case State.Observe:
                Observe();
                break;
            case State.Chase:
                Chase();
                break;
            case State.SoundInvestigate:
                SoundInvestigate();
                break;

        }

        /*
        // regardless of state, NPC always looks in the direction they are moving
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookDirection = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, Time.deltaTime * rotationSpeed);
        }
        */

        // show state on screen
        stateText.text = $"State: {state}";

        // if NPC ever gets close to player, end 
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;
        if (distToPlayer < playerDistThreshold && canSeePlayer) SceneManager.LoadScene("END");
    }

    
    void GoHome()
    {
        agent.SetDestination(homePosition);
        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {


            Debug.Log(state);
            state = State.Chase;
        }

        float distance = Vector3.Distance(homePosition, transform.position);
        if (distance < playerDistThreshold)
        {
            state = State.Observe;
        }

    }

    void Observe()
    {
        transform.Rotate(Vector3.up*observeRotation);

        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {
            
            
            Debug.Log(state);
            state = State.Chase;
        }

        if (soundHeard)
        {
            state = State.SoundInvestigate;
        }

    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(character.position);

        canSeePlayer = IsInViewCone();
        if (!canSeePlayer)
        {
            state = State.GoHome;
        }
    }

    

    void SoundInvestigate()
    {
        Debug.Log(state);
        
        agent.SetDestination(alertRadius.transform.position);

        float distance = Vector3.Distance(transform.position, alertRadius.transform.position);
        Debug.Log(distance);
        if (distance <= soundCheckDistance)
        {
            
            
            
                
                state = State.GoHome;
                Debug.Log(state);
            
           
        }

        

        canSeePlayer = IsInViewCone();
        if (canSeePlayer)
        {
           
            state = State.Chase;
            Debug.Log(state);
        }
    }



    // --- HELPER FUNCTIONS ---

    bool IsInViewCone()
    {
        Vector3 toPlayer = character.position - transform.position;
        float distToPlayer = toPlayer.magnitude;

        // 1. Distance check
        if (distToPlayer > viewRadius) return false;

        // 2. Angle check
        Vector3 dirToPlayer = toPlayer.normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle > viewAngle * 0.5f) return false;

        // 3. Raycast
        if (Physics.Raycast(transform.position, dirToPlayer, out RaycastHit hit, viewRadius))
        {
            return hit.transform == character.transform;
        }
        return false;
    }

  

    // --- GIZMO DRAWING FOR DEBUG ---

    private void OnDrawGizmos()
    {

        // draw the view cone (2D version)
        
            Handles.color = new Color(0f, 1f, 1f, 0.25f);
            if (canSeePlayer) Handles.color = new Color(1f, 0f, 0f, 0.25f);

            Vector3 forward = transform.forward;
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, viewAngle / 2f, viewRadius);
            Handles.DrawSolidArc(transform.position, Vector3.up, forward, -viewAngle / 2f, viewRadius);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SoundEvent"))
        {
            soundHeard = true;
            Debug.Log("soundHeard");
            Invoke("NotHeard", 5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
      
    }

    void NotHeard()
    {
        soundHeard = false;
    }

}
