using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EquipmentScript : MonoBehaviour
{
    [TabGroup("Equipment Slots")] public EquipmentSO unarmed, mainHand, offHand, head, top, bottom;
    [TabGroup("Skill Slots")] public SkillSlot upSlot, downSlot, leftSlot, rightSlot;
    public Combo upCombo, downCombo, leftCombo, rightCombo;
    [TabGroup("Containers")] public Transform mainHandContainer, slashContainer, particleContainer;
    public SkillSlot selectedSlot;

    public TutorialSO skillTutorial;
    public TutorialSO hoeTutorial;

    SkillHandler handler;
    AttackScript attackScript;
    Animator anim;
    Status status;
    public bool wateringCan;
    // Start is called before the first frame update
    private void Awake()
    {
        status = GetComponent<Status>();
        attackScript = GetComponent<AttackScript>();
        anim = GetComponentInChildren<Animator>();
        handler = GetComponent<SkillHandler>();
        DataManager.Instance.saveDataEvent += SaveSkills;
        DataManager.Instance.loadDataEvent += LoadSkills;
    }

    void Start()
    {

        SetupWeapon();
        handler.lvlEvent += UpdateMoves;
    }

    void DeleteChildren(Transform tempTransform)
    {
        int children = tempTransform.childCount;
        for (int i = 0; i < children; ++i)
            Destroy(tempTransform.GetChild(i).gameObject);

    }

    void LoadSkills()
    {
        leftSlot.skill = ItemDatabase.Instance.GetSkill(DataManager.Instance.currentSaveData.activeSkills[0]);
        upSlot.skill = ItemDatabase.Instance.GetSkill(DataManager.Instance.currentSaveData.activeSkills[1]);
        downSlot.skill = ItemDatabase.Instance.GetSkill(DataManager.Instance.currentSaveData.activeSkills[2]);
        rightSlot.skill = ItemDatabase.Instance.GetSkill(DataManager.Instance.currentSaveData.activeSkills[3]);

        UpdateMoves();
    }

    void SaveSkills()
    {
        DataManager.Instance.currentSaveData.activeSkills = new int[4];
        DataManager.Instance.currentSaveData.activeSkills[0] = ItemDatabase.Instance.GetSkillID(leftSlot.skill);
        DataManager.Instance.currentSaveData.activeSkills[1] = ItemDatabase.Instance.GetSkillID(upSlot.skill);
        DataManager.Instance.currentSaveData.activeSkills[2] = ItemDatabase.Instance.GetSkillID(downSlot.skill);
        DataManager.Instance.currentSaveData.activeSkills[3] = ItemDatabase.Instance.GetSkillID(rightSlot.skill);
    }

    void UpdateMoves()
    {
        //if (upSlot.skill != null)
        //{
        //    if (upSlot.skill.level >= 8) { upCombo.moves[0] = upSlot.skill.move3; }
        //    else if (upSlot.skill.level >= 4) { upCombo.moves[0] = upSlot.skill.move2; }
        //    else
        //        upCombo.moves[0] = upSlot.skill.move;
        //}
        //else upCombo.moves[0] = null;

        //if (downSlot.skill != null)
        //{
        //    if (downSlot.skill.level >= 8) { downCombo.moves[0] = downSlot.skill.move3; }
        //    else if (downSlot.skill.level >= 4) { downCombo.moves[0] = downSlot.skill.move2; }
        //    else
        //        downCombo.moves[0] = downSlot.skill.move;
        //}
        //else downCombo.moves[0] = null;

        //if (leftSlot.skill != null)
        //{
        //    if (leftSlot.skill.level >= 8) { leftCombo.moves[0] = leftSlot.skill.move3; }
        //    else if (leftSlot.skill.level >= 4) { leftCombo.moves[0] = leftSlot.skill.move2; }
        //    else
        //        leftCombo.moves[0] = leftSlot.skill.move;
        //}
        //else leftCombo.moves[0] = null;

        //if (rightSlot.skill != null)
        //{
        //    if (rightSlot.skill.level >= 8) { rightCombo.moves[0] = rightSlot.skill.move3; }
        //    else if (rightSlot.skill.level >= 4) { rightCombo.moves[0] = rightSlot.skill.move2; }
        //    else
        //        rightCombo.moves[0] = rightSlot.skill.move;
        //}
        //else rightCombo.moves[0] = null;
    }

    public void SelectSlot(SkillSlot slot)
    {
        RemoveHighlight();
        selectedSlot = slot;
        selectedSlot.highlightGO.SetActive(true);
    }

    public void ChangeSkill(SkillSlot slot)
    {
        //if (selectedSlot == null) return;

        //if (slot.skill != null)
        //{
        //    if (!DataManager.Instance.currentSaveData.triggers.skillTutorial2)
        //    {
        //        DataManager.Instance.currentSaveData.triggers.skillTutorial2 = true;
        //        TutorialManager.Instance.StartTutorial(skillTutorial);
        //    }
        //}

        selectedSlot.skill = slot.skill;
        selectedSlot.UpdateSkill();
        UpdateMoves();
        RemoveHighlight();
    }

    void RemoveHighlight()
    {
        if (selectedSlot == null) return;
        upSlot.highlightGO.SetActive(false);
        downSlot.highlightGO.SetActive(false);
        leftSlot.highlightGO.SetActive(false);
        rightSlot.highlightGO.SetActive(false);
        selectedSlot.highlightGO.SetActive(false);
        selectedSlot = null;
    }

    private void OnDisable()
    {
        RemoveHighlight();
    }

    void ResetWeapon()
    {
        DeleteChildren(mainHandContainer);
    }

    public void Unarmed()
    {
        ResetWeapon();
        //status.rawStats.attack = 0;
        //attackScript.moveset = unarmed.moveset;

        //if (unarmed.controller != null)
        //    anim.runtimeAnimatorController = unarmed.controller;

        //GameObject tempEquipment = Instantiate(unarmed.equipmentGO, mainHandContainer);
        //attackScript.weaponParticles = tempEquipment.GetComponent<WeaponParticles>();
        //UpdateMoves();
    }

    public void SetupWeapon()
    {
        if (mainHand == null) { Unarmed(); return; }
        ResetWeapon();

        //if (mainHand.name.Contains("Hoe"))
        //{
        //    if (!DataManager.Instance.currentSaveData.triggers.farmingTutorial2 && DataManager.Instance.currentSaveData.triggers.leciaTriggerStep >= 0)
        //    {
        //        DataManager.Instance.currentSaveData.triggers.farmingTutorial2 = true;
        //        TutorialManager.Instance.StartTutorial(hoeTutorial);
        //    }
        //}
        //wateringCan = (mainHand.name.Contains("Can"));
        status.currentStats.attack = mainHand.attack;
        attackScript.moveset = mainHand.moveset;

        if (mainHand.controller != null)
            anim.runtimeAnimatorController = mainHand.controller;

        GameObject tempEquipment = Instantiate(mainHand.gameObject, mainHandContainer);
       // Destroy(tempEquipment.GetComponent<Rigidbody>());
        //Destroy(tempEquipment.GetComponent<ItemScript>());
        //attackScript.weaponParticles = tempEquipment.GetComponent<WeaponParticles>();
        ////WeaponScript weaponScript = tempEquipment.GetComponent<WeaponScript>();
        ////attackScript.weapon = weaponScript;
        UpdateMoves();
    }
}
