using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankerSkills : MonoBehaviourPunCallbacks, SkillsInterface
{
    public void UseSkill()
    {
        photonView.RPC("UseSkillSignal", RpcTarget.All);
        // �ɷ±��� + ��ȭ�鿡 ������ �ִϸ��̼� or ����Ʈ ����
    }

    [PunRPC]
    public void UseSkillSignal()
    {
        // �ٸ����� ���忡�� �ñر� ����ߴٴ°� ���ϸ��� �ִϸ��̼� or ����Ʈ ���⿡ ����
    }
}
