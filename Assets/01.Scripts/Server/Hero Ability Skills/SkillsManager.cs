using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsManager : MonoBehaviour
{
    private SkillsInterface _currentSkill;
    private Button _heroSkillButton;

    private void Awake()
    {
        _heroSkillButton = transform.Find("Button - HeroSkillButton").GetComponent<Button>();
    }

    private void Start()
    {
        _heroSkillButton.onClick.AddListener(UseCurrentSkill);
    }

    public void SetSkill(SkillsInterface skill)
    {
        _currentSkill = skill; 
    }

    public void UseCurrentSkill()
    {
        _currentSkill?.UseSkill();
    }
}
