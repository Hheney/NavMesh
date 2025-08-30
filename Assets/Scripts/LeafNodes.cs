//�� ��ũ��Ʈ�� ���� ���� ������ �����ϴ� ���� ����(����/�ൿ)�� ���� �մϴ�.
using UnityEngine;

public abstract class LeafNode : Node
{
    protected EnemyBT enemyBT;

    protected LeafNode() { } //�⺻ ������
}

/* ===================== [���� ���] ===================== */

//�÷��̾ Ž��(����) ���� �ȿ� �ִ°�?
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
            Debug.Log("[���� ���� ���] ���� ��ȯ");
            return NodeStatus.Success;
        }
        return NodeStatus.Failure;
    }
}

//�÷��̾ ���� ���� �ȿ� �ִ°�?
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
            Debug.Log("[���� ���� ���] ���� ��ȯ");

            return NodeStatus.Success;
        }
        return NodeStatus.Failure;
    }
}

/* ===================== [�ൿ ���] ===================== */

//����: ��������Ʈ�� ������� �̵��Ѵ�. (�׻� Running, ��������Ʈ �̼��� �� Failure)
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
            //������ ��������Ʈ�� ������ ���з� ����
            return NodeStatus.Failure;
        }

        Transform current = enemyBT.Waypoints[nWaypointIndex];
        if (current == null)
        {
            return NodeStatus.Failure;
        }

        Debug.Log("[PatrolNode Ȱ��ȭ] ������!");

        enemyBT.f_NavMesh(current.position);

        if(enemyBT.f_Arrived())
        {
            nWaypointIndex = (nWaypointIndex +1) % enemyBT.Waypoints.Length;
        }

        //���� ��������Ʈ�� �̵�
        /*
        enemyBT.f_MoveToTarget(current.position);

        //����� ��������ٸ� ���� ��������Ʈ�� �̵�
        float fDistance = Vector3.Distance(enemyBT.transform.position, current.position);
        
        if (fDistance <= enemyBT.StopDistance)
        {
            nWaypointIndex = (nWaypointIndex + 1) % enemyBT.Waypoints.Length; //���� ��ġ�� �̵�(��ȯ)
        }
        */

        //������ ���� �����̹Ƿ� Running ��ȯ
        return NodeStatus.Running;
    }
}

//����: �÷��̾ ���� �̵��Ѵ�.
//���� ������ ���� Success�� ��ȯ�Ͽ� ���� Ʈ������ ���� �б�� ��ȯ�� �� �ְ� �Ѵ�.
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

        Debug.Log("[ChaseNode Ȱ��ȭ] ������!");

        //���� ��Ÿ��� ���� ������ ������ Success (���� Selector�� ���� �������� ����)
        if (fDistance <= enemyBT.AttackRange)
        {
            return NodeStatus.Success;
        }

        //���� ��Ÿ� ���̸� ��� �÷��̾ ���� �̵�
        
        //enemyBT.f_MoveToTarget(enemyBT.Player.position);
        
        enemyBT.f_NavMesh(enemyBT.Player.position);
        
        return NodeStatus.Running;
    }
}

//����: ���ǿ� ���� ��Ÿ� ���̶�� �ǴܵǾ��� �� ȣ��.
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

        //���������� �ܼ��� �α׸� ��� �մϴ�.
        Debug.Log("[AttackNode Ȱ��ȭ] ������!");

        //�� �����ӿ� ���� ó��. ���� �����ӿ��� ������ �����Ǹ� ������ �ݺ� ȣ��ȴ�.
        return NodeStatus.Success;
    }
}
