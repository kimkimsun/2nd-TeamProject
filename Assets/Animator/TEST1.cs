using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TEST1 : MonoBehaviour
{
    public Queue<Vector3> playerPos = new Queue<Vector3>();
    public Animator animator;
    public NavMeshAgent agent;
    public TEST2 test2;

    public CinemachineVirtualCamera loopCam;
    public CinemachineVirtualCamera goalCam;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        test2.SetPathes(playerPos);
        agent.SetDestination(playerPos.Dequeue());
        
    }
    private float agentSpeed;
    // Update is called once per frame
    void Update()
    {
        
        animator.SetFloat("speed", 1f);
        if(agent.remainingDistance <= 0.5f)
        {
            agent.ResetPath();
            agent.SetDestination(playerPos.Dequeue());
            if (playerPos.Count == 1)
            {
                goalCam.Priority = 11;
            }
            if(playerPos.Count == 0)
            {
                goalCam.Priority = 9;
                test2.SetPathes(playerPos);
            }
        }
    }
}
