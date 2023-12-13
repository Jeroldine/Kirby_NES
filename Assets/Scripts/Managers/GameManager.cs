using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    public static GameManager Instance => instance;
    int gameOverSceneIndex;
    public UnityEvent<int> OnHPValueChanged;
    public UnityEvent<int> OnLivesValueChanged;
    public UnityEvent<int> OnScoreValueChanged;

    [SerializeField] PlayerController playerPrefb;
    [HideInInspector] public PlayerController playerInstance;
    [HideInInspector] Transform spawnPoint;
    bool isInputDisabled = false;
    float delayAmount = 0f;
    float delayTimer = 0f;

    [SerializeField] AudioClip maxTomatoSound;
    [SerializeField] AudioClip bottleSound;
    [SerializeField] AudioClip lifeUpSound;

    // scene build index
    private int _currentSceneIndex;
    public int currentSceneIndex
    {
        get => _currentSceneIndex;
        set
        {
            _currentSceneIndex = value;
        }
    }

    private int _contSceneIndex;
    public int contSceneIdex
    {
        get => _contSceneIndex;
        set
        {
            _contSceneIndex = value;
        }
    }

    // Health and lives
    [SerializeField] int maxHP = 6;
    private int _currentHP = 4;
    public int currentHP
    {
        get { return _currentHP; }
        set
        {
            _currentHP = value;
            if (_currentHP > 0)
                OnHPValueChanged?.Invoke(_currentHP);
            if (_currentHP > maxHP)
                _currentHP = maxHP;
            else if (_currentHP <= 0)
                currentLives--;

            Debug.Log("HP has been set to: " + _currentHP.ToString());
        }
    }

    [SerializeField] int maxLives = 99;
    private int _currentLives = 3;
    public int currentLives
    {
        get { return _currentLives; }
        set
        {
            if (value < 0)
            {
                //Debug.Log("Line before loading to game over screen.");
                LoadLevel(gameOverSceneIndex);
                return;
            }

            if (_currentLives > value)
            {
                Debug.Log("Respawned");
                Respawn();
            }
                
            _currentLives = value;

            if (_currentLives > maxLives)
                _currentLives = maxLives;

            Debug.Log("Lives has been set to: " + _currentLives.ToString());
            OnLivesValueChanged?.Invoke(_currentLives);
        }
    }

    // score 
    [SerializeField] int maxScore = 9999999;
    private int _currentScore = 0;
    public int currentScore
    {
        get => _currentScore;
        set
        {
            _currentScore = value;
            if (_currentScore > maxScore)
                _currentScore = maxScore;
            OnScoreValueChanged?.Invoke(_currentScore);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        gameOverSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //Debug.Log("gameOverSceneIndex" + gameOverSceneIndex.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isInputDisabled)
        {
            if (SceneManager.GetActiveScene().name == "Intro")
            {
                //_currentSceneIndex++;
                SceneManager.LoadScene(++_currentSceneIndex);
            }
            //else if (SceneManager.GetActiveScene().name == "TitleScreen")
            //{
            //    _currentSceneIndex++;
            //    SceneManager.LoadScene(_currentSceneIndex);
            //}

            //else if (SceneManager.GetActiveScene().name == "GameOver")
            //    _currentSceneIndex = 1;
            //    SceneManager.LoadScene(_currentSceneIndex);
        }

        if (isInputDisabled)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= delayAmount)
                isInputDisabled = false;
        }
    }

    public void SpawnPlayer(Transform spawnLocation, int lvlSection)
    {
        //if (!playerInstance)
        //    playerInstance = Instantiate(playerPrefb, spawnLocation.position, spawnLocation.rotation);
        //else
        //    playerInstance.transform.position = spawnLocation.position;
        playerInstance = Instantiate(playerPrefb, spawnLocation.position, spawnLocation.rotation);
        if (lvlSection == 1)
            _contSceneIndex = SceneManager.GetActiveScene().buildIndex;
        spawnPoint = spawnLocation;
    }

    public void Respawn()
    {
        playerInstance.transform.position = spawnPoint.position;
        currentHP = maxHP;
    }

    public void UpdateSpawnPoint(Transform updatedPoint)
    {
        spawnPoint = updatedPoint;
    }

    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void DisableInput(bool disabledState, float delay = 0)
    {
        isInputDisabled = disabledState;
        delayTimer = 0;
        delayAmount = delay;
    }

}
