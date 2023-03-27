using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Gizmos))]
public class CustomClassEditor : Editor
{
    private Gizmos c;

    public void OnSceneGUI()
    {
        c = GameObject.Find("GPB").GetComponent<Gizmos>();
        Handles.color = Color.red;
        Handles.DrawWireDisc((c.MC.RightHandController.ThisCon_Parts.transform.position)// position
                                      , Vector3.forward                      // normal
                                      , c.GP.RightHand.MaxLimbDistance);
        Handles.DrawWireDisc((c.MC.LeftHandController.ThisCon_Parts.transform.position)// position
                                      , Vector3.forward                      // normal
                                      , c.GP.LeftHand.MaxLimbDistance);                              // radius
        Handles.DrawWireDisc((c.MC.RightFootController.ThisCon_Parts.transform.position)// position
                                      , Vector3.forward                      // normal
                                      , c.GP.RightLeg.MaxLimbDistance);                              // radius
        Handles.DrawWireDisc((c.MC.LeftFootController.ThisCon_Parts.transform.position)// position
                                      , Vector3.forward                      // normal
                                      , c.GP.LeftLeg.MaxLimbDistance);                              // radius// radius
        Handles.color = Color.blue;
        Handles.DrawWireDisc(c.MainPlayerHip.transform.position, Vector3.forward, 0.15f);
    }
}
