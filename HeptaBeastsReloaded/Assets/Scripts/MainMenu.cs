using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject singlePlayerGM;
    public GameObject multiPlayerGM;
    private void Start()
    {
    }
    public void Singleplayer()
    {
        SceneManager.LoadScene("MapScene");
    }
    public void Multiplayer()
    {
    }
}
