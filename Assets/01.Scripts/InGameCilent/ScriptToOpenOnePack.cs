using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScriptToOpenOnePack : MonoBehaviour
{
    //��¦�̴� �̹���
    public Image GlowImage;

    //����
    public Color32 GlowColor;

    //��� �Ǵ��� Ȯ�� ����
    private bool _allowedToOpen = false;

    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public void AllowToOpenThisPack()
    {
        _allowedToOpen = true;
        ShopManager.instance.OpeningArea.AllowedToDragAPack = false;
        //���� ���� ���� ���� �ڷΰ��� ��ư�� ��Ȱ��ȭ �ؾ���.
        ShopManager.instance.OpeningArea.BackButton.interactable = false;

        if (CursorOverPack())
        {
            GlowImage.DOColor(GlowColor, 0.5f);
        }
    }
    private bool CursorOverPack()
    {
        RaycastHit[] hits;

        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);
        bool passedTableCollider = false;
        foreach(RaycastHit hit in hits)
        {
            if(hit.collider == _collider)
            {
                passedTableCollider = true;
            }
        }
        return passedTableCollider;
    }
    private void OnMouseEnter()
    {
        if (_allowedToOpen)
        {
            GlowImage.DOColor(GlowColor, 0.5f);
        }
    }
    private void OnMouseExit()
    {
        //����� ����Ʈ ����
        GlowImage.DOColor(Color.clear, 0.5f);
    }
    private void OnMouseDown()
    {
        if (_allowedToOpen)
        {
            //�ٽ� ���� �� �������ְ� ����
            _allowedToOpen = false;
            //��Ʈ�� ������ ����
            Sequence sequence = DOTween.Sequence();
            //���� ���� ��ġ�� �ű�ϴ�.
            sequence.Append(transform.DOLocalMoveZ(-2f, 0.5f));
            sequence.Append(transform.DOShakeRotation(1f,20f, 20));

            sequence.OnComplete(() =>
            {
                //���⿡ glow �� ��ƼŬ �ý����� ������ �˴ϴ�.

                //�� ���� üũ �� �����
                ShopManager.instance.OpeningArea.ShowPackOpening(transform.position);
                if (ShopManager.instance.PacksCreated > 0)
                {
                    ShopManager.instance.PacksCreated--;
                }
                //�� �������� ������ �� ����������
                Destroy(gameObject);
            });
        }
    }

}
