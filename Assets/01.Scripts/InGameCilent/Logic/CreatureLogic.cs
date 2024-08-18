using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

[System.Serializable]
public class CreatureLogic : ICharacter
{
    public Player owner;

    //이 카드의 정보
    public CardAsset cardAsset;

    public CreatureEffect effect;

    public int UniqueCreatureID;

    public bool Frozen = false;

    public int ID
    {
        get { return UniqueCreatureID; }
    }

    private int baseHealth;
    public int MaxHealth
    {
        get { return baseHealth; }
    }

    private int health;
    public int Health
    {
        get { return health; }

        set
        {
            if (value > MaxHealth)
                health = MaxHealth;
            else if (value <= 0)
                Die();
            else
                health = value;
        }
    }

    public bool Taunt
    {
        get;
        set;
    }


    public bool CanAttack
    {
        get
        {
            bool ownersTurn = (TurnManager.instance.whoseTurn == owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && !Frozen);
        }
    }

    private int baseAttack;
    public int Attack
    {
        get { return baseAttack; }
    }

    private int attacksForOneTurn = 1;
    public int AttacksLeftThisTurn
    {
        get;
        set;
    }

    public CreatureLogic(Player owner, CardAsset cardAsset)
    {
        this.owner = owner;
        UniqueCreatureID = IDFactory.GetUniqueID();
        //if (cardAsset.CreatureScriptName != null && cardAsset.CreatureScriptName != "")
        //{
        //    effect = System.Activator.CreateInstance(System.Type.GetType(cardAsset.CreatureScriptName), new System.Object[] { owner, this, cardAsset.SpecialCreatureAmount }) as CreatureEffect;
        //    effect.RegisterEventEffect();
        //}
        CreaturesCreatedThisGame.Add(UniqueCreatureID, this);
    }

    public void OnTurnStart()
    {
        AttacksLeftThisTurn = attacksForOneTurn;
    }

    public void Die()
    {
        //    // 죽음의 메아리
        //    if (effect != null)
        //    {
        //        effect.WhenACreatureDies();
        //        effect.UnRegisterEventEffect();
        //        effect = null;
        //    }

        //    new CreatureDieCommand(UniqueCreatureID, owner).AddToQueue();
    }
// 카드 ID관리
public static Dictionary<int, CreatureLogic> CreaturesCreatedThisGame = new Dictionary<int, CreatureLogic>();

}
