using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsManager : MonoBehaviour
{
    private SkillsInterface _currentSkill;

    public void SetSkill(SkillsInterface skill)
    {
        _currentSkill = skill; 
    }

    public void UseCurrentSkill()
    {
        _currentSkill?.UseSkill();
    }
}
