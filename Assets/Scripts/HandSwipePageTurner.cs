using UnityEngine;

public class HandSwipePageTurner : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public BookViewer bookViewer;

    public float swipeThreshold = 0.15f; // �ּ� �̵� �Ÿ�
    public float swipeCooldown = 0.7f;   // �ߺ� ���� ������ ��Ÿ��

    private Vector3 lastRightPos;
    private Vector3 lastLeftPos;
    private float lastSwipeTime = 0;

    void Update()
    {
        if (Time.time - lastSwipeTime < swipeCooldown) return;

        DetectSwipe(rightHand, ref lastRightPos, isRightHand: true);
        DetectSwipe(leftHand, ref lastLeftPos, isRightHand: false);
    }

    void DetectSwipe(Transform hand, ref Vector3 lastPos, bool isRightHand)
    {
        if (hand == null) return;

        Vector3 delta = hand.position - lastPos;

        if (isRightHand && delta.x < -swipeThreshold)
        {
            // ������
            Debug.Log("������ �������� �� ����: ���� ������");
            bookViewer.NextPage();
            lastSwipeTime = Time.time;
        }
        else if (!isRightHand && delta.x > swipeThreshold)
        {
            Debug.Log("�޼� �������� �� ������: ���� ������");
            bookViewer.PreviousPage();
            lastSwipeTime = Time.time;
        }

        lastPos = hand.position;
    }
}
