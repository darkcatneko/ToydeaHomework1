using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    Vector3 Cam_Offset;//相機與玩家的固定距離
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
