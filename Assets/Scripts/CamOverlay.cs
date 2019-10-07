using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamOverlay : MonoBehaviour
{
    public static CamOverlay instance;

    void Start()
    {
        instance = this;
        Transition(true);
    }

    public void Transition(bool toClear)
    {
        StartCoroutine(Trans(toClear));
    }
    Color realClear = new Color(1, 1, 1, 0);
    IEnumerator Trans(bool toClear)
    {
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        float t = 0;
        while (t < 2f)
        {
            s.color = Color.Lerp(toClear?Color.white:realClear,toClear?realClear:Color.white,t/2f);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
