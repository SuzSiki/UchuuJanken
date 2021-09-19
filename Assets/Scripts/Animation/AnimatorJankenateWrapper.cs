using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AnimatorJankenateExchanger
{
    protected Animator animator;

    public Animator baseAnimator { get { return animator; } }

    public AnimatorJankenateExchanger(Animator animator) : base()
    {
        this.animator = animator;
        //StartCoroutineを使いたいので誰かしら捕まえる

    }

    //次Playが実行されたときに発行されるTrigger
    public IJankenAnimation GetTriggerAction(string name)
    {
        if(name == "Attack")
        {
            Debug.Log("attack");
        }
        var anime = new VirtualAnime(animator);
        anime.RegisterTrigger(name);
        return anime;
    }


    class VirtualAnime : NormalJankenAnimation
    {
        Animator animator;
        MonoBehaviour virtualBehaviour;
        protected string trigger;
        public VirtualAnime(Animator animator)
        {
            this.animator = animator;
            virtualBehaviour = animator.GetComponent<MonoBehaviour>();
        }



        public void RegisterTrigger(string name)
        {
            trigger = name;
        }


        public override void SetUp(float timescale)
        {
            animator.speed = timescale;
        }

        public override void Play(AudioSource audio)
        {
            base.Play(audio);

            if (!virtualBehaviour.isActiveAndEnabled)
            {
                Debug.LogError("Cursed error");
            }

            virtualBehaviour.StartCoroutine(WaitForCompleation());
        }

        IEnumerator WaitForCompleation()
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            var oldName = info.fullPathHash;
            animator.SetTrigger(trigger);

            //変わるまで待つ
            yield return new WaitUntil(() =>
            {
                info = animator.GetCurrentAnimatorStateInfo(0);
                return oldName  != info.fullPathHash;                
            });

            oldName = info.fullPathHash;
            yield return new WaitWhile(()=>
            {
                info = animator.GetCurrentAnimatorStateInfo(0);
                return info.normalizedTime <= 1 && oldName == info.fullPathHash;
            });


            compleate = true;
        }
    }
}
