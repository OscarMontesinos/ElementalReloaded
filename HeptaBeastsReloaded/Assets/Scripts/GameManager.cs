using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    bool ingame;

    public static GameManager Instance;

    public GameModes gamemode;

    public GameObject baseController;
    public GameObject baseUIManager;
    public List<GameObject> waypoints;

    public List<List<GameObject>> teams;

    int charaterDisplayed;

    public List<PjBase> pjList = new List<PjBase>();

    public LayerMask wallLayer;
    public LayerMask airLayer;
    public LayerMask unitLayer;
    public LayerMask playerWallLayer;

    public GameObject damageText;
    public GameObject iaTeam;

    public Color32 iceColor;
    public Color32 fireColor;
    public Color32 waterColor;
    public Color32 desertColor;
    public Color32 natureColor;
    public Color32 windColor;
    public Color32 lightningColor;
    public Color32 crystalColor;

    public GameObject FoV;

    public float ingameSpeed;

    public bool spawn;
    public List<GameObject> waveSpawners;

    [Serializable]
    public struct Wave
    {
        public List<GameObject> enemies;
    }

    public List<Wave> waves;
    [HideInInspector]
    public List<GameObject> currentWave;

    public enum GameModes
    {
        singleplayer, multiplayer
    }
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Time.timeScale = ingameSpeed;

        if (spawn)
        {
            foreach (GameObject unit in waves[Random.Range(0,waves.Count)].enemies)
            {
                GameObject waveSpawner = GetWaveSpawner();
                GameObject enemy = Instantiate(unit, waveSpawner.transform.position, waveSpawner.transform.rotation);
                currentWave.Add(enemy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (currentWave.Count == 0 && spawn)
        {
            foreach (PjBase unit in pjList)
            {
                if (unit != null)
                {
                    unit.Heal(unit, unit.stats.mHp * 0.5f, unit.element1);
                }
            }

            foreach (GameObject unit in waves[Random.Range(0, waves.Count)].enemies)
            {
                GameObject waveSpawner = GetWaveSpawner();
                GameObject enemy = Instantiate(unit, waveSpawner.transform.position, waveSpawner.transform.rotation);
                currentWave.Add(enemy);
            }
        }

    }

    public static Vector2 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        Vector3 angle = new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        return new Vector2(-angle.x,angle.z);
    }

    public GameObject GetWaveSpawner()
    {
        float maxRange = 0;
        float range = 0;
        GameObject selectedSpawner = null;
        foreach (GameObject spawner in waveSpawners)
        {
            foreach (PjBase pj in pjList)
            {
                range = 0;
                range += (spawner.transform.position - pj.transform.position).magnitude;
            }


            if (spawner == null || range > maxRange)
            {
                selectedSpawner = spawner;
                maxRange = range;
            }
        }

        return selectedSpawner;
    }
}
