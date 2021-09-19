using UnityEngine;
using System.Collections.Generic;
//MonoBehaviourTに対するAnimationの具体的な実装を行い、
//要求に応じてそのレシピを渡す者

public abstract class AnimationConductor:MonoBehaviour
{
    //public abstract Dictionary<AnimationType,IJankenAnimation> animations{get;protected set;}
    public abstract IJankenAnimation GetAnimation(AnimationType type);
    
}