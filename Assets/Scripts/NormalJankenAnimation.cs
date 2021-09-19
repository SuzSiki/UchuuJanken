using UnityEngine;

public abstract class NormalJankenAnimation : IJankenAnimation
{
    bool _compleate = false;
    public bool compleate
    {
        get { return _compleate; }
        protected set
        {
            if(value)
            {
                if(onCompleate!= null)
                {
                    onCompleate();
                    onCompleate = null;
                }
            }
            _compleate = value;
        }
    }

    public float duration { get; private set; }
    public System.Action onCompleate { get; set;}

    public NormalJankenAnimation()
    {
        compleate = false;
        duration = JankenManager.instance.settingProfile.unitDuration;
    }

    public abstract void SetUp(float timescale);

    public virtual void Play(AudioSource audio)
    {
        compleate = false;
    }
}