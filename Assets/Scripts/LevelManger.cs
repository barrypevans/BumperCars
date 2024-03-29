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
                    tiles.Add(new Vector2(i,j),Instantiate(m_floorTile, new Vector3((i - (width / 2.0f)) * kCarLength, 0, (j - (length / 2.0f)) * kCarLength), Quaternion.identity).transform.GetChild(0).GetComponent<Tile>());
                }
                else
                    Debug.LogError("please supply a floor tile, dumbass.");
            }
        }   
    }

    Vector2 lastCollision = new Vector2(-5,-5);
    public void TakeOutTile(Vector3 position,Color car1, Color car2)
    {
        RaycastHit[] hits = Physics.RaycastAll(position+ Vector3.up*2, Vector3.down);
        var tileHits = hits.Where(hit => hit.collider.tag == "GroundTile");

        if (tileHits.Count() > 0)
        {
            Tile pingTile = tileHits.FirstOrDefault().collider.GetComponent<Tile>();
            var index = tiles.Where(tile => tile.Value == pingTile).FirstOrDefault();


            //Prevent duplicate collision reads, prevent invalid tile reads
            if (matteColoring && lastCollision!=index.Key && tiles.ContainsKey(index.Key) && tiles[index.Key] != null && index.Key!=Vector2.zero)
            {
                lastCollision = index.Key;
                Color mix = Color.Lerp(car1,car2,.5f);

                //find a strong version of the mixed color
                Vector3 normalColor = new Vector3(mix.r, mix.g, mix.b);
                float absoluteRatio = 0;
                if (normalColor.x > absoluteRatio)
                    absoluteRatio = normalColor.x;
                if (normalColor.y > absoluteRatio)
                    absoluteRatio = normalColor.y;
                if (normalColor.z > absoluteRatio)
                    absoluteRatio = normalColor.z;

                if (absoluteRatio != 0)
                {
                    absoluteRatio = 1f / absoluteRatio;
                    normalColor *= absoluteRatio;
                    mix = new Color(normalColor.x, normalColor.y, normalColor.z);
                }

                StartCoroutine(DelayedTileColoring(index,mix));

            }
                       
            pingTile.Destroy();
            //tiles.Remove(index.Key);
        }
    }

    private IEnumerator DelayedTileColoring(KeyValuePair<Vector2, Tile> index, Color mix)
    {
        SetRelativeTileColor(index, new Vector2(0, 0), mix);
        SetRelativeTileColor(index, new Vector2(1, 0), mix);
        SetRelativeTileColor(index, new Vector2(-1, 0), mix);
        SetRelativeTileColor(index, new Vector2(0, 1), mix);
        SetRelativeTileColor(index, new Vector2(0, -1), mix);

        mix.a *= .5f;
        yield return new WaitForSeconds(.5f);

        SetRelativeTileColor(index, new Vector2(2, 0), mix);
        SetRelativeTileColor(index, new Vector2(-2, 0), mix);
        SetRelativeTileColor(index, new Vector2(0, 2), mix);
        SetRelativeTileColor(index, new Vector2(0, -2), mix);
        SetRelativeTileColor(index, new Vector2(1, 1), mix);
        SetRelativeTileColor(index, new Vector2(1, -1), mix);
        SetRelativeTileColor(index, new Vector2(-1, 1), mix);
        SetRelativeTileColor(index, new Vector2(-1, -1), mix);

        mix.a *= .4f;
        yield return new WaitForSeconds(.2f);
        SetRelativeTileColor(index, new Vector2(2, 1), mix);
        SetRelativeTileColor(index, new Vector2(2, -1), mix);
        SetRelativeTileColor(index, new Vector2(1, 2), mix);
        SetRelativeTileColor(index, new Vector2(1, -2), mix);
        SetRelativeTileColor(index, new Vector2(-2, 1), mix);
        SetRelativeTileColor(index, new Vector2(-2, -1), mix);
        SetRelativeTileColor(index, new Vector2(-1, 2), mix);
        SetRelativeTileColor(index, new Vector2(-1, -2), mix);

        mix.a *= .5f;
        yield return new WaitForSeconds(.1f);
        SetRelativeTileColor(index, new Vector2(3, 0), mix);
        SetRelativeTileColor(index, new Vector2(-3, 0), mix);
        SetRelativeTileColor(index, new Vector2(0, 3), mix);
        SetRelativeTileColor(index, new Vector2(0, -3), mix);



    }

    private void SetRelativeTileColor(KeyValuePair<Vector2, Tile> index, Vector2 v, Color c)
    {
        if(tiles.ContainsKey(index.Key + v))
            tiles[index.Key + v].LerpColor(c);

    }


    public void Win()
    {
        float delay = 0f;
        foreach(KeyValuePair<Vector2, Tile> t in tiles)
        {
            t.Value.gameObject.SetActive(true);
            StartCoroutine(tileDance(t.Value, delay));
            delay += .005f;
        }
    }

    IEnumerator tileDance(Tile tile, float delay)
    {
        Vector3 startPos = tile.transform.parent.position;
        Vector3 zeroPos = tile.transform.parent.position;
        zeroPos.y = 0f;

        Vector3 endPos = new Vector3(zeroPos.x, -70, zeroPos.z);

        yield return new WaitForSeconds(delay);
        float t = 0;
        while (t < 1f)
        {
            tile.transform.parent.position = Vector3.Lerp(tile.transform.parent.position, zeroPos, t);
            t += Time.deltaTime;
            yield return null;
        }
        tile.transform.parent.position = zeroPos;

        yield return new WaitForSeconds(1f);

        t = 0;
        while (t < .7f)
        {
            tile.transform.parent.position = Vector3.Lerp(zeroPos, endPos, t/.7f);
            t += Time.deltaTime;
            yield return null;
        }
        tile.transform.parent.position = endPos;
    }
}
