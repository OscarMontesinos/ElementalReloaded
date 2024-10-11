using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodes : MonoBehaviour
{
    public List<GameObject> fragments = new List<GameObject>();

    public void UpdateFragment(int fragmentSelected)
    {
        foreach(GameObject fragment in fragments)
        {
            fragment.SetActive(false);
        }
        fragments[fragmentSelected].SetActive(true);
    }
}
