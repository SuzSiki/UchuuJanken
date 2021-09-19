using UnityEngine;

public class NullJankenAnime : IJankenAnimation
{
    public bool compleate { get { return true; } }
    public float duration { get { return JankenManager.instance.settingProfile.unitDuration; } }
    public void SetUp(float timescale) { }
    public void Play(AudioSource source)
    {
        if(onCompleate!=null)
        {
            onCompleate();
        }
    }
    public System.Action onCompleate{get;set;}
}