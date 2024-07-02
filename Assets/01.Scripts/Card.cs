using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    //사용방법 
    //프로젝트 뷰에서 우클릭하고 Create > Card 선택하면  Scriptable Object생성함
    //카드 명
    public string cardName;
    //카드 설명
    public string description;
    //요구치 1번 공격치
    public int swordAttack;
    //요구치 2번 공격치
    public int magicAttack;
    //요구치 3번 공격치
    public int shieldAttack;
    //카드의 희귀도
    public string rarity;
    //카드의 타입
    public string type;
}

