using UnityEngine;
using System.Collections;
using Photon.Realtime;

public abstract class CreatureEffect
{
    protected Player owner;
    protected CreatureLogic creature;
    protected int specialAmount;

    public CreatureEffect(Player owner, CreatureLogic creature, int specialAmount)
    {
        this.creature = creature;
        this.owner = owner;
        this.specialAmount = specialAmount;
    }

    public virtual void RegisterEventEffect() { }

    public virtual void UnRegisterEventEffect() { }

    public virtual void CauseEventEffect() { }

    public virtual void WhenACreatureIsPlayed() { }

    public virtual void WhenACreatureDies() { }
}
