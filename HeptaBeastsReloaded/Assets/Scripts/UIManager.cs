using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI dmgText;
    public Slider shieldSlider;
    public TextMeshProUGUI shieldText;
    public Slider stunSlider;
    public List<HabilityUIIndicator> habIndicators = new List<HabilityUIIndicator>();

    public GameObject pauseMenu;
    public GameObject habIndicatorsGamepad;
    public GameObject habIndicatorsKeyboard;

    public PjBase ch;

    private void Awake()
    {
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (ch != null)
        {
            UpdateHpBars();
        }

        UpdateHabIndicators();

        
    }

    public void Pause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = GameManager.Instance.ingameSpeed;
            pauseMenu.SetActive(false); 
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            Cursor.visible = true;
        }
    }

    void UpdateHpBars()
    {
        hpSlider.value = ch.stats.hp;
        hpSlider.maxValue = ch.stats.mHp;
        hpText.text = ch.stats.hp.ToString("F0");

        ch.stunnBar = stunSlider;

        shieldSlider.value = ch.stats.shield;
        shieldSlider.maxValue = ch.stats.mHp*1.5f;
        if (ch.stats.shield > 0)
        {
            shieldText.text = ch.stats.shield.ToString("F0");
        }
        else
        {
            shieldText.text = "";
        }

        stunSlider.value = ch.stunTime;
    }


    public void UpdateHabIndicatorsImages()
    {
        if (ch.currentMove1)
        {
            habIndicators[0].UpdateImage(ch.currentMove1.sprite);
            habIndicators[3].UpdateImage(ch.currentMove1.sprite);
        }
        if (ch.currentMove2)
        {
            habIndicators[1].UpdateImage(ch.currentMove2.sprite);
            habIndicators[4].UpdateImage(ch.currentMove2.sprite);
        }
        if (ch.currentMove3)
        {
            habIndicators[2].UpdateImage(ch.currentMove3.sprite);
            habIndicators[5].UpdateImage(ch.currentMove3.sprite);
        }
    }


    void UpdateHabIndicators()
    {
        if (ch.controller)
        {
            if (ch.controller.isUsingGamepad)
            {
                habIndicatorsGamepad.SetActive(true);
                habIndicatorsKeyboard.SetActive(false);
            }
            else
            {
                habIndicatorsKeyboard.SetActive(true);
                habIndicatorsGamepad.SetActive(false);
            }
        }

        if (ch != null && ch.stats.hp > 0)
        {
            if (ch.currentMove1)
            {
                habIndicators[0].UIUpdate(ch.currentMove1.cd, ch.currentHab1Cd);
                habIndicators[3].UIUpdate(ch.currentMove1.cd, ch.currentHab1Cd);
            }
            if (ch.currentMove2)
            {
                habIndicators[1].UIUpdate(ch.currentMove2.cd, ch.currentHab2Cd);
                habIndicators[4].UIUpdate(ch.currentMove2.cd, ch.currentHab2Cd);
            }
            if (ch.currentMove3)
            {
                habIndicators[2].UIUpdate(ch.currentMove3.cd, ch.currentHab3Cd);
                habIndicators[5].UIUpdate(ch.currentMove3.cd, ch.currentHab3Cd);
            }
        }
    }

    public void UpdateDamageText()
    {
        dmgText.text = ch.dmgDealed.ToString("F0");
    }
}
