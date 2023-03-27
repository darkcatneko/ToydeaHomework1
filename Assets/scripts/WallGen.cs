using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGen : MonoBehaviour
{
    [SerializeField] Transform Start_Point;
    [SerializeField] Transform End_Point;
    [SerializeField]public GameObject Stone_Prefab;
    [SerializeField] Transform Wall_Parent;
    [SerializeField] float Y_Offset;
    [SerializeField] StoneMap_SO StoneMap;
    float X_Change;
    float Y_Now = 0; float Y_Change;
    void Start()
    {
        //X_Change = End_Point.position.x - Start_Point.position.x;
        //Y_Change = End_Point.position.y - Start_Point.position.y;
        //while (Y_Now < Y_Change)
        //{
        //    Gen_Stone();
        //}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool Gen_Stone()
    {
        var Final_Spot = new Vector3();
        var x = 0f; 
        var y = 0f; 
        y= Random.Range(0.2f, Y_Offset);
        if (Y_Now+y>=Y_Change)
        {
            Y_Now = Y_Change;
            var time = Random.Range(1, 3);
            for (int i = 0; i < time; i++)
            {               
                x = Random.Range(0f, X_Change);
                Final_Spot = new Vector3(x,Y_Change, 0) + Start_Point.position;
                Instantiate<GameObject>(Stone_Prefab, Final_Spot, Quaternion.identity, Wall_Parent);                
            }
            return true;
        }
        else
        {
            Y_Now = Y_Now+y;
            var time = Random.Range(1, 3);
            for (int Y = 0; Y < time; Y++)
            {
                x = Random.Range(0f, X_Change);
                Final_Spot = new Vector3(x, Y_Now, 0) + Start_Point.position;
                Instantiate<GameObject>(Stone_Prefab, Final_Spot, Quaternion.identity, Wall_Parent);                
            }
            return false;
        }
    }
    public void SpawnStones()
    {
        StoneMap.Load();
        for (int i = 0; i < StoneMap.Stones.MYStones.Count; i++)
        {
            Instantiate<GameObject>(Stone_Prefab, StoneMap.Stones.MYStones[i], Quaternion.identity, Wall_Parent);
        }
    }
    public void SpawnOneStone(Vector3 Position)
    {
        Instantiate<GameObject>(Stone_Prefab, Position, Quaternion.identity, Wall_Parent);
    }
}
