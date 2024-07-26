
using UnityEngine;

enum MyJob
{
    None,
    Attacker,
    Tanker,
    Healer,
    Buffer,
}
public interface SkillsInterface
{
    void SetSkill();
    void UseSkill();

    void UseSkillSignal();
}
