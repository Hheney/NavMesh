//Behavior Tree의 공통 기반 클래스와 실행 결과 상태를 정의하는 스크립트 입니다.

public enum NodeStatus
{
    Success, //행동 성공
    Failure, //행동 실패 (다음 대안 실행)
    Running  //행동 진행 중 (다음 프레임에 다시 Tick)
}

/*
 * [ 추상 클래스(abstract class)란? ]
 * - 클래스나, 메소드가 abstract로 선언되면 구체적인 내용이 없는 추상적인 상태로 존재하게 된다.
 * - (예시) 자동차는 달린다. (자동차가 어떻게 달리는지는 구체적으로 정의하지 않음)
 */

public abstract class Node
{
    //모든 노드는 매 프레임 Tick()을 호출받아 자신의 작업을 수행하고 상태를 반환한다.
    public abstract NodeStatus Tick();

}