/*
 * 이 스크립트는 행동 트리(Behavior Tree)에서 사용되는 합성 노드(Composite Node)를 정의합니다.
 * 상속 구조 : Node -> CompositeNode -> (SequenceNode, SelectorNode)
 */

using System.Collections.Generic;

//CompositeNode는 Node를 상속받아 구현된 추상 클래스
public abstract class CompositeNode : Node
{
    protected readonly List<Node> children = new List<Node>(); //자식 노드 리스트

    public void f_AddChild(Node child) //자식 노드를 추가하는 메소드
    {
        if (child != null)
        {
            children.Add(child);
        }
    }
}

//SequenceNode는 CompositeNode를 상속받아 구현된 클래스
public class SequenceNode : CompositeNode
{
    public override NodeStatus Tick()
    {
        for (int i = 0; i < children.Count; i++)
        {
            NodeStatus status = children[i].Tick();
            if (status == NodeStatus.Failure) 
            { 
                return NodeStatus.Failure; 
            }
            
            if (status == NodeStatus.Running) 
            { 
                return NodeStatus.Running; 
            }
        }
        return NodeStatus.Success;
    }
}

//SelectorNode는 CompositeNode를 상속받아 구현된 클래스
public class SelectorNode : CompositeNode
{
    public override NodeStatus Tick()
    {
        for (int i = 0; i < children.Count; i++)
        {
            NodeStatus status = children[i].Tick();
            if (status == NodeStatus.Success) 
            { 
                return NodeStatus.Success; 
            }
            
            if (status == NodeStatus.Running) 
            { 
                return NodeStatus.Running; 
            }
        }
        return NodeStatus.Failure;
    }
}
