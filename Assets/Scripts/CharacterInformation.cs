using UnityEngine;

[System.Serializable]
public class CharacterInformation
{
    [SerializeField] string _name;
    [SerializeField,Multiline] string _introduction;
    [SerializeField,Multiline] string _positiveEntangle;
    [SerializeField,Multiline] string _negativeEntangle;
    [SerializeField] int charaID;

    public string name{get{return _name;}}
    public string introduction{get{return _introduction;}}
    public string positiveEntangle{get{return _positiveEntangle;}}
    public string negativeEntangle{get{return _negativeEntangle;}}
    public int ID{get{return charaID;}}
}

public enum CharacterType
{
    none,
    Chiri,
    Nemi
}