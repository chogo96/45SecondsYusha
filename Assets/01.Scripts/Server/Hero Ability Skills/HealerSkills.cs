using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealerSkills : MonoBehaviourPunCallbacks, SkillsInterface
{
    SkillsManager skillsManager;

    private void Awake()
    {
        skillsManager = FindObjectOfType<SkillsManager>();
    }
    private void Start()
    {
        SetSkill();
    }
    public void SetSkill()
    {
        skillsManager.SetSkill(this);
    }

    // ���� - ����� ī�� ������ ��ü �� 5�徿 �ǵ���
    public void UseSkill()
    {
        // photonView.RPC("UseSkillSignal", RpcTarget.All);
        // �ɷ±��� + ��ȭ�鿡 ������ �ִϸ��̼� or ����Ʈ ����
    }

    [PunRPC]
    public void UseSkillSignal()
    {
        // �ٸ����� ���忡�� �ñر� ����ߴٴ°� ���ϸ��� �ִϸ��̼� or ����Ʈ ���⿡ ����
    }
}
