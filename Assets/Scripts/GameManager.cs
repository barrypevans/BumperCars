using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static readonly string kMainLevelName = "Play";

    private int m_playerCount = -1;
    private int[] m_playerContollerMappings;
    private CarPalletteSO[] m_palletts = null;

    private List<int> m_remainingPlayers; 

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
        }
        else
        {
            Destroy(this);
        }
    }

    public void LoadRound(int playerCount, int[] controllerMappings, CarPalletteSO[] pallettes)
    {
        if (playerCount >= 2 && pallettes.Length == playerCount && controllerMappings.Length == playerCount)
        {
            m_playerCount = playerCount;
            m_playerContollerMappings = controllerMappings;
            m_palletts = pallettes;

            SceneManager.LoadScene(kMainLevelName);
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == kMainLevelName)
        {
            InitializeMainLevel();
        }
    }

    private void InitializeMainLevel()
    {
        m_remainingPlayers = new List<int>() {1,2,3,4};

        var cars = GameObject.FindObjectsOfType<CarController>();
        var carindex = 0;
        foreach(var car in cars)
        {
            if (carindex < m_playerCount)
            {
                car.Pallette = m_palletts[carindex];
                car.Init(m_playerContollerMappings[carindex], carindex);
            }
            else
            {
                Destroy(car.gameObject);
                PlayerDied(carindex);
            }
            ++carindex;
        }
    }

    private void AwardWinner(int index)
    {
        print("Player "+ index.ToString()+ " wins!");
    }

    public void PlayerDied(int index)
    {
        if (m_remainingPlayers.Count() == 2)
        {
            AwardWinner(index);
        }
        m_remainingPlayers.Remove(index);
    }
}
