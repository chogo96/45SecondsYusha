using UnityEngine;

/// <summary>
/// �� �޸��� ��Ȯ�ؾ� �ϴ� ī�忡 �߰��ؾ��ϴ� ��ũ��Ʈ
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
        // ����ĳ��Ʈ�� ���� �ݶ��̴��� �´´ٸ� �ո�
        // ī�� �ݶ��̴��� ����ģ�ٸ� �޸�
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
            // �ٲ���ٸ�
            showingBack = passedThroughColliderOnCard;
            if (showingBack)
            {
                // �޸��� ������
                CardFront.gameObject.SetActive(false);
                CardBack.gameObject.SetActive(true);
            }
            else
            {
                //  ������ ������ 
                CardFront.gameObject.SetActive(true);
                CardBack.gameObject.SetActive(false);
            }

        }

    }
}
