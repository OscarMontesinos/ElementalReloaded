using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorManager : MonoBehaviour
{
    public void Close()
    {
        GameManager.Instance.CloseSelector();
    }
}
