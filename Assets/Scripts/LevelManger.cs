using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LevelManger : MonoBehaviour
{

    public static LevelManger Instance;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PopulateGameBoard(20, 20);
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
                    Instantiate(m_floorTile, new Vector3((i  - (width/2.0f)) * kCarLength, 0, (j - (length / 2.0f)) * kCarLength), Quaternion.identity);
                else
                    Debug.LogError("please supply a floor tile, dumbass.");
            }
        }   
    }

    public void TakeOutTile(Vector3 position)
    {
        RaycastHit[] hits = Physics.RaycastAll(position+ Vector3.up, Vector3.down);
       
        var tileHits = hits.Where(hit => hit.collider.tag == "GroundTile");
        if (tileHits.Count() > 0)
        {
            print(tileHits.First());
            tileHits.First().collider.GetComponent<Tile>().Destroy();
        }
    }

}
