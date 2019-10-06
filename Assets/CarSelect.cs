using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class CarSelect : MonoBehaviour
{
    public CarController[] cars;
    public CarPalletteSO grey;
    public CarPalletteSO[] carPallettes;
    public InputManager[] playerInputs;
    public int playerCount;
    public List<int> playerControllerMappings = new List<int>();
    public List<CarPalletteSO> availablePall = new List<CarPalletteSO>();
    public List<PlayerCustomizer> customizers = new List<PlayerCustomizer>();
    private int m_lockinCount = 0;
    public Text[] CarNames;

    public int lockInCount
    {
        get
        {
            return m_lockinCount;
        }

        set
        {
            if (value == playerCount && playerCount > 1)
            {
                GameManager.Instance.LoadRound(
                    playerCount,
                    playerControllerMappings.ToArray(),
                    customizers.Select(mizer => carPallettes[mizer.CurrentSkin]).ToArray()
                    );
            }
            else
            {
                m_lockinCount = value;
                foreach (var mizers in customizers)
                    mizers.CurrentSkin = mizers.CurrentSkin;
            }
        }
    }
    void Start()
    {
        playerInputs = new InputManager[] { new InputManager(0), new InputManager(1), new InputManager(2), new InputManager(3) };

        cars[0].Pallette = grey;
        cars[1].Pallette = grey;
        cars[2].Pallette = grey;
        cars[3].Pallette = grey;

        availablePall.AddRange(carPallettes);
        PlayerCustomizer.lockedSkins.Clear();
    }


    void Update()
    {

        foreach (var mizers in customizers)
            mizers.Update();
        logInPlayer();
    }

    void logInPlayer()
    {

        for (int i = 0; i < playerInputs.Length; i++)
        {
            if (playerLogedIn(i)) continue;
            if (playerInputs[i].Submit)
            {
                playerCount++;
                playerControllerMappings.Add(i);
                var playerIndex = playerControllerMappings.Count - 1;
                cars[playerIndex].Pallette = carPallettes[0];
                customizers.Add(new PlayerCustomizer(playerIndex, playerInputs[i], this, cars[playerIndex]));
                MakeExclusive();
            }
        }
    }

    bool playerLogedIn(int controllerIndex)
    {
        return playerControllerMappings.Contains(controllerIndex);
    }

    void MakeExclusive()
    {
        foreach (var mizers in customizers)
        {
            if (PlayerCustomizer.lockedSkins.Contains(mizers.CurrentSkin) && !mizers.lockedIn)
            {
                for (int i = 0; i < carPallettes.Length; i++)
                {
                    if (!PlayerCustomizer.lockedSkins.Contains(i))
                    {
                        mizers.SetSkin(i);
                        break;
                    }
                }
            }
        }
    }

    public class PlayerCustomizer
    {
        private int m_playerIndex;
        private InputManager m_inputManager;
        private CarSelect m_daddy;
        private CarController m_car;
        private int currentSkin = 0;
        public bool lockedIn = false;
        public static List<int> lockedSkins = new List<int>();
        public int CurrentSkin
        {
            get
            {
                return currentSkin;
            }

            set
            {   
                if (value > m_daddy.carPallettes.Length - 1)
                {
                    currentSkin = 0;
                }
                else if (value < 0)
                {
                    currentSkin = m_daddy.carPallettes.Length - 1;
                }
                else
                {
                    currentSkin = value;
                }
                m_daddy.CarNames[m_playerIndex].text = m_daddy.carPallettes[currentSkin].name;
                var color = m_daddy.carPallettes[currentSkin].Body;
                color.a = 1;
                m_daddy.CarNames[m_playerIndex].color = color;
            }
        }
        public PlayerCustomizer(int playerIndex, InputManager inputManager, CarSelect daddy, CarController car)
        {
            m_inputManager = inputManager;
            m_playerIndex = playerIndex;
            m_daddy = daddy;
            m_car = car;
            m_daddy.CarNames[m_playerIndex].text = m_daddy.carPallettes[currentSkin].name;
            var color = m_daddy.carPallettes[currentSkin].Body;
            color.a = 1;
            m_daddy.CarNames[m_playerIndex].color = color;
        }

        public void Update()
        {
            if (m_inputManager.Submit)
            {
                if (lockedIn)
                    Unlock();
                else
                    Lockin();
            }

            if (lockedIn)
            {
                var pivot = m_car.transform.parent;
                pivot.transform.localRotation = Quaternion.Slerp(pivot.transform.localRotation, Quaternion.Euler(0, 180, 0), 5f * Time.deltaTime);
            }
            else
            {
                SkinSelect();
            }
        }

        public void SkinSelect()
        {
            if (m_inputManager.LeftBumper)
            {
                do
                    CurrentSkin--;
                while (lockedSkins.Contains(CurrentSkin));


                m_car.Pallette = m_daddy.carPallettes[CurrentSkin];
            }

            if (m_inputManager.RightBumper)
            {
                do
                    CurrentSkin++;
                while (lockedSkins.Contains(CurrentSkin));
                    
                m_car.Pallette = m_daddy.carPallettes[CurrentSkin];
            }
        }

        public void SetSkin(int i)
        {
            CurrentSkin = i;
            m_car.Pallette = m_daddy.carPallettes[CurrentSkin];
        }

        public void Lockin()
        {
            lockedIn = true;
            var pivot = m_car.transform.parent;
            pivot.GetComponent<Animation>().enabled = false;
            m_daddy.lockInCount++;
            lockedSkins.Add(currentSkin);
            m_daddy.MakeExclusive();
        }

        public void Unlock()
        {
            lockedIn = false;
            var pivot = m_car.transform.parent;
            pivot.GetComponent<Animation>().enabled = true;
            m_daddy.lockInCount--;
            lockedSkins.Remove(currentSkin);
        }
    }
}
