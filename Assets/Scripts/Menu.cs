using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    Animation menuPlayAnim;
    public Animation camAnim;

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
        StartCoroutine(CamUp());
    }

    IEnumerator CamUp()
    {
        yield return new WaitForSeconds(1);
        camAnim.Play();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
