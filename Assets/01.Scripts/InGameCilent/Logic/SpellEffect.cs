using Photon.Realtime;

public abstract class SpellEffect
{
    public PlayerScripts owner;
    public abstract void ActivateEffect(PlayerScripts owner, ICharacter target);
}