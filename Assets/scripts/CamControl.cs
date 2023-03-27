using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    Vector3 Cam_Offset;//�۾��P���a���T�w�Z��
    void Start()
    {
        Cam_Offset = MainController.instance.MainPlayer.transform.position - Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.position = MainController.instance.MainPlayer.transform.position - Cam_Offset;
    }
}
