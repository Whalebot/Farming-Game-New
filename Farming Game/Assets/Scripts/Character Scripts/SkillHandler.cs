using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

public class SkillHandler : MonoBehaviour
{
    Status status;
    public Stats modifiedStats;
    public List<Skill> skills;
    public DescriptionWindow window;
    public GameObject skillLevelUpWindow;
    public DescriptionWindow skillUnlockWindow;
    public Transform skillLevelUpPanel;
    public SkillSlot[] slots;
    public TutorialSO skillTutorial;
    public GameObject kbQuickslots;

    public delegate void SkillEvent();
    public SkillEvent expEvent;
    public SkillEvent lvlEvent;

    private void Awake()
    {
        DataManager.Instance.saveDataEvent += SaveSkills;
        DataManager.Instance.loadDataEvent += LoadSkills;
    }

    void Start()
    {
        status = GetComponent<Status>();
        UpdateStats();
    }

    private void FixedUpdate()
    {
        //kbQuickslots.SetActive(!GameManager.isPaused && !GameManager.inventoryMenuOpen);
    }

    private void OnEnable()
    {

    }

    void SaveSkills()
    {
      //  DataManager.Instance.currentSaveData.skillData.Clear();
        foreach (Skill skill in ItemDatabase.Instance.skills)
        {
            SaveSkill(skill);
        }
    }

    void SaveSkill(Skill skill)
    {
        //if (skill == null) return;
        //SkillData data = new SkillData
        //{
        //    skillID = ItemDatabase.Instance.GetSkillID(skill),
        //    level = skill.level,
        //    experience = skill.experience
        //};
        //DataManager.Instance.currentSaveData.skillData.Add(data);
    }

    void LoadSkills()
    {
        //foreach (SkillData data in DataManager.Instance.currentSaveData.skillData)
        //{
        //    Skill skill = ItemDatabase.Instance.GetSkill(data.skillID);
        //    skill.level = data.level;
        //    skill.experience = data.experience;
        //}
    }

    public void UpdateStats()
    {
        modifiedStats = new Stats();
        foreach (Skill skill in skills)
        {
            CalculateSkillStats(skill);
        }
        ApplySkillEffects();
    }

    void UpdateSkillSlot()
    {
        foreach (SkillSlot slot in slots)
        {
            slot.gameObject.SetActive(slot.skill.level > 0);
        }
    }

    void UnlockSkill(Skill skill)
    {
        if (skill == null) skillUnlockWindow.gameObject.SetActive(false);
        else
        {
            skillUnlockWindow.gameObject.SetActive(true);
            skillUnlockWindow.DisplayUI(skill);
        }
    }

    public void SkillLevelUI(Skill skill)
    {
        //if (!DataManager.Instance.currentSaveData.triggers.skillTutorial && skill.type == SkillType.Active)
        //{
        //    DataManager.Instance.currentSaveData.triggers.skillTutorial = true;
        //   TutorialManager.Instance.StartTutorial(skillTutorial);
        //}

        skillLevelUpWindow.gameObject.SetActive(true);
        GameObject GO = Instantiate(skillLevelUpWindow, skillLevelUpPanel);
        GO.GetComponent<DescriptionWindow>().DisplayUI(skill);

    }

    public void SkillEXP(Skill skill, int exp)
    {
        expEvent?.Invoke();
        if (skill.level >= 10)
        {
            skill.experience = (int)Mathf.Pow(1.5F, skill.level) * 100;
            return;
        }
        skill.experience += exp;
        if (skill.experience >= Mathf.Pow(1.5F, skill.level) * 100 || skill.level == 0)
        {
            skill.level++;
            skill.experience = 0;
            lvlEvent?.Invoke();
            SkillLevelUI(skill);
            UpdateSkillSlot();
            UpdateStats();
            if (skill.level >= 10)
            {
                skill.experience = (int)Mathf.Pow(1.5F, skill.level) * 100;
                return;
            }
        }
    }

    public void SkillUI(Skill skill)
    {
        if (skill == null) window.gameObject.SetActive(false);
        else
        {
            window.gameObject.SetActive(true);
            window.DisplayUI(skill);
        }
    }

    public void CalculateSkillStats(Skill skill)
    {
        return;
        Stats def1 = modifiedStats;
        Stats def2 = skill.stats;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);

            if (var1 is int)
            {
                if ((int)var2 != 0)
                    defInfo1[i].SetValue(obj, (int)var1 + (int)var2);
            }
            else if (var1 is float)
            {
                if ((float)var2 != 0)
                    defInfo1[i].SetValue(obj, (float)var1 + (float)var2 * skill.level / 10);
            }
        }
    }

    public void ApplySkillEffects()
    {
        return;
        Stats def1 = status.currentStats;
        Stats def2 = modifiedStats;
        Stats def3 = status.baseStats;
        FieldInfo[] defInfo1 = def1.GetType().GetFields();
        FieldInfo[] defInfo2 = def2.GetType().GetFields();
        FieldInfo[] defInfo3 = def3.GetType().GetFields();

        for (int i = 0; i < defInfo1.Length; i++)
        {
            object obj = def1;
            object obj2 = def2;
            object obj3 = def3;

            object var1 = defInfo1[i].GetValue(obj);
            object var2 = defInfo2[i].GetValue(obj2);
            object var3 = defInfo3[i].GetValue(obj3);

            if (var1 is int)
            {
                if ((int)var2 != 0)
                    defInfo1[i].SetValue(obj, (int)var3 + (int)var2);
            }
            else if (var1 is float)
            {
                if ((float)var2 != 0)
                    defInfo1[i].SetValue(obj, (float)var3 + (float)var2);
            }
            //else if (var1 is bool)
            //{
            //    //SET VALUES
            //    if ((bool)var2)
            //        defInfo1[i].SetValue(obj, defInfo2[i].GetValue(obj2));
            //}
        }
    }
}
