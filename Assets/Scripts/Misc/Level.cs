using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] int levelSection;
    [SerializeField] Transform spawnPoint;
    [SerializeField] int startingLives = 3;
    [SerializeField] int startingHP = 1;
    [SerializeField] int startingScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SpawnPlayer(spawnPoint, levelSection);
        if (levelSection == 1)
        {
            GameManager.Instance.currentLives = startingLives;
            GameManager.Instance.currentHP = startingHP;
            GameManager.Instance.currentScore = startingScore;
        }
        else
        {
            GameManager.Instance.currentLives = GameManager.Instance.currentLives;
            GameManager.Instance.currentHP = GameManager.Instance.currentHP;
            GameManager.Instance.currentScore = GameManager.Instance.currentScore;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
