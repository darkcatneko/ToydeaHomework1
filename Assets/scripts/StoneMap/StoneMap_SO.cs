using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.IO;

[CreateAssetMenu(fileName = "StoneMap", menuName = "StoneMap")]
public class StoneMap_SO : ScriptableObject
{
    public StoneMapList Stones ;
    public void SaveAllStone()
    {
        GameObject[] StonesOBJ = GameObject.FindGameObjectsWithTag("Rock");
        for (int i = 0; i < StonesOBJ.Length; i++)
        {
            Stones.MYStones.Add(StonesOBJ[i].transform.position);
        }
        Save();
    }
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + "/Save.ept", FileMode.Create);
        string Json = JsonUtility.ToJson(Stones, true);
        bf.Serialize(stream, Json);
        stream.Close();
        Debug.Log("Save Complete" + Application.dataPath + "/Save.ept");
    }
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/Save.ept"))
        {
            StoneMapList temp = new StoneMapList();
            Stones.MYStones = new List<Vector3>();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/Save.ept", FileMode.Open);
            string save_string = bf.Deserialize(stream).ToString();
            JsonUtility.FromJsonOverwrite(save_string, temp);
            for (int i = 0; i < temp.MYStones.Count; i++)
            {
                Stones.MYStones.Add(temp.MYStones[i]);
            };
            stream.Close();
            Debug.Log("Load");

        }
    }
}
[System.Serializable]
public class StoneMapList
{
    public List<Vector3> MYStones = new List<Vector3>();
}