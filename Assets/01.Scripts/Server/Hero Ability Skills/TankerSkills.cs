using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankerSkills : MonoBehaviourPunCallbacks, SkillsInterface
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

    // 탱커 - 인내심 > 영웅능력을 누르면 자신이 사용한 카드 덱에서 15장 되돌림
    public void UseSkill()
    {
        photonView.RPC("UseSkillSignal", RpcTarget.All);
        // 능력구현 + 내화면에 보여줄 애니메이션 or 이팩트 구현
    }

    [PunRPC]
    public void UseSkillSignal()
    {
        // 다른유저 입장에서 궁극기 사용했다는걸 보일만한 애니메이션 or 이팩트 여기에 적기
    }
}
