using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IJankenAnimation
{
    bool compleate{get;}
    float duration{get;}
    void SetUp(float timescale);
    void Play(AudioSource source);
    Action onCompleate{get;set;}
}
