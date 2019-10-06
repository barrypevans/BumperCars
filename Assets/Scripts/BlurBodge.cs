using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurBodge : MonoBehaviour
{

    bool move;
    Vector3 startpos;
    public GameObject endposGO;
    Vector3 endpos;
    public float speed;
    public float offset;
    float lerpPos;
    void Start()
    {

        
        startpos = transform.position;
    }


    void Update()
    {
        lerpPos = Mathf.Sin(offset + (Time.time * speed)) * 0.5f + 0.5f;
        transform.position = Vector3.Lerp(startpos, endposGO.transform.position, lerpPos);

        /*
        transform.position += new Vector3(Time.deltaTime*2, 0, 0);

        
        if(move)
        {
            transform.position = endposGO.transform.position;
            move = false;
        }
        else
        {
            transform.position = startpos;
            move = true;
        }*/
        
    }
}
