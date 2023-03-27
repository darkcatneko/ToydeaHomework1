using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GravityPoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool CanMove = false;
    //[SerializeField] public float MaxRightHandDistance;
    //[SerializeField] public float MaxLeftHandDistance;
    //[SerializeField] public float MaxRightLegDistance;
    //[SerializeField] public float MaxLeftLegDistance;
    [SerializeField] public Limbs RightHand;
    [SerializeField] public Limbs LeftHand;
    [SerializeField] public Limbs RightLeg;
    [SerializeField] public Limbs LeftLeg;
    [SerializeField] float UseableRightHandDistance;
    Vector3 Offset = new Vector3();
    //public Vector3 RightHandOffset = new Vector3();
    //public Vector3 LeftHandOffset = new Vector3();
    //public Vector3 RightLegOffset = new Vector3();
    //public Vector3 LeftLegOffset = new Vector3();
    void Start()
    {
        //RightHand.MaxLimbDistance = (MainController.instance.RightHandController.TwistCenter.transform.position - MainController.instance.RightHandController.ThisCon_Parts.transform.position).magnitude;
        //LeftHand.MaxLimbDistance = (MainController.instance.LeftHandController.TwistCenter.transform.position - MainController.instance.LeftHandController.ThisCon_Parts.transform.position).magnitude;
        //RightLeg.MaxLimbDistance = (MainController.instance.RightFootController.TwistCenter.transform.position - MainController.instance.RightFootController.ThisCon_Parts.transform.position).magnitude;
        //LeftLeg.MaxLimbDistance = (MainController.instance.LeftFootController.TwistCenter.transform.position - MainController.instance.LeftFootController.ThisCon_Parts.transform.position).magnitude;
        RightHand.Set_MaxLimbDistance();
        LeftHand.Set_MaxLimbDistance();
        RightLeg.Set_MaxLimbDistance();
        LeftLeg.Set_MaxLimbDistance();
        this.transform.position = MainController.instance.MainPlayerHip.transform.position;
        Offset = MainController.instance.MainPlayer.transform.position - this.transform.position;
        Offset.z = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = MainController.instance.MainPlayerHip.transform.position;
        Vector3 MousePos = Input.mousePosition;
        Vector3 Result = new Vector3();
        MousePos.z = Camera.main.nearClipPlane + 4;
        if (CanMove && MainController.instance.GrabingCount() >= 3)
        {
            Result = Camera.main.ScreenToWorldPoint(MousePos);
            var InAreaPoint = InArea(MainController.instance.Gravity_Point, Result);
            MainController.instance.MainPlayer.transform.position = InAreaPoint + Offset;
            this.transform.position = InAreaPoint;
        }
        if (Input.GetMouseButtonUp(0) && CanMove)
        {
            CanMove = false;
            //MainController.instance.MainPlayer.transform.position = this.transform.position + Offset;
            this.transform.position = MainController.instance.MainPlayerHip.transform.position;
        }
    }
    private void OnMouseDown()
    {
        Debug.Log("Get");
        CanMove = true;
        MainController.instance.Gravity_Point = MainController.instance.MainPlayerHip.transform.position;
        //RightHandOffset = MainController.instance.RightHandController.ThisCon_Parts.transform.position - MainController.instance.RightHandController.TwistCenter.transform.position;
        //RightLegOffset.z = 0;
        //LeftHandOffset = MainController.instance.LeftHandController.ThisCon_Parts.transform.position - MainController.instance.LeftHandController.TwistCenter.transform.position;
        //LeftHandOffset.z = 0;
        //RightLegOffset = MainController.instance.RightFootController.ThisCon_Parts.transform.position - MainController.instance.RightFootController.TwistCenter.transform.position;
        //RightLegOffset.z = 0;
        //LeftLegOffset = MainController.instance.LeftFootController.ThisCon_Parts.transform.position - MainController.instance.LeftFootController.TwistCenter.transform.position;
        //LeftLegOffset.z = 0;
        RightHand.Set_Limb_Orbit_Offset();
        LeftHand.Set_Limb_Orbit_Offset();
        RightLeg.Set_Limb_Orbit_Offset();
        LeftLeg.Set_Limb_Orbit_Offset();
    }
    public Vector3 InArea(Vector3 OriginPoint, Vector3 MousePoint)
    {
        Vector3 temp = new Vector3();
        //Vector3 new_OrbitR_H = OriginPoint + RightHandOffset;
        //Vector3 new_OrbitL_H = OriginPoint + LeftHandOffset;
        //Vector3 new_OrbitR_L = OriginPoint + RightLegOffset;
        //Vector3 new_OrbitL_L = OriginPoint + LeftLegOffset;
        RightHand.Set_New_Orbit_Point(OriginPoint);
        LeftHand.Set_New_Orbit_Point(OriginPoint);
        RightLeg.Set_New_Orbit_Point(OriginPoint);
        LeftLeg.Set_New_Orbit_Point(OriginPoint);

        temp = ToOrbitPoint(MousePoint,RightHand);
        temp = ToOrbitPoint(temp, LeftHand);
        temp = ToOrbitPoint(temp, RightLeg);
        temp = ToOrbitPoint(temp, LeftLeg);
        temp.z = 0;
        return temp;
    }
    //public Vector3 ToOrbitPoint(Vector3 InputPoint, Vector3 Orbit_Point, float Radius)
    //{
    //    Vector3 direction = InputPoint - Orbit_Point;
    //    direction.z = 0.0f; // only allow movement in x,y plane        
    //    if (direction.magnitude < Radius)
    //    {
    //        return InputPoint;
    //    }
    //    else
    //    {
    //        return direction.normalized * Radius + Orbit_Point;
    //    }
    //}
    public Vector3 ToOrbitPoint(Vector3 InputPoint,Limbs ThisPart)
    {
        Vector3 direction = InputPoint - ThisPart.New_Orbit_Point;
        direction.z = 0.0f; // only allow movement in x,y plane        
        if (direction.magnitude < ThisPart.MaxLimbDistance)
        {
            return InputPoint;
        }
        else
        {
            return direction.normalized * ThisPart.MaxLimbDistance + ThisPart.New_Orbit_Point;
        }
    }
}
[System.Serializable]
public struct Limbs 
{
    public float MaxLimbDistance;
    public Vector3 LimbOffset;
    public Vector3 New_Orbit_Point;
    public Touch Limb_Controller;
    public void Set_MaxLimbDistance()
    {
        MaxLimbDistance =( Limb_Controller.TwistCenter.transform.position - Limb_Controller.ThisCon_Parts.transform.position).magnitude;
    }
    public void Set_Limb_Orbit_Offset()
    {
        LimbOffset = Limb_Controller.ThisCon_Parts.transform.position - Limb_Controller.TwistCenter.transform.position;
        LimbOffset.z = 0;
    }
    public void Set_New_Orbit_Point(Vector3 Origin_Point)
    {
        New_Orbit_Point = Origin_Point + LimbOffset;
    }

}
