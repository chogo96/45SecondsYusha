//using UnityEngine;
//using System.Collections;
//using DG.Tweening;

//public class HoverPreview : MonoBehaviour
//{
//    public GameObject TurnThisOffWhenPreviewing;  //null ���̸� �ƹ��͵� �ȵ�����
//    public Vector3 TargetPosition;
//    public float TargetScale;
//    public GameObject previewGameObject;
//    public bool ActivateInAwake = false;

//    private static HoverPreview currentlyViewing = null;

//    /// <summary>
//    /// ������Ƽ��
//    /// </summary>
//    private static bool _PreviewsAllowed = true;
//    public static bool PreviewsAllowed
//    {
//        get { return _PreviewsAllowed; }

//        set
//        {
//            _PreviewsAllowed = value;
//            if (!_PreviewsAllowed)
//                StopAllPreviews();
//        }
//    }

//    private bool _thisPreviewEnabled = false;
//    public bool ThisPreviewEnabled
//    {
//        get { return _thisPreviewEnabled; }

//        set
//        {
//            _thisPreviewEnabled = value;
//            if (!_thisPreviewEnabled)
//                StopThisPreview();
//        }
//    }

//    public bool OverCollider { get; set; }

//    void Awake()
//    {
//        ThisPreviewEnabled = ActivateInAwake;
//    }

//    void OnMouseEnter()
//    {
//        OverCollider = true;
//        if (PreviewsAllowed && ThisPreviewEnabled)
//            PreviewThisObject();
//    }

//    void OnMouseExit()
//    {
//        OverCollider = false;

//        if (!PreviewingSomeCard())
//            StopAllPreviews();
//    }

//    void PreviewThisObject()
//    {
//        // 1) �� ī�带 ������
//        // �̹� �����䰡 ������ �� ī�带 ������
//        StopAllPreviews();
//        // 2) �� �����並 ���� ������� ����
//        currentlyViewing = this;
//        // 3) ���� ������Ʈ�� ������ Ȱ��ȭ
//        previewGameObject.SetActive(true);
//        // 4) ��Ȱ���� �Ұ� ������ ��Ȱ��ȭ ��Ŵ
//        if (TurnThisOffWhenPreviewing != null)
//            TurnThisOffWhenPreviewing.SetActive(false);
//        // 5) �ִϸ��̼� �߰�
//        previewGameObject.transform.localPosition = Vector3.zero;
//        previewGameObject.transform.localScale = Vector3.one;

//        previewGameObject.transform.DOLocalMove(TargetPosition, 1f).SetEase(Ease.OutQuint);
//        previewGameObject.transform.DOScale(TargetScale, 1f).SetEase(Ease.OutQuint);
//    }

//    void StopThisPreview()
//    {
//        previewGameObject.SetActive(false);
//        previewGameObject.transform.localScale = Vector3.one;
//        previewGameObject.transform.localPosition = Vector3.zero;
//        if (TurnThisOffWhenPreviewing != null)
//            TurnThisOffWhenPreviewing.SetActive(true);
//    }

//    private static void StopAllPreviews()
//    {
//        if (currentlyViewing != null)
//        {
//            currentlyViewing.previewGameObject.SetActive(false);
//            currentlyViewing.previewGameObject.transform.localScale = Vector3.one;
//            currentlyViewing.previewGameObject.transform.localPosition = Vector3.zero;
//            if (currentlyViewing.TurnThisOffWhenPreviewing != null)
//                currentlyViewing.TurnThisOffWhenPreviewing.SetActive(true);
//        }

//    }

//    private static bool PreviewingSomeCard()
//    {
//        if (!PreviewsAllowed)
//            return false;

//        HoverPreview[] allHoverBlowups = GameObject.FindObjectsOfType<HoverPreview>();

//        foreach (HoverPreview hoverPreview in allHoverBlowups)
//        {
//            if (hoverPreview.OverCollider && hoverPreview.ThisPreviewEnabled)
//                return true;
//        }

//        return false;
//    }


//}
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HoverPreview : MonoBehaviour
{
    public GameObject TurnThisOffWhenPreviewing;  // null ���̸� �ƹ��͵� �� ������
    public Vector3 TargetPosition;
    public float TargetScale;
    public GameObject previewGameObject;
    public bool ActivateInAwake = false;

    private static HoverPreview currentlyViewing = null;

    /// <summary>
    /// ������Ƽ��
    /// </summary>
    private static bool _PreviewsAllowed = true;
    public static bool PreviewsAllowed
    {
        get { return _PreviewsAllowed; }
        set
        {
            _PreviewsAllowed = value;
            if (!_PreviewsAllowed)
                StopAllPreviews();
        }
    }

    private bool _thisPreviewEnabled = false;
    public bool ThisPreviewEnabled
    {
        get { return _thisPreviewEnabled; }
        set
        {
            _thisPreviewEnabled = value;
            if (!_thisPreviewEnabled)
                StopThisPreview();
        }
    }

    public bool OverCollider { get; set; }

    void Awake()
    {
        ThisPreviewEnabled = ActivateInAwake;
    }

    void OnMouseEnter()
    {
        OverCollider = true;
        if (PreviewsAllowed && ThisPreviewEnabled)
            PreviewThisObject();
    }

    void OnMouseExit()
    {
        OverCollider = false;

        if (!PreviewingSomeCard())
            StopAllPreviews();
    }

    void PreviewThisObject()
    {
        // 1) �� ī�带 ������
        // �̹� �����䰡 ������ �� ī�带 ������
        StopAllPreviews();
        // 2) �� �����並 ���� ������� ����
        currentlyViewing = this;
        // 3) ���� ������Ʈ�� ������ Ȱ��ȭ
        if (previewGameObject == null)
        {
            Debug.LogError("Preview GameObject is not assigned!");
            return;
        }

        // ī�� ������ previewGameObject�� OneCardManager�� �Ҵ�
        OneCardManager previewManager = previewGameObject.GetComponent<OneCardManager>();
        if (previewManager == null)
        {
            Debug.LogError("OneCardManager is not attached to the previewGameObject!");
            return;
        }

        OneCardManager originalManager = GetComponent<OneCardManager>();
        if (originalManager == null)
        {
            Debug.LogError("OneCardManager is not attached to the current GameObject!");
            return;
        }

        previewManager.cardAsset = originalManager.cardAsset;
        previewManager.ReadCardFromAsset();

        previewGameObject.SetActive(true);
        // 4) ��Ȱ��ȭ�� �� ������ ��Ȱ��ȭ ��Ŵ
        if (TurnThisOffWhenPreviewing != null)
            TurnThisOffWhenPreviewing.SetActive(false);
        // 5) �ִϸ��̼� �߰�
        previewGameObject.transform.localPosition = Vector3.zero;
        previewGameObject.transform.localScale = Vector3.one;

        previewGameObject.transform.DOLocalMove(TargetPosition, 1f).SetEase(Ease.OutQuint);
        previewGameObject.transform.DOScale(TargetScale, 1f).SetEase(Ease.OutQuint);
    }

    void StopThisPreview()
    {
        if (previewGameObject != null)
        {
            previewGameObject.SetActive(false);
            previewGameObject.transform.localScale = Vector3.one;
            previewGameObject.transform.localPosition = Vector3.zero;
        }
        if (TurnThisOffWhenPreviewing != null)
            TurnThisOffWhenPreviewing.SetActive(true);
    }

    private static void StopAllPreviews()
    {
        if (currentlyViewing != null)
        {
            currentlyViewing.previewGameObject.SetActive(false);
            currentlyViewing.previewGameObject.transform.localScale = Vector3.one;
            currentlyViewing.previewGameObject.transform.localPosition = Vector3.zero;
            if (currentlyViewing.TurnThisOffWhenPreviewing != null)
                currentlyViewing.TurnThisOffWhenPreviewing.SetActive(true);
        }
    }

    private static bool PreviewingSomeCard()
    {
        if (!PreviewsAllowed)
            return false;

        HoverPreview[] allHoverBlowups = GameObject.FindObjectsOfType<HoverPreview>();

        foreach (HoverPreview hoverPreview in allHoverBlowups)
        {
            if (hoverPreview.OverCollider && hoverPreview.ThisPreviewEnabled)
                return true;
        }

        return false;
    }
}
