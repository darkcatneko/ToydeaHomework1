using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    [SerializeField] GameObject Mouse_Aim_Traget;
    private void Update()
    {

        Vector3 MousePos = Input.mousePosition;      
        MousePos.z = Camera.main.nearClipPlane + 4;
        Vector3 Result = new Vector3();
        Result = Camera.main.ScreenToWorldPoint(MousePos);
        Mouse_Aim_Traget.transform.position = Result;
    }
}
