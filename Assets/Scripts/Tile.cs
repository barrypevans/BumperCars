using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Coroutine colorLerp;
    int pastPaints = 0;
    public void Destroy()
    {
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
        Color startValue = new Color(m.material.color.r, m.material.color.g, m.material.color.b);
        Color endValue = (startValue + c)/2f;

        /*
        //falloff
        if (pastPaints >= 4)
            c.a = .1f;
        else
            c.a = (4- pastPaints)/4f;
            */
        float t = 0;
        while (t < 2f)
        {
            m.material.SetColor("_BaseColor", Color.Lerp(startValue,endValue, t/2f));
            t += Time.deltaTime;
            yield return null;
        }

    }
}
