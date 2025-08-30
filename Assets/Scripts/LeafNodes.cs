//이 스크립트는 실제 게임 로직을 수행하는 리프 노드들(조건/행동)을 정의 합니다.
using UnityEngine;

public abstract class LeafNode : Node
{
    protected EnemyBT enemyBT;

    protected LeafNode() { } //기본 생성자
}

/* ===================== [조건 노드] ===================== */

//플레이어가 탐지(추적) 범위 안에 있는가?
public class IsPlayerInDetectRange : LeafNode
{
    public IsPlayerInDetectRange(EnemyBT agent)
    {
        enemyBT = agent;
    }

    public override NodeStatus Tick()
    {
        if (enemyBT.Player == null)
        {
            return NodeStatus.Failure;
        }

        float fDistance = Vector3.Distance(enemyBT.transform.position, enemyBT.Player.position);
        if (fDistance <= enemyBT.DetectRange)
        {
            Debug.Log("[추적 조건 노드] 성공 반환");
            return NodeStatus.Success;
        }
        return NodeStatus.Failure;
    }
}

//플레이어가 공격 범위 안에 있는가?
public class IsPlayerInAttackRange : LeafNode
{
    public IsPlayerInAttackRange(EnemyBT agent)
    {
        enemyBT = agent;
    }

    public override NodeStatus Tick()
    {
        if (enemyBT.Player == null)
        {
            return NodeStatus.Failure;
        }

        float fDistance = Vector3.Distance(enemyBT.transform.position, enemyBT.Player.position);
        if (fDistance <= enemyBT.AttackRange)
        {
            Debug.Log("[공격 조건 노드] 성공 반환");

            return NodeStatus.Success;
        }
        return NodeStatus.Failure;
    }
}

/* ===================== [행동 노드] ===================== */

//순찰: 웨이포인트를 순서대로 이동한다. (항상 Running, 웨이포인트 미설정 시 Failure)
public class PatrolNode : LeafNode
{
    private int nWaypointIndex = 0;

    public PatrolNode(EnemyBT agent)
    {
        enemyBT = agent;
    }

    public override NodeStatus Tick()
    {
        if (enemyBT.Waypoints == null || enemyBT.Waypoints.Length == 0)
        {
            //순찰할 웨이포인트가 없으면 실패로 간주
            return NodeStatus.Failure;
        }

        Transform current = enemyBT.Waypoints[nWaypointIndex];
        if (current == null)
        {
            return NodeStatus.Failure;
        }

        Debug.Log("[PatrolNode 활성화] 순찰중!");

        enemyBT.f_NavMesh(current.position);

        if(enemyBT.f_Arrived())
        {
            nWaypointIndex = (nWaypointIndex +1) % enemyBT.Waypoints.Length;
        }

        //현재 웨이포인트로 이동
        /*
        enemyBT.f_MoveToTarget(current.position);

        //충분히 가까워졌다면 다음 웨이포인트로 이동
        float fDistance = Vector3.Distance(enemyBT.transform.position, current.position);
        
        if (fDistance <= enemyBT.StopDistance)
        {
            nWaypointIndex = (nWaypointIndex + 1) % enemyBT.Waypoints.Length; //다음 위치로 이동(순환)
        }
        */

        //순찰은 지속 행위이므로 Running 반환
        return NodeStatus.Running;
    }
}

//추적: 플레이어를 향해 이동한다.
//공격 범위에 들어서면 Success를 반환하여 상위 트리에서 공격 분기로 전환될 수 있게 한다.
public class ChaseNode : LeafNode
{
    public ChaseNode(EnemyBT agent)
    {
        enemyBT = agent;
    }

    public override NodeStatus Tick()
    {
        if (enemyBT.Player == null)
        {
            return NodeStatus.Failure;
        }

        float fDistance = Vector3.Distance(enemyBT.transform.position, enemyBT.Player.position);

        Debug.Log("[ChaseNode 활성화] 추적중!");

        //공격 사거리면 추적 동작을 끝내고 Success (상위 Selector가 공격 시퀀스를 선택)
        if (fDistance <= enemyBT.AttackRange)
        {
            return NodeStatus.Success;
        }

        //아직 사거리 밖이면 계속 플레이어를 향해 이동
        
        //enemyBT.f_MoveToTarget(enemyBT.Player.position);
        
        enemyBT.f_NavMesh(enemyBT.Player.position);
        
        return NodeStatus.Running;
    }
}

//공격: 조건에 의해 사거리 안이라고 판단되었을 때 호출.
public class AttackNode : LeafNode
{
    public AttackNode(EnemyBT agent)
    {
        enemyBT = agent;
    }

    public override NodeStatus Tick()
    {
        if (enemyBT.Player == null)
        {
            return NodeStatus.Failure;
        }

        //예제에서는 단순히 로그만 출력 합니다.
        Debug.Log("[AttackNode 활성화] 공격중!");

        //한 프레임에 성공 처리. 다음 프레임에도 조건이 유지되면 공격이 반복 호출된다.
        return NodeStatus.Success;
    }
}
