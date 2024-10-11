using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodesChanger : MonoBehaviour
{
    public List<MapNodes> mapNodes = new List<MapNodes>();


    [ContextMenu("UpdateNodes")]
    public void UpdateNodes()
    {
        List<int> fragmentsSelected = new List<int>();
        foreach(MapNodes node in mapNodes)
        {
            if(fragmentsSelected.Count == mapNodes.Count)
            {
                fragmentsSelected.Clear();
            }
            int fragmentSelected = Random.Range(0, node.fragments.Count);
            while (fragmentsSelected.Contains(fragmentSelected))
            {
                fragmentSelected = Random.Range(0, node.fragments.Count);
            }
            fragmentsSelected.Add(fragmentSelected);
            node.UpdateFragment(fragmentSelected);
        }
    }
}
