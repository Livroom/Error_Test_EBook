using UnityEngine;

public class HandSwipePageTurner : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public BookViewer bookViewer;

    public float swipeThreshold = 0.15f; // 최소 이동 거리
    public float swipeCooldown = 0.7f;   // 중복 감지 방지용 쿨타임

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
            // 오른쪽
            Debug.Log("오른손 스와이프 → 왼쪽: 다음 페이지");
            bookViewer.NextPage();
            lastSwipeTime = Time.time;
        }
        else if (!isRightHand && delta.x > swipeThreshold)
        {
            Debug.Log("왼손 스와이프 → 오른쪽: 이전 페이지");
            bookViewer.PreviousPage();
            lastSwipeTime = Time.time;
        }

        lastPos = hand.position;
    }
}
