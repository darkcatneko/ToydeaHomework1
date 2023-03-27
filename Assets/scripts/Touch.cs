using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
public class Touch : MonoBehaviour
{
    [Header("是哪個部位")]
    [SerializeField] public BodyParts ThisPart;
    [Header("部件設定")]
    [SerializeField] public GameObject HandTarget;
    [SerializeField] GameObject TipTarget;
    [SerializeField] public GameObject ThisCon_Parts;
    [SerializeField] public GameObject TwistCenter;
    [Header("是否正在啟用")]
    [SerializeField] private bool CanMove = false;
    [Header("IK系統")]
    [SerializeField] private TwoBoneIKConstraint Data;
    [SerializeField] private MultiAimConstraint TopData;
    [SerializeField] private TwoBoneIKConstraint Data2;
    

    private Vector3 ReturnPlace;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var MousePos = Input.mousePosition;
        var Result = new Vector3();
        MousePos.z = Camera.main.nearClipPlane + 4;
        if (CanMove) //跟隨滑鼠
        {
            Result = Camera.main.ScreenToWorldPoint(MousePos);
            if (ThisPart == BodyParts.LeftHand|| ThisPart == BodyParts.RightHand)
            {
                HandTarget.transform.position = InClone(TwistCenter.transform, Result, -180, 180, 0.6f,0.3f);
                TipTarget.transform.position = OnCricle(TwistCenter.transform, Result, -180, 180, 0.6f);
            }
            else
            {
                HandTarget.transform.position = InCloneFoot(TwistCenter.transform, Result, -180, 0, 0.75f,0.75f);
                TipTarget.transform.position = OnCricle(TwistCenter.transform, Result, -180, 180, 0.95f);
            }
        }
        if (Input.GetMouseButtonUp(0) && CanMove)//放開時判定是否是在岩石上放開的
        {
            CanMove = false;
            RaycastHit hit;                                                                                 
            if (Physics.Raycast(HandTarget.transform.position, new Vector3(0, 0, 1), out hit, 10.0f))
            {
                if (hit.transform.CompareTag("Rock"))
                {
                    HandTarget.transform.position = hit.point - new Vector3(0, 0, 0.005f);
                    //往擊中的點位向上移動一點點，可以使手的放置位置更合理
                    BodyMovementCheck();
                    Step_On_Rock_SE();
                    //BodyMovement(hit.point, ThisCon_Parts);
                }
                else
                {
                    GrabCancel(ThisPart);
                }

                Debug.DrawLine(HandTarget.transform.position, hit.point, Color.green);
            }

        }
        if (Input.GetKeyDown(KeyCode.Space) && CanMove) //生成石頭的作弊code
        {
            CanMove = false;
            RaycastHit hit;
            if (Physics.Raycast(HandTarget.transform.position, new Vector3(0, 0, 1), out hit, 10.0f))
            {
                if (hit.transform.CompareTag("Rock"))
                {
                    HandTarget.transform.position = hit.point - new Vector3(0, 0, 0.005f);
                    BodyMovementCheck();
                    //BodyMovement(hit.point, ThisCon_Parts);

                }
                else
                {
                    var FinalSpot = new Vector3(HandTarget.transform.position.x, HandTarget.transform.position.y, 0.3649997f);
                    GameObject.Find("System").GetComponent<WallGen>().SpawnOneStone(FinalSpot);
                    RaycastHit Hit2;
                    if (Physics.Raycast(HandTarget.transform.position, new Vector3(0, 0, 1), out Hit2, 10.0f))
                    {
                        HandTarget.transform.position = Hit2.point - new Vector3(0, 0, 0.005f);
                        BodyMovementCheck();
                    }
                }

                Debug.DrawLine(HandTarget.transform.position, hit.point, Color.green);
            }
        }
        RaycastHit hit2;
        if (Physics.Raycast(HandTarget.transform.position, new Vector3(0, 0, 1), out hit2, 10.0f))
        {
            Debug.DrawLine(HandTarget.transform.position, hit2.point, Color.green);
        }


    }
    private void OnMouseDown() //抓取瞬間
    {
        if (MainController.instance.NowStatus==GameStatus.Gaming)
        {
            Data.weight = 1;
            if (TopData != null)
            {
                TopData.weight = 1f;
            }
            if (Data2 != null)
            {
                Data2.weight = 0.4f;
            }
            var target = Quaternion.Euler(-90, 0, 0);
            HandTarget.transform.rotation = target;//旋轉target來讓手心面向牆壁
            ReturnPlace = HandTarget.transform.position;
            CanMove = true;
        }        
        
    }
    /// <summary>
    /// 判斷角色是否可以移動，如果可以的話，則移動角色，
    /// 改變重心
    /// </summary>
    private void ChangeGravityPoint()
    {
        var GravityPoint = new Vector3();
        int GrabbingPoint = 0;
        if (MainController.instance.RightHandGrabbing)
        {
            GrabbingPoint++;
            GravityPoint += MainController.instance.RightHandController.ThisCon_Parts.transform.position;
        }
        if (MainController.instance.LeftHandGrabbing)
        {
            GrabbingPoint++;
            GravityPoint += MainController.instance.LeftHandController.ThisCon_Parts.transform.position;
        }
        if (MainController.instance.RightFootGrabbing)
        {
            GrabbingPoint++;
            GravityPoint += MainController.instance.RightFootController.ThisCon_Parts.transform.position;
        }
        if (MainController.instance.LeftFootGrabbing)
        {
            GrabbingPoint++;
            GravityPoint += MainController.instance.LeftFootController.ThisCon_Parts.transform.position;
        }
        GravityPoint = GravityPoint / GrabbingPoint;
        Vector3 Change = GravityPoint - MainController.instance.MainPlayerHip.transform.position;
        Vector3 FinalPos = new Vector3(MainController.instance.MainPlayer.transform.position.x + Change.x, MainController.instance.MainPlayer.transform.position.y + Change.y, MainController.instance.MainPlayer.transform.position.z);
        if (FinalPos.y - ReturnPlace.y>0&&this.ThisCon_Parts.transform.position.y-FinalPos.y>0)
        {
            MainController.instance.MainPlayer.transform.DOMove(FinalPos, 0.2f);
        }        
        MainController.instance.Gravity_Point = FinalPos;
    }
    /// <summary>
    /// 使特定角色部件的權重變為0
    /// </summary>
    /// <param name="BP">身體部件</param>
    public void GrabCancel(BodyParts BP)
    {
        HandTarget.transform.position = ReturnPlace;
        Data.weight = 0;
        if (TopData != null)
        {
            TopData.weight = 0;
        }
        if (Data2 != null)
        {
            Data2.weight = 0;
        }
        switch (BP)
        {
            case BodyParts.LeftLeg:
                MainController.instance.LeftFootGrabbing = false;
                break;
            case BodyParts.RightLeg:
                MainController.instance.RightFootGrabbing = false;
                break;
            case BodyParts.RightHand:
                MainController.instance.RightHandGrabbing = false;
                break;
            case BodyParts.LeftHand:
                MainController.instance.LeftHandGrabbing = false;
                break;
        }
    }
    /// <summary>
    /// 在滑鼠放開那一刻,讓主控制器的bool開關
    /// </summary>
    /// <param name="hit"></param>
    private void BodyMovementCheck()
    {
        var MainPlayer = MainController.instance;
        switch (ThisPart)
        {
            case BodyParts.LeftLeg:
                if (MainPlayer.RightFootGrabbing && !MainPlayer.RightHandController && !MainPlayer.LeftHandGrabbing)
                {
                    GrabCancel(BodyParts.LeftLeg);
                    break;
                }
                MainPlayer.LeftFootGrabbing = true;
                if (MainPlayer.GrabingCount() >= 3)
                {
                    ChangeGravityPoint();
                }
                break;
            case BodyParts.RightLeg:
                if (MainPlayer.LeftFootGrabbing && !MainPlayer.RightHandController && !MainPlayer.LeftHandGrabbing)
                {
                    GrabCancel(BodyParts.RightLeg);
                    break;
                }
                MainPlayer.RightFootGrabbing = true;
                if (MainPlayer.GrabingCount() >= 3)
                {
                    ChangeGravityPoint();
                }
                break;
            case BodyParts.RightHand:
                MainPlayer.RightHandGrabbing = true;
                if (MainPlayer.GrabingCount() >= 3)
                {
                    ChangeGravityPoint();
                }
                break;
            case BodyParts.LeftHand:
                MainPlayer.LeftHandGrabbing = true;
                if (MainPlayer.GrabingCount() >= 3)
                {
                    ChangeGravityPoint();
                }
                break;
        }

    }
    /// <summary>
    /// 限制手部位置,並根據滑鼠位置取得一個最佳的點
    /// </summary>
    /// <param name="HandTarget">手部位置</param>
    /// <param name="MousePoint">滑鼠位置</param>
    /// <param name="MinangleLimit">最小角度</param>
    /// <param name="MaxangleLimit">最大角度</param>
    /// <param name="radius">最長伸長距離</param>
    /// <param name="Minradius">最短伸長距離</param>
    /// <returns></returns>
    public Vector3 InClone(Transform HandTarget, Vector3 MousePoint,float MinangleLimit,float MaxangleLimit, float radius,float Minradius)
    {
        Vector3 direction = MousePoint - HandTarget.position;
        direction.z = 0.0f; // only allow movement in x,y plane
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // clamp angle within limit        
        angle = Mathf.Clamp(angle, MinangleLimit, MaxangleLimit);

        // calculate new position based on clamped angle and distance
        Vector3 newPosition = Quaternion.Euler(0.0f, 0.0f, angle) * Vector3.right * distance;

        // clamp distance within radius
        if (newPosition.magnitude > radius)
        {
            newPosition = newPosition.normalized * radius;
        }
        if (newPosition.magnitude < Minradius)
        {
            newPosition = newPosition.normalized * Minradius;
        }
        // update pointB position
        return  HandTarget.position + newPosition;
    }
    public Vector3 OnCricle(Transform pointA, Vector3 pointB, float MinangleLimit, float MaxangleLimit, float radius)
    {
        Vector3 direction = pointB - pointA.position;
        direction.z = 0.0f; // only allow movement in x,y plane
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // clamp angle within limit        
        angle = Mathf.Clamp(angle, MinangleLimit, MaxangleLimit);

        // calculate new position based on clamped angle and distance
        Vector3 newPosition = Quaternion.Euler(0.0f, 0.0f, angle) * Vector3.right * distance;

       
        newPosition = newPosition.normalized * radius;
       
        // update pointB position
        return pointA.position + newPosition;
    }
    public Vector3 InCloneFoot(Transform pointA, Vector3 pointB, float MinangleLimit, float MaxangleLimit, float radius, float Minradius)
    {
        Vector3 direction = pointB - pointA.position;
        direction.z = 0.0f; // only allow movement in x,y plane
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // clamp angle within limit
        if (angle<180 && angle>90)
        {
            angle = MinangleLimit;
        }
        angle = Mathf.Clamp(angle, MinangleLimit, MaxangleLimit);
        //Debug.Log(angle);
        // calculate new position based on clamped angle and distance
        Vector3 newPosition = Quaternion.Euler(0.0f, 0.0f, angle) * Vector3.right * distance;

        //clamp distance within radius
        if (newPosition.magnitude > radius)
        {
            newPosition = newPosition.normalized * radius;
        }
        if (angle<0)
        {
            if (newPosition.magnitude < (0.25f) / 40f * (-40 - angle) + Minradius - 0.25f && angle > -80 && angle < -40)
            {
                newPosition = newPosition.normalized * ((0.25f) / 40f * (-40 - angle) + Minradius-0.25f);
            }
            else if (newPosition.magnitude < (0.25f) / 40f * (-100 - angle) + Minradius - 0.25f && angle > -140 && angle < -100)
            {
                newPosition = newPosition.normalized * (Minradius-(0.25f) / 40f * (-100 - angle) );
            }
            else
            {
                if (newPosition.magnitude < Minradius && angle > -100 && angle < -80)
                {
                    newPosition = newPosition.normalized * (Minradius);
                }
                else if(newPosition.magnitude < Minradius - 0.25f)
                {
                    newPosition = newPosition.normalized * (Minradius-0.25f);
                }
            }
        }
        else
        {
            if (newPosition.magnitude < (Minradius - 0.25f))
            {
                newPosition = newPosition.normalized * (Minradius - 0.25f);
            }
        }


        // update pointB position
        return pointA.position + newPosition;
    }
    public void Step_On_Rock_SE()
    {
        MainController.instance.BGM_Master.PlayOneShot(MainController.instance.Climbing);
    }
}
public enum BodyParts
{
    RightHand,
    LeftHand,
    RightLeg,
    LeftLeg,
}