using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public bool togCred;
    GameObject cam;

    [SerializeField]
    private float RotationSpeed;
    [SerializeField]
    private float RotationWidth;

    public float creditsLerp;
    Vector3 camSweep;
    public Quaternion camStartRot;
    public Quaternion camRot;

    Vector3 rotto;

    void Start()
    {
        cam = transform.GetChild(0).gameObject;
        camStartRot = cam.transform.localRotation;
    }


    void Update()
    {
        

        camSweep = new Vector3(0, Mathf.Sin(RotationSpeed*Time.time)*RotationWidth, 0);

        transform.eulerAngles = Vector3.Lerp(camSweep, Vector3.zero, creditsLerp);

        cam.transform.localRotation = Quaternion.Lerp(camStartRot, camRot, Mathf.SmoothStep(0, 1, creditsLerp));


        if (togCred)
        {
            if (creditsLerp < 1)
                creditsLerp += Time.deltaTime;
        }
        else
        {
            if (creditsLerp > 0)
                creditsLerp -= Time.deltaTime;
        }
    }

    public void toggleCredits()
    {
        if (!togCred)
        {
            togCred = true;
        }
        else
        {
            togCred = false;
        }
    }
}
