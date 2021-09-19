using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class  PlayerData
{
    [SerializeField] public string name;
    [SerializeField] public int rating = 1000;
    [SerializeField] public int ID;
    [SerializeField] public int maxWin = 0;
    [SerializeField] public List<PickData> pickCount = new List<PickData>();
}

[System.Serializable]
public class PickData
{
    [SerializeField]public int id;
    [SerializeField] public  int count;
}


