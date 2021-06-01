// Класс PlayerUI.cs описывает интерфейс игрока
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{
    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private Text playerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;

    [SerializeField]
    private Text cybeMana;
    [SerializeField]
    private Text sphereMana;
    [SerializeField]
    private Text cylinderMana;
    [SerializeField]
    private Text pyramidMana;
    [SerializeField]
    private Text experience;

    public void SetName(string name)
    {
        playerNameText.text = name;
    }

    public void EditHealthSlider(float val)
    {
        playerHealthSlider.value -= val;
    }

    public void EditCybeMana(int val)
    {
        cybeMana.text = "Cybe mana: " + val.ToString();
    }
    public void EditSphereMana(int val)
    {
        sphereMana.text = "Sphere mana: " + val.ToString();
    }
    public void EditCylinderMana(int val)
    {
        cylinderMana.text = "Cylinder mana: " + val.ToString();
    }
    public void EditPyramidMana(int val)
    {
        pyramidMana.text = "Pyramid mana: " + val.ToString();
    }
    public void EditExp(int val)
    {
        experience.text = "Experience: " + val.ToString();
    }

    public void ClickOnSkill()
    {
        GameManager.S.SkillShot();
    }
}
