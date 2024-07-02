using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    //����� 
    //������Ʈ �信�� ��Ŭ���ϰ� Create > Card �����ϸ�  Scriptable Object������
    //ī�� ��
    public string cardName;
    //ī�� ����
    public string description;
    //�䱸ġ 1�� ����ġ
    public int swordAttack;
    //�䱸ġ 2�� ����ġ
    public int magicAttack;
    //�䱸ġ 3�� ����ġ
    public int shieldAttack;
    //ī���� ��͵�
    public string rarity;
    //ī���� Ÿ��
    public string type;
}

