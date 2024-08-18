using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class DragSpellOnTarget : DraggingActions
{
    // Ÿ���� �ɼ� (��� ��, ��, �ڽ� ��)
    public TargetingOptions Targets = TargetingOptions.All;
    private SpriteRenderer sr; // ī���� ��������Ʈ ������
    private LineRenderer lr; // �巡�� �� ������ �� ������ (ȭ��ǥ)
    private WhereIsTheCardOrCreature whereIsThisCard; // ī���� ���� ��ġ ���¸� �����ϴ� ������Ʈ
    private VisualStates tempVisualState; // �巡�� ���� ���� �ð��� ���¸� �����ϱ� ���� ����
    private Transform triangle; // ȭ��ǥ �� �κ��� �ﰢ��
    private SpriteRenderer triangleSR; // �ﰢ���� ��������Ʈ ������
    private GameObject Target; // �巡���� ������ Ÿ���� �� ������Ʈ
    private OneCardManager manager; // ī�� ���� �Ŵ���

    private CurvedLinePoint[] linePoints = new CurvedLinePoint[0]; // � ������ ����Ʈ��
    private Vector3[] linePositions = new Vector3[0]; // ������ ��ġ��
    private Vector3[] linePositionsOld = new Vector3[0]; // ���� ������ ��ġ��

    // �巡�� �������� ���θ� �����ϴ� �Ӽ� (�׻� true�� ����)
    public override bool CanDrag
    {
        get
        {
            return true;
        }
    }

    void Awake()
    {
        // ������Ʈ���� �ʱ�ȭ
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponentInChildren<LineRenderer>();
        lr.sortingLayerName = "AboveEverything"; // ���� ��� �� ���� ǥ��
        triangle = transform.Find("Triangle");
        triangleSR = triangle.GetComponent<SpriteRenderer>();

        manager = GetComponentInParent<OneCardManager>();
        whereIsThisCard = GetComponentInParent<WhereIsTheCardOrCreature>();
    }

    // �巡�� ���� �� ȣ��Ǵ� �Լ�
    public override void OnStartDrag()
    {
        tempVisualState = whereIsThisCard.VisualState; // ���� ���� ����
        whereIsThisCard.VisualState = VisualStates.Dragging; // ���¸� �巡�� ������ ����

        sr.enabled = true; // ī�� ��������Ʈ�� Ȱ��ȭ
        lr.enabled = true; // �� �������� Ȱ��ȭ

        whereIsThisCard.SetHandSortingOrder(); // ī���� ���� ������ ����
    }

    // �巡�� ���� �� �� ������ ȣ��Ǵ� �Լ�
    public override void OnDraggingInUpdate()
    {
        Vector3 notNormalized = transform.position - transform.parent.position; // ī��� �θ� ������Ʈ ���� ���� ���
        Vector3 direction = notNormalized.normalized; // ���� ���� ���
        float distanceToTarget = (direction * 2.3f).magnitude; // Ÿ�ٱ����� �Ÿ� ���

        if (notNormalized.magnitude > distanceToTarget)
        {
            linePoints = lr.gameObject.GetComponentsInChildren<CurvedLinePoint>(); // � ������ ����Ʈ�� ��������

            // ��ġ ����
            Vector3 midPoint = Vector3.Lerp(transform.parent.position, transform.position - direction * 2.3f, 0.5f); // �߰� ���� ���
            linePositions = new Vector3[linePoints.Length]; // ���� ������ �迭 ����

            midPoint += new Vector3(0, 0, -lr.positionCount * 0.1f); // �߰� ������ Z �� ����
            if (midPoint.z > 0)
                midPoint.z = 0;
            if (midPoint.z < -10f)
                midPoint.z = -10f;

            if (lr.positionCount < 2)
                midPoint.z = 0;

            // ���� ����Ʈ���� ��ġ ����
            linePoints[0].transform.position = transform.parent.position;
            linePoints[1].transform.position = midPoint;
            linePoints[2].transform.position = transform.position - direction * 1.5f;

            linePositions[0] = linePoints[0].transform.position;
            linePositions[1] = linePoints[1].transform.position;
            linePositions[2] = linePoints[2].transform.position;

            // ���� ���� �����ǰ� ũ�Ⱑ ���� ������ ���� ����
            if (linePositionsOld.Length != linePositions.Length)
            {
                linePositionsOld = new Vector3[linePositions.Length];
            }

            // �ε巯�� ���� ���
            Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePositions, 2);

            // ���� ������ ����
            lr.positionCount = smoothedPoints.Length;
            lr.SetPositions(smoothedPoints);

            lr.enabled = true; // ���� ������ Ȱ��ȭ

            // ȭ��ǥ �� �κ��� �ﰢ�� ��ġ �� ȸ�� ����
            triangleSR.enabled = true;
            triangleSR.transform.position = transform.position - 1.35f * direction;

            float rot_z = Mathf.Atan2(notNormalized.y, notNormalized.x) * Mathf.Rad2Deg;
            triangleSR.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
        else
        {
            // Ÿ���� �ʹ� �����ٸ� ȭ��ǥ�� ��Ȱ��ȭ
            lr.enabled = false;
            triangleSR.enabled = false;
        }

    }

    // �巡�� ���� �� ȣ��Ǵ� �Լ�
    public override void OnEndDrag()
    {
        HandVisual PlayerHand = playerOwner.PArea.handVisual; // �÷��̾��� �� ���־� ����
        Target = null;
        RaycastHit[] hits;

        // ����ĳ��Ʈ ����, Ÿ���� �˻�
        hits = Physics.RaycastAll(origin: Camera.main.transform.position,
            direction: (-Camera.main.transform.position + this.transform.position).normalized,
            maxDistance: 30f);

        foreach (RaycastHit h in hits)
        {
            if (h.transform.tag.Contains("Player"))
            {
                // �÷��̾ ������
                Target = h.transform.gameObject;
            }
            else if (h.transform.tag.Contains("Creature"))
            {
                // ������ ������
                Target = h.transform.parent.gameObject;
            }
        }

        bool targetValid = false;

        if (Target != null)
        {
            // Ÿ���� �ɼǿ� ���� ������ ������� Ȯ��
            int targetID = Target.GetComponent<IDHolder>().UniqueID;
            switch (Targets)
            {
                case TargetingOptions.All:
                    targetValid = true;
                    playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                    break;
                case TargetingOptions.Enemy:
                    if (Target.tag.Contains("��"))
                    {
                        targetValid = true;
                        playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                    }
                    break;
                case TargetingOptions.Myself:
                    if (Target.tag.Contains("Creature") || Target.tag.Contains("Player"))
                    {
                        if ((tag.Contains("Low") && Target.tag.Contains("Top"))
                            || (tag.Contains("Top") && Target.tag.Contains("Low")))
                        {
                            playerOwner.PlayACardFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID);
                            targetValid = true;
                            playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                        }
                    }
                    break;
                case TargetingOptions.Other:
                    if (Target.tag.Contains("Creature"))
                    {
                        if ((tag.Contains("Low") && Target.tag.Contains("Top"))
                            || (tag.Contains("Top") && Target.tag.Contains("Low")))
                        {
                            playerOwner.PlayACardFromHand(GetComponentInParent<IDHolder>().UniqueID, targetID);
                            targetValid = true;
                            playerOwner.PArea.handVisual.PlayASpellFromHand(GetComponentInParent<IDHolder>().UniqueID);
                        }
                    }
                    break;
                default:
                    Utils.LogRed("DragSpellOnTarget���� �⺻ ���̽��� �����߽��ϴ�! �ǽɽ����� ����!!");
                    break;
            }
        }

        if (!targetValid)
        {
            // Ÿ���� ��ȿ���� �ʴٸ� ī�� ���¸� ����
            whereIsThisCard.VisualState = tempVisualState;
            whereIsThisCard.SetHandSortingOrder();

            // ī�带 ���� �������� �̵�
            PlayerHand.PlaceCardsOnNewSlots();
        }

        // ȭ��ǥ�� Ÿ���� ���� ��ġ�� ����
        transform.localPosition = new Vector3(0f, 0f, -1f);
        sr.enabled = false;
        lr.enabled = false;
        triangleSR.enabled = false;
    }

    // ������ �ʴ� �Լ�
    protected override bool DragSuccessful()
    {
        return true;
    }

    public override void OnCancelDrag()
    {

    }
}
