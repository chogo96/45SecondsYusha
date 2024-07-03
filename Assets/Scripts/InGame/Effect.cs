using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    /// <summary>
    /// �� ���� �ִ� �Լ�
    /// </summary>
    /// <param name="amount">�� ����ġ</param>
    /// <param name="target">ī���� ���</param>
    public void Sword(float amount, AbilityTarget target)
    {
            
    }
    /// <summary>
    /// ���� ���� �ִ� �Լ�
    /// </summary>
    /// <param name="amount">���� ����ġ</param>
    /// <param name="target">ī���� ���</param>
    public void Magic(float amount, AbilityTarget target) 
    {
        
    }
    /// <summary>
    /// ���� ������ �ִ� �Լ�
    /// </summary>
    /// <param name="amount">���� ����ġ</param>
    /// <param name="target">ī���� ���</param>
    public void Shield(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// ��, ����, ���� �� �� ���� ����ġ�� �ִ� �Լ�
    /// </summary>
    /// <param name="amount">���� ����ġ</param>
    /// <param name="target">ī���� ���</param>
    public void Random(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// ������ ī�� �� ������ ī�带 amount�� ��ŭ ������ �ǵ���
    /// </summary>
    /// <param name="amount">�ǵ��� ��</param>
    /// <param name="target">ī���� ���</param>
    public void RandomRestore(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// ������ ī�� �� ī�带 �����ؼ� amount�� ��ŭ ������ �ǵ���
    /// </summary>
    /// <param name="amount">�ǵ��� ����</param>
    /// <param name="target">ī���� ���</param>
    public void SelectRestore(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// ������ ������� �����Ѵ�. (�� ������� ��� �����Ѵ�...)
    /// </summary>
    /// <param name="amount">������ ����</param>
    /// <param name="target">ī���� ���</param>
    public void RemoveRandomDebuff(float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// ����� ��� ������� �����Ѵ�.
    /// </summary>
    /// <param name="target">ī���� ���</param>
    public void RemoveAllDebuff(AbilityTarget target)
    {

    }
    /// <summary>
    /// ���� ���� ī�忡 ���ݷ� �߰� ������
    /// </summary>
    /// <param name="type">�߰� ���ݷ� ����</param>
    /// <param name="amount">�߰� ���ݷ� ��</param>
    /// <param name="target">ī���� ���</param>
    public void AddAttack(string type, float amount, AbilityTarget target)
    {

    }
    /// <summary>
    /// ���� ȿ���� ���� ������ �� ���� �Լ�
    /// </summary>
    /// <param name="amount">������ ���� ����</param>
    /// <param name="target">ī���� ���</param>
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