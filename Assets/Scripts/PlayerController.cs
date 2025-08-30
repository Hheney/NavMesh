//�� ��ũ��Ʈ�� Player(Capsule) ������Ʈ�� �����ϴ� ����� �����մϴ�.

using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    float fMoveSpeed = 5.0f;    //�÷��̾��� �̵� �ӵ�
    float fAxisX = 0.0f;        //������(X) �Է� ��
    float fAxisZ = 0.0f;        //������(Y) �Է� ��

    Vector3 vMoveDirection = Vector3.zero; //�÷��̾��� �̵� ������ �����ϴ� ����

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard InputKey = Keyboard.current;   //���� Ű���� �Է��� �����ɴϴ�.
        if (InputKey == null) return;           //Ű���� �Է��� ������ �Ʒ� �ڵ尡 ������� �ʵ��� �մϴ�.

        //Ű������ WASD �Ǵ� ȭ��ǥ Ű �Է��� ���� �÷��̾��� �̵� ������ �����մϴ�.
        fAxisX = (InputKey.aKey.isPressed || InputKey.leftArrowKey.isPressed ? -1.0f : 0.0f) +
                 (InputKey.dKey.isPressed || InputKey.rightArrowKey.isPressed ? 1.0f : 0.0f);

        fAxisZ = (InputKey.wKey.isPressed || InputKey.upArrowKey.isPressed ? 1.0f : 0.0f) +
                 (InputKey.sKey.isPressed || InputKey.downArrowKey.isPressed ? -1.0f : 0.0f);
        
        //�Էµ� ������ ����ȭ�Ͽ� ���� ���͸� ����ϴ�.(�밢�� �̵� �� �ӵ��� �������� ���� �����մϴ�.)
        vMoveDirection = new Vector3(fAxisX, 0.0f, fAxisZ).normalized;

        transform.Translate(vMoveDirection * fMoveSpeed * Time.deltaTime); //�÷��̾� ������Ʈ�� �̵���ŵ�ϴ�.
    }
}
