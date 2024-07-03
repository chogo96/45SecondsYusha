using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardModel : MonoBehaviour
{
    Image _image;

    /// <summary>
    /// ī�� �ո� ������ ����
    /// </summary>
    public Sprite[] faces;
    /// <summary>
    /// ī�� �޸� ������ ����
    /// </summary>
    public Sprite[] cardBack;

    public int faceCardIndex; //���� �迭 ��� ���� Ȱ��
    public int backCardIndex; //����
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
