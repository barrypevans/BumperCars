﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LevelManger : MonoBehaviour
{

    public static LevelManger Instance;
    public bool matteColoring = false;
    private static Dictionary<Vector2,Tile> tiles;

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
        tiles = new Dictionary<Vector2, Tile>();
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < length; ++j)
            {
                if (null != m_floorTile)
                {
                    tiles.Add(new Vector2(i,j),Instantiate(m_floorTile, new Vector3((i - (width / 2.0f)) * kCarLength, 0, (j - (length / 2.0f)) * kCarLength), Quaternion.identity).GetComponent<Tile>());
                }
                else
                    Debug.LogError("please supply a floor tile, dumbass.");
            }
        }   
    }

    public void TakeOutTile(Vector3 position,Color car1, Color car2)
    {
        RaycastHit[] hits = Physics.RaycastAll(position+ Vector3.up, Vector3.down);
       
        var tileHits = hits.Where(hit => hit.collider.tag == "GroundTile");
        if (tileHits.Count() > 0)
        {
            //Color adjacent tiles
            Tile pingTile = tileHits.First().collider.GetComponent<Tile>();
            if (matteColoring)
            {
                var index = tiles.Where(tile => tile.Value == pingTile).First();
                Color mix = Color.Lerp(car1, car2, .5f);

                SetRelativeTileColor(index, new Vector2(1, 0), mix);
                SetRelativeTileColor(index, new Vector2(-1, 0), mix);
                SetRelativeTileColor(index, new Vector2(0, 1), mix);
                SetRelativeTileColor(index, new Vector2(0, -1), mix);

                Vector3 colorAmp = new Vector3();
                Color.RGBToHSV(mix, out colorAmp.x, out colorAmp.y, out colorAmp.z);
                colorAmp.y *= .7f;
                colorAmp.z *= 1.1f;

                mix = Color.HSVToRGB(colorAmp.x, colorAmp.y, colorAmp.z);

                SetRelativeTileColor(index, new Vector2(2, 0), mix);
                SetRelativeTileColor(index, new Vector2(-2, 0), mix);
                SetRelativeTileColor(index, new Vector2(0, 2), mix);
                SetRelativeTileColor(index, new Vector2(0, -2), mix);
                SetRelativeTileColor(index, new Vector2(1, 1), mix);
                SetRelativeTileColor(index, new Vector2(1, -1), mix);
                SetRelativeTileColor(index, new Vector2(-1, 1), mix);
                SetRelativeTileColor(index, new Vector2(-1, -1), mix);

            }
                       
            pingTile.Destroy();
        }
    }

    private void SetRelativeTileColor(KeyValuePair<Vector2, Tile> index, Vector2 v, Color c)
    {
        if(tiles.ContainsKey(index.Key + v))
            tiles[index.Key + v].LerpColor(c);

    }

}
