using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    Animation menuPlayAnim;

    void Start()
    {
        menuPlayAnim = GetComponent<Animation>();
    }


    void Update()
    {
        
    }

    public void startGame()
    {
        menuPlayAnim.Play();
    }
}
