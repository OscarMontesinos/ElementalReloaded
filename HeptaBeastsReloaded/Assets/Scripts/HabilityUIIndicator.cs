using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabilityUIIndicator : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    public GameObject content;
    public Image image;

    public void UIUpdate(float maxCD, float cd)
    {
        slider.maxValue = maxCD;
        slider.value = cd;
        if (cd > 0)
        {
            text.text = cd.ToString("F0");
        }
        else
        {
            text.text = "";
        }
    }

    public void UpdateImage(Sprite image)
    {
        this.image.sprite = image;
    }

    public void Activate(bool value)
    {
        content.SetActive(value);
    }
}
