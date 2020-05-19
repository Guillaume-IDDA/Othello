using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera2Dto3D : MonoBehaviour
{

    [SerializeField] GameObject camera3D;
    [SerializeField] GameObject camera2D;

    public enum STATECAM { ON2D, ON3D };

    STATECAM actualCam;
    // Update is called once per frame

    void Start()
    {
        actualCam = STATECAM.ON3D;
    }

    public void SwitchCamera()
    {
        switch (actualCam)
        {
            case STATECAM.ON2D:
                camera3D.SetActive(true);
                camera2D.SetActive(false);
                actualCam = STATECAM.ON3D;
                break;
            case STATECAM.ON3D:
                camera3D.SetActive(false);
                camera2D.SetActive(true);
                actualCam = STATECAM.ON2D;
                break;
            default:
                break;
        }
    }

}
