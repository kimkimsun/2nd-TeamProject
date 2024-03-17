using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Soldier : MonoBehaviour
{

    private SoldierStateMachine sm;
    public NavMeshAgent agent;
    public Animator animator;
    public float agentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        sm = new SoldierStateMachine(this);

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        sm.AddStateDic(CustomInterface.STATE_TYPE.IDLE, new IdleState());
        sm.AddStateDic(CustomInterface.STATE_TYPE.MOVE, new MoveState());
        sm.SetState(CustomInterface.STATE_TYPE.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        agentSpeed = Mathf.Sqrt(agent.velocity.x * agent.velocity.x + agent.velocity.z * agent.velocity.z);
        sm.Update();
    }
    public void MoveAuto(Vector3 pos)//�౺���� ȣ���ϴ� �Լ�
    {
        agent.enabled = true;
        agent.SetDestination(pos);
    }
}
