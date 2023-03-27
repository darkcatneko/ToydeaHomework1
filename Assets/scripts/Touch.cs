using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
public class Touch : MonoBehaviour
{
    [Header("�O���ӳ���")]
    [SerializeField] public BodyParts ThisPart;
    [Header("����]�w")]
    [SerializeField] public GameObject HandTarget;
    [SerializeField] GameObject TipTarget;
    [SerializeField] public GameObject ThisCon_Parts;
    [SerializeField] public GameObject TwistCenter;
    [Header("�O�_���b�ҥ�")]
    [SerializeField] private bool CanMove = false;
    [Header("IK�t��")]
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
        if (CanMove) //���H�ƹ�
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
        if (Input.GetMouseButtonUp(0) && CanMove)//��}�ɧP�w�O�_�O�b���ۤW��}��
        {
            CanMove = false;
            RaycastHit hit;                                                                                 
            if (Physics.Raycast(HandTarget.transform.position, new Vector3(0, 0, 1), out hit, 10.0f))
            {
                if (hit.transform.CompareTag("Rock"))
                {
                    HandTarget.transform.position = hit.point - new Vector3(0, 0, 0.005f);
                    //���������I��V�W���ʤ@�I�I�A�i�H�Ϥ⪺��m��m��X�z
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
        if (Input.GetKeyDown(KeyCode.Space) && CanMove) //�ͦ����Y���@��code
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
    private void OnMouseDown() //�������
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
            HandTarget.transform.rotation = target;//����target������߭��V���
            ReturnPlace = HandTarget.transform.position;
            CanMove = true;
        }        
        
    }
    /// <summary>
    /// �P�_����O�_�i�H���ʡA�p�G�i�H���ܡA�h���ʨ���A
    /// ���ܭ���
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
    /// �ϯS�w���ⳡ���v���ܬ�0
    /// </summary>
    /// <param name="BP">���鳡��</param>
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
    /// �b�ƹ���}���@��,���D�����bool�}��
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
    /// ����ⳡ��m,�îھڷƹ���m���o�@�ӳ̨Ϊ��I
    /// </summary>
    /// <param name="HandTarget">�ⳡ��m</param>
    /// <param name="MousePoint">�ƹ���m</param>
    /// <param name="MinangleLimit">�̤p����</param>
    /// <param name="MaxangleLimit">�̤j����</param>
    /// <param name="radius">�̪������Z��</param>
    /// <param name="Minradius">�̵u�����Z��</param>
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