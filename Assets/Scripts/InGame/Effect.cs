using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    /// <summary>
    /// 검 공격 주는 함수
    /// </summary>
    /// <param name="amount">검 공격치</param>
    /// <param name="target">카드의 대상</param>
    public void Sword(float amount, AbilityTarget target)
    {
            
    }
    /// <summary>
    /// 마법 공격 주는 함수
    /// </summary>
    /// <param name="amount">마법 공격치</param>
    /// <param name="target">카드의 대상</param>
    public void Magic(float amount, AbilityTarget target) 
    {
        
    }
    /// <summary>
    /// 방패 공격을 주는 함수
    /// </summary>
    /// <param name="amount">방패 공격치</param>
    /// <param name="target">카드의 대상</param>
    public void Shield(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// 검, 방패, 마법 중 한 개의 공격치를 주는 함수
    /// </summary>
    /// <param name="amount">랜덤 공격치</param>
    /// <param name="target">카드의 대상</param>
    public void Random(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// 버려진 카드 중 랜덤한 카드를 amount양 만큼 덱으로 되돌림
    /// </summary>
    /// <param name="amount">되돌릴 양</param>
    /// <param name="target">카드의 대상</param>
    public void RandomRestore(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// 버려진 카드 중 카드를 선택해서 amount양 만큼 덱으로 되돌림
    /// </summary>
    /// <param name="amount">되돌릴 갯수</param>
    /// <param name="target">카드의 대상</param>
    public void SelectRestore(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// 랜덤한 디버프를 제거한다. (그 디버프가 없어도 제거한다...)
    /// </summary>
    /// <param name="amount">제거할 개수</param>
    /// <param name="target">카드의 대상</param>
    public void RemoveRandomDebuff(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// 대상의 모든 디버프를 제거한다.
    /// </summary>
    /// <param name="target">카드의 대상</param>
    public void RemoveAllDebuff(AbilityTarget target)
    {

    }
    /// <summary>
    /// 다음 공격 카드에 공격력 추가 데미지
    /// </summary>
    /// <param name="type">추가 공격력 종류</param>
    /// <param name="amount">추가 공격력 양</param>
    /// <param name="target">카드의 대상</param>
    public void AddAttack(string type, float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// 적의 효과로 인해 버려질 때 막는 함수
    /// </summary>
    /// <param name="amount">적용할 버프 갯수</param>
    /// <param name="target">카드의 대상</param>
    public void ResistHandDeath (float amount, AbilityTarget target) 
    {
        
    }
}
public enum AbilityTarget
{
    None = 0,
    Self = 4,
    //except Self
    OneAllyPlayer = 5,
    Opponent = 6,
    AllAllyPlayers = 7,
}