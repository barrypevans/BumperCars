using System.Collections;
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

    Vector2 lastCollision = new Vector2(-1,-1);
    public void TakeOutTile(Vector3 position,Color car1, Color car2)
    {
        RaycastHit[] hits = Physics.RaycastAll(position+ Vector3.up*2, Vector3.down);
        var tileHits = hits.Where(hit => hit.collider.tag == "GroundTile");

        if (tileHits.Count() > 0)
        {
            Tile pingTile = tileHits.FirstOrDefault().collider.GetComponent<Tile>();
            var index = tiles.Where(tile => tile.Value == pingTile).FirstOrDefault();


            //Prevent duplicate collision reads, prevent invalid tile reads
            if (matteColoring && lastCollision!=index.Key && tiles.ContainsKey(index.Key) && tiles[index.Key] != null)
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
            tiles.Remove(index.Key);
        }
    }

    private IEnumerator DelayedTileColoring(KeyValuePair<Vector2, Tile> index, Color mix)
    {

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

}
