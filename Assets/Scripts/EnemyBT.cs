/*
 * 이 스크립트는 Behavior Tree 기반의 적 AI를 구현합니다.
 * Start()에서 Behavior Tree를 구성
 * Update()에서 매 프레임 root.Tick() 호출
 * Tick() 호출 시점에 각 노드가 자신의 작업을 수행하고 상태를 반환
 * 
 * + NavMesh를 사용하여 이동하는 예제를 다룹니다. (NavMeshAgent 컴포넌트를 사용하여 경로 탐색 및 이동)
 */

using UnityEngine;
using UnityEngine.AI; //NavMeshAgent 사용

public class EnemyBT : MonoBehaviour
{
    [SerializeField] private Transform player;             //추적/공격 대상
    [SerializeField] private Transform[] waypoints;        //순찰 경로

    float fMoveSpeed = 3.0f;      //이동 속도
    float fStopDistance = 0.2f;   //웨이포인트 도착 판정 거리
    float fDetectRange = 6.0f;    //추적 시작 거리
    float fAttackRange = 2.0f;    //공격 사거리

    //외부 노드에서 접근할 수 있도록 읽기 전용 프로퍼티(getter)
    public Transform Player { get { return player; } }
    public Transform[] Waypoints { get { return waypoints; } }
    public float MoveSpeed { get { return fMoveSpeed; } }
    public float StopDistance { get { return fStopDistance; } }
    public float DetectRange { get { return fDetectRange; } }
    public float AttackRange { get { return fAttackRange; } }

    private Node root = null; //루트 노드
    private NavMeshAgent navMeshAgent = null; //NavMeshAgent 컴포넌트

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>(); //NavMeshAgent 컴포넌트 가져오기

        navMeshAgent.speed = fMoveSpeed;                //NavMeshAgent 속도 설정
        navMeshAgent.stoppingDistance = fStopDistance;  //NavMeshAgent 정지 거리 설정
    }

    void Start()
    {
        /* [트리 구성]
         * Root(Selector)
         *  ├─ Sequence( IsPlayerInAttackRange → Attack )
         *  ├─ Sequence( IsPlayerInDetectRange → Chase )
         *  └─ Patrol
         *  공격이 가능한 상황이면 가장 먼저 공격 시퀀스가 선택되고,
         *  그렇지 않으면 추적, 둘 다 아니라면 순찰을 선택한다.
         */

        //공격 시퀀스
        SequenceNode attackSequence = new SequenceNode();
        attackSequence.f_AddChild(new IsPlayerInAttackRange(this)); //조건 노드 추가
        attackSequence.f_AddChild(new AttackNode(this));            //행동 노드 추가

        //추적 시퀀스
        SequenceNode chaseSequence = new SequenceNode();
        chaseSequence.f_AddChild(new IsPlayerInDetectRange(this));  //조건 노드 추가
        chaseSequence.f_AddChild(new ChaseNode(this));              //행동 노드 추가

        //순찰 노드
        PatrolNode patrol = new PatrolNode(this); //행동 노드

        //셀렉터 노드(List에 추가하는 순서가 우선순위로 작용됨)
        SelectorNode rootSelector = new SelectorNode(); 
        rootSelector.f_AddChild(attackSequence);    //공격 시퀀스 추가
        rootSelector.f_AddChild(chaseSequence);     //추적 시퀀스 추가
        rootSelector.f_AddChild(patrol);            //순찰 노드 추가

        root = rootSelector; //루트 노드 설정
    }

    void Update()
    {
        if (root != null)
        {
            root.Tick(); //매 프레임마다 트리를 실행한다
        }
    }

    //목표 지점으로 수평 이동(예제 단순화 중력, NavMesh 미사용 이후 강의에서 확장)
    public void f_MoveToTarget(Vector3 targetPosition)
    {
        //y축(높이)은 현재 값 유지하여 수평만 이동
        Vector3 vTargetPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        //MoveTowards 메소드 : 어떤 위치에서 목표 위치로 정해진 속도로 이동하는 기능
        transform.position = Vector3.MoveTowards(transform.position, vTargetPos, fMoveSpeed * Time.deltaTime);
    }

    //목표 지점으로 이동(NavMesh 사용)
    public void f_NavMesh(Vector3 targetPosition)
    {
        if(!navMeshAgent.isOnNavMesh) { return; } //NavMesh 위에 있지 않으면 이동하지 않음

        navMeshAgent.stoppingDistance = fStopDistance;  //정지 거리
        navMeshAgent.SetDestination(targetPosition);    //목표 지점 설정
    }

    public bool f_Arrived()
    {
        if(navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            return false; //NavMeshAgent 컴포넌트가 없거나 NavMesh 위에 있지 않으면 도착하지 않음
        }

        if(navMeshAgent.pathPending)
        {
            return false; //경로 탐색이 진행 중이면 도착하지 않음
        }

        //목표 지점에 거의 도착했고 멈춰 있으면 도착한 것으로 간주
        bool isCloseEnough = navMeshAgent.remainingDistance <= Mathf.Max(navMeshAgent.stoppingDistance, 0.05f);
        bool isStopped = !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude <= 0.0001f;

        return isCloseEnough && isStopped;
    }

}
