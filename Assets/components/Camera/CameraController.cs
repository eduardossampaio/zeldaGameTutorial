using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera")]
    [SerializeField]private GameObject vcam2;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "CamTrigger":
                vcam2.SetActive(true);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "CamTrigger":
                vcam2.SetActive(false);
                break;
        }
    }
}
