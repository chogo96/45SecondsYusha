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
        _heroSkillButton.interactable = true;
    }

    public void SetSkill(SkillsInterface skill)
    {
        _currentSkill = skill; 
    }

    public void UseCurrentSkill()
    {
        _heroSkillButton.interactable = false;
        _currentSkill?.UseSkill();
        _heroSkillButton.interactable = false;
    }
}
