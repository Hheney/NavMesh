/*
 * �� ��ũ��Ʈ�� �ൿ Ʈ��(Behavior Tree)���� ���Ǵ� �ռ� ���(Composite Node)�� �����մϴ�.
 * ��� ���� : Node -> CompositeNode -> (SequenceNode, SelectorNode)
 */

using System.Collections.Generic;

//CompositeNode�� Node�� ��ӹ޾� ������ �߻� Ŭ����
public abstract class CompositeNode : Node
{
    protected readonly List<Node> children = new List<Node>(); //�ڽ� ��� ����Ʈ

    public void f_AddChild(Node child) //�ڽ� ��带 �߰��ϴ� �޼ҵ�
    {
        if (child != null)
        {
            children.Add(child);
        }
    }
}

//SequenceNode�� CompositeNode�� ��ӹ޾� ������ Ŭ����
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

//SelectorNode�� CompositeNode�� ��ӹ޾� ������ Ŭ����
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
