using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextReferencer : MonoBehaviour
{
    TextMeshProUGUI text;
    public TextMeshProUGUI textRef;


    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        text.text = textRef.text;
    }
}
