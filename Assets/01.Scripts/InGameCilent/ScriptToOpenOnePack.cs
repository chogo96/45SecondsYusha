using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScriptToOpenOnePack : MonoBehaviour
{
    //반짝이는 이미지
    public Image GlowImage;

    //색상
    public Color32 GlowColor;

    //열어도 되는지 확인 여부
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
        //팩을 열고 있을 때는 뒤로가기 버튼이 비활성화 해야함.
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
        //적용된 이펙트 끄기
        GlowImage.DOColor(Color.clear, 0.5f);
    }
    private void OnMouseDown()
    {
        if (_allowedToOpen)
        {
            //다시 여는 걸 방지해주고 시작
            _allowedToOpen = false;
            //두트윈 시퀀스 시작
            Sequence sequence = DOTween.Sequence();
            //팩을 여는 위치에 옮깁니다.
            sequence.Append(transform.DOLocalMoveZ(-2f, 0.5f));
            sequence.Append(transform.DOShakeRotation(1f,20f, 20));

            sequence.OnComplete(() =>
            {
                //여기에 glow 와 파티클 시스템을 넣으면 됩니다.

                //팩 개수 체크 및 지우기
                ShopManager.instance.OpeningArea.ShowPackOpening(transform.position);
                if (ShopManager.instance.PacksCreated > 0)
                {
                    ShopManager.instance.PacksCreated--;
                }
                //이 시퀀스가 끝나면 팩 지워버리기
                Destroy(gameObject);
            });
        }
    }

}
