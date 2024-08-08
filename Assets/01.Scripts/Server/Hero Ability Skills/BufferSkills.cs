using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BufferSkills : MonoBehaviourPunCallbacks, SkillsInterface
{
    SkillsManager skillsManager;
    UI_Timer uI_Timer;

    private void Awake()
    {
        skillsManager = FindObjectOfType<SkillsManager>();
        uI_Timer = FindObjectOfType<UI_Timer>();
    }
    private void Start()
    {
        SetSkill();
    }
    public void SetSkill()
    {
        skillsManager.SetSkill(this);
    }

    // ���� - ȸ�߽ð� > ���� �ɷ��� ������ �ð� 45�ʷ� ȸ��
    public void UseSkill()
    {
        // photonView.RPC("UseSkillSignal", RpcTarget.All);
        uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 45f); //�ɷ� ���� �Ϸ�
        // �ɷ±��� + ��ȭ�鿡 ������ �ִϸ��̼� or ����Ʈ ����
    }

    [PunRPC]
    public void UseSkillSignal()
    {
        // �ٸ����� ���忡�� �ñر� ����ߴٴ°� ���ϸ��� �ִϸ��̼� or ����Ʈ ���⿡ ����
    }
}
