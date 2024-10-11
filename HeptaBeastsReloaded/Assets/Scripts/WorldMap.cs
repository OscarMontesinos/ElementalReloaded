using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMap : MonoBehaviour
{
    public string lita;
    public string sila;
    public string silfrheim;
    public string orisil;
    public string merine;
    public string shaanti;
    public string hariken;

    GameManager manager;



    public void Lita()
    {
        SceneManager.LoadScene(lita);
        StartGame();
    }
    
    public void Sila()
    {
        SceneManager.LoadScene(sila);
        StartGame();
    }
    
    public void Silfrheim()
    {
        SceneManager.LoadScene(silfrheim);
        StartGame();
    }
    
    public void Orisil()
    {
        SceneManager.LoadScene(orisil);
        StartGame();
    }
    
    public void Merine()
    {
        SceneManager.LoadScene(merine);
        StartGame();
    }
    
    public void Shaanti()
    {
        SceneManager.LoadScene(shaanti);
        StartGame();
    }
    
    public void Hariken()
    {
        SceneManager.LoadScene(hariken);
        StartGame();
    }

    void StartGame()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.StartGame());
    }
}
