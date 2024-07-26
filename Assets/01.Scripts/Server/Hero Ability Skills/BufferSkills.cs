using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BufferSkills : MonoBehaviourPunCallbacks, SkillsInterface
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

    // 버퍼 - 회중시계 > 영웅 능력을 누르면 시간 45초로 회복
    public void UseSkill()
    {
        photonView.RPC("UseSkillSignal", RpcTarget.All);
        photonView.RPC("TimePlusMinus", RpcTarget.All, 45); //능력 구현 완료
        // 능력구현 + 내화면에 보여줄 애니메이션 or 이팩트 구현
    }

    [PunRPC]
    public void UseSkillSignal()
    {
        // 다른유저 입장에서 궁극기 사용했다는걸 보일만한 애니메이션 or 이팩트 여기에 적기
    }
}
