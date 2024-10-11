using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{

    public TextMeshProUGUI damageText;
    [HideInInspector]
    public Color32 textColor;
    // Start is called before the first frame update
    void Start()
    {
        damageText.color = textColor;
    }

}
