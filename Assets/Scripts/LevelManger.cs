using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManger : MonoBehaviour
{

    public static LevelManger Instance;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private readonly float kCarLength = 2.7f;

    [SerializeField]
    private GameObject m_floorTile;

    public void PopulateGameBoard(int width, int length)
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < length; ++j)
            {
                if (null != m_floorTile)
                    Instantiate(m_floorTile, new Vector3(i * kCarLength, 0, j * kCarLength), Quaternion.identity);
                else
                    Debug.LogError("please supply a floor tile, dumbass.");
            }
        }   
    }

}
