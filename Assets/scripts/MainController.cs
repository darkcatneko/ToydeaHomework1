using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController instance;
    [SerializeField] public GameObject MainPlayer;
    [SerializeField] public GameObject MainPlayerHip;
    [SerializeField] public Touch RightHandController;
    [SerializeField] public Touch LeftHandController;
    [SerializeField] public Touch RightFootController;
    [SerializeField] public Touch LeftFootController;
    [SerializeField] public bool RightHandGrabbing;
    [SerializeField] public bool LeftHandGrabbing;
    [SerializeField] public bool RightFootGrabbing;
    [SerializeField] public bool LeftFootGrabbing;
    [SerializeField] public Vector3 Gravity_Point;
    [SerializeField] public StoneMap_SO StoneMap;
    [SerializeField] public GameStatus NowStatus = GameStatus.Waiting;
    [SerializeField] int Count;
    [SerializeField] private float MaxTime;
    [SerializeField] public float NowTime;
    [SerializeField] public AudioClip Climbing;
    [SerializeField] public AudioSource BGM_Master;
    // [SerializeField] float Used_Gravity_Offset;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        NowTime = MaxTime;
        Gravity_Point = MainPlayerHip.transform.position;
    }
    public int GrabingCount()
    {
        int count = 0;
        if (RightFootGrabbing)
        {
            count++;
        }
        if (LeftHandGrabbing)
        {
            count++;
        }
        if (RightHandGrabbing)
        {
            count++;
        }
        if (LeftFootGrabbing)
        {
            count++;
        }
        return count;
    }
    public void StoneMapSave()
    {
        StoneMap.Stones.MYStones = new List<Vector3>();
        StoneMap.SaveAllStone();
    }    
    private void Update()
    {
        Count = GrabingCount();       
        if (NowStatus == GameStatus.Gaming)
        {
            if (NowTime<=0)
            {
                NowStatus = GameStatus.Lose;
            }
            if (MainPlayer.transform.position.y>0.2f)
            {
                if (Count<=3)
                {
                    NowTime -= 2 * Time.deltaTime;
                }
                if (Count==4)
                {
                    NowTime -= 1 * Time.deltaTime;
                }
            }
        }
    }
    public float GetFillPercentage()
    {
        return NowTime / MaxTime;
    }
}
public enum GameStatus
{
    Win,
    Gaming,
    Lose,
    Waiting,
}
