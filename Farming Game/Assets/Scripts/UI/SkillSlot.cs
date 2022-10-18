using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SkillSlot : MonoBehaviour
{
    public enum Type { ActiveSlot, Menu, Hotkey }
    public Type type;
    public SkillType skillType;
    Button thisButton;
    public Image itemSprite;
    public Skill skill;

    public GameObject hoverSFX;
    public GameObject clickSFX;
    public bool selected = false;
    EquipmentScript equip;
    SkillHandler skillHandler;
    public GameObject highlightGO;

    public TextMeshProUGUI levelText;
    public Slider expSlider;
    public SkillSlot quickslotTarget;

    public enum ButtonType
    {
        West, North, South, East
    }

    // Start is called before the first frame update
    private void Awake()
    {
        thisButton = GetComponent<Button>();
        equip = PlayerInputHandler.Instance.GetComponent<EquipmentScript>();
        skillHandler = PlayerInputHandler.Instance.GetComponent<SkillHandler>();
    }


    void Start()
    {

        if (type == Type.Hotkey) return;
        InputManager.Instance.westInput += WestButton;
        InputManager.Instance.northInput += NorthButton;
        InputManager.Instance.southInput += NorthButton;
        InputManager.Instance.eastInput += NorthButton;


        thisButton.onClick.AddListener(ButtonClicked);

    }

    // Update is called once per frame


    public void OnHover()
    {
        selected = true;
        skillHandler.SkillUI(skill);
        //Instantiate(hoverSFX);
    }
    public void Deselect()
    {
        selected = false;
    }



    void WestButton()
    {

    }
    void NorthButton()
    {

    }
    void SouthButton()
    {

    }
    void EastButton()
    {

    }


    private void OnEnable()
    {
        CheckSkillUnlock();
        UpdateSkill();
        //skillHandler.expEvent += UpdateSkill;
    }

    private void OnDisable()
    {
        //skillHandler.expEvent -= UpdateSkill;
    }

    private void OnValidate()
    {
        UpdateSkill();
    }

    void CheckSkillUnlock()
    {
        if (skill == null) return;
        bool unlocked = false;
        for (int i = 0; i < DataManager.Instance.currentSaveData.skillData.Count; i++)
        {

        }
        if (skill.level > 0)
        {
            unlocked = true;
        }

        if (!unlocked) gameObject.SetActive(false);

    }

    public void UpdateSkill()
    {
        if (type == Type.Hotkey)
        {
            skill = quickslotTarget.skill;
        }

        if (skill != null)
        {
            itemSprite.color = Color.white;
            itemSprite.sprite = skill.sprite;

            expSlider.maxValue = Mathf.Pow(1.5F, skill.level) * 100;
            expSlider.value = skill.experience;

            levelText.text = "" + skill.level;

            expSlider.gameObject.SetActive(true);
            levelText.gameObject.SetActive(true);
        }
        else
        {
            itemSprite.color = Color.clear;
            itemSprite.sprite = null;
            expSlider.gameObject.SetActive(false);
            levelText.gameObject.SetActive(false);
        }

    }

    void ButtonClicked()
    {
        if (type == Type.ActiveSlot)
        {
            equip.SelectSlot(this);
        }
        else
        {
            if (skillType == SkillType.Active)
                equip.ChangeSkill(this);
        }
        //Instantiate(clickSFX);
    }
}
