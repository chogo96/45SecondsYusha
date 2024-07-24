using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BufferSkills : MonoBehaviourPunCallbacks, SkillsInterface
{
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
