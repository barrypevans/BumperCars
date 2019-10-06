using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sinRot : MonoBehaviour
{
    public float offset;
    public float speed;
    public float width;
    float y;
    float starty;
    Vector3 rotto;

    void Start()
    {
        rotto = transform.localEulerAngles;
        starty = transform.localEulerAngles.y;
    }


    void Update()
    {
        rotto.y = Mathf.Cos(Time.time * speed + offset) * width;
        transform.localEulerAngles = rotto;
    }
}
