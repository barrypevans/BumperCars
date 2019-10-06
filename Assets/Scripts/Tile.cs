using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Coroutine colorLerp;
    int pastPaints = 0;
    public void Destroy()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
    public void LerpColor(Color c)
    {
        pastPaints++;
        if (colorLerp != null)
            StopCoroutine(colorLerp);

        colorLerp = StartCoroutine(ColorChange(c));
    }
    private IEnumerator ColorChange(Color c)
    {
        MeshRenderer m = GetComponent<MeshRenderer>();
        Color startValue = m.material.GetColor("_BaseColor");

        /*
        //falloff after reuse
        if (pastPaints >= 4)
            c.a = .1f;
        else
            c.a = (4 - pastPaints) / 4f;
            */
        Color endValue = Color.Lerp(startValue,c,c.a);
        endValue.a = 1;

        float t = 0;
        while (t < 1f)
        {
            m.material.SetColor("_BaseColor", Color.Lerp(startValue,endValue, t));
            t += Time.deltaTime;
            yield return null;
        }

    }
}
