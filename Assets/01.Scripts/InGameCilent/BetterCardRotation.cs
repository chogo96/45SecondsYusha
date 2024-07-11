using UnityEngine;

/// <summary>
/// 앞 뒷면이 정확해야 하는 카드에 추가해야하는 스크립트
/// </summary>

[ExecuteInEditMode]
public class BetterCardRotation : MonoBehaviour
{

    public RectTransform CardFront;

    public RectTransform CardBack;

    public Transform targetFacePoint;

    public Collider Collider;

    private bool showingBack = false;

    void Update()
    {
        // 레이캐스트로 쏴서 콜라이더에 맞는다면 앞면
        // 카드 콜라이더를 지나친다면 뒷면
        RaycastHit[] hits;
        hits = Physics.RaycastAll(origin: Camera.main.transform.position,
                                  direction: (-Camera.main.transform.position + targetFacePoint.position).normalized,
            maxDistance: (-Camera.main.transform.position + targetFacePoint.position).magnitude);
        bool passedThroughColliderOnCard = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == Collider)
                passedThroughColliderOnCard = true;
        }
        if (passedThroughColliderOnCard != showingBack)
        {
            // 바뀌었다면
            showingBack = passedThroughColliderOnCard;
            if (showingBack)
            {
                // 뒷면을 보여줌
                CardFront.gameObject.SetActive(false);
                CardBack.gameObject.SetActive(true);
            }
            else
            {
                //  앞쪽을 보여줌 
                CardFront.gameObject.SetActive(true);
                CardBack.gameObject.SetActive(false);
            }

        }

    }
}
