using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Pallette", menuName = "ScriptableObjects/CarPallette", order = 1)]
public class CarPalletteSO : ScriptableObject
{
    public string CarName;
    public Color Body;
    public Color Seat;
    public Color Window;
    public Color Antenna;
    public Color Metal;
    public Color Lights;
    public Color Tires;
    public Color ExtraDetail1;
    public Color ExtraDetail2;
}
