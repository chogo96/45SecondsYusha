using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardModel : MonoBehaviour
{
    Image _image;

    /// <summary>
    /// 카드 앞면 변수로 지정
    /// </summary>
    public Sprite[] faces;
    /// <summary>
    /// 카드 뒷면 변수로 지정
    /// </summary>
    public Sprite[] cardBack;

    public int faceCardIndex; //위의 배열 사용 변수 활용
    public int backCardIndex; //동일
    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void ToggleFace(bool showFace)
    {
        if (showFace)
        {
            _image.sprite = faces[faceCardIndex];
        }
        else
        {
            _image.sprite = cardBack[backCardIndex];
        }
    }
}
