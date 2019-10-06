using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelect : MonoBehaviour
{
    public CarController[] cars;
    public CarPalletteSO grey;
    public CarPalletteSO[] carPallettes;
    public InputManager[] playerInputs;
    public int playerCount;
    public List<int> playerControllerMappings = new List<int>();
    public List<CarPalletteSO> availablePall = new List<CarPalletteSO>();

    void Start()
    {
        playerInputs = new InputManager[] { new InputManager(0), new InputManager(1), new InputManager(2), new InputManager(3) };

        cars[0].Pallette = grey;
        cars[1].Pallette = grey;
        cars[2].Pallette = grey;
        cars[3].Pallette = grey;

        availablePall.AddRange(carPallettes);
    }


    void Update()
    {
        logInPlayer();
    }

    void logInPlayer()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i].Submit)
            {
                playerCount++;
                playerControllerMappings.Add(i);
                cars[i].Pallette = carPallettes[0];

            }
        }
    }

    void changeCarColor()
    {
        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerInputs[i].TapLeft && playerLogedIn(i))
            {

            }
        }
    }

    bool playerLogedIn(int controllerIndex)
    {
        return playerControllerMappings.Contains(controllerIndex);
    }
}
