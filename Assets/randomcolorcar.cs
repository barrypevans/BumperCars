using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomcolorcar : MonoBehaviour
{
    CarController cc;
    public CarPalletteSO[] palets;

    void Start()
    {
        cc = GetComponent<CarController>();
        cc.Pallette = palets[Random.Range(0, palets.Length)];
    }


    void Update()
    {
        
    }
}
