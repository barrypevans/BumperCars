using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimOffset : MonoBehaviour
{
    public float offset;
    Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        anim["CarSelectSpin"].time = offset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
