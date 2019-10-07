using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    Coroutine colorLerp;
    int pastPaints = 0;
    public void Destroy()
    {
        StartCoroutine(Collapse());
    }
    private IEnumerator Collapse()
    {
        Vector3 startPos = transform.position;
        Vector3 crumblePos = transform.position;
        crumblePos.y -= .3f;
        Vector3 fallPos = crumblePos;
        fallPos.y -= 5f;

        AudioManager.instance.CreateOneShot("FallingDebris2",.5f);

        float t = 0;
        while (t < .5f)
        {
            transform.position = Vector3.Lerp(startPos,crumblePos,t/.5f);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(crumblePos, fallPos, t);
            t += Time.deltaTime;
            yield return null;
        }
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
