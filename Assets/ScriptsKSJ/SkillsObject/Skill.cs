using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInterface;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Object/Skill", order = 0)]
public class Skill : ScriptableObject
{
    [SerializeField]
    private string skillName;
    public string SkillName
    {
        get { return skillName; }
    }
    [SerializeField]
    private string skillInfo;
    public string SkillInfo
    {
        get { return skillInfo; }
    }
    public int cost;
    public ANIMAL_COST_TYPE costType;
    public CARD_SKILL_TYPE skillType;
}
