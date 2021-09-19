using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMController : MonoBehaviour
{
    [SerializeField] float defaultVolume = 1;
    AudioSource[] source;
    [SerializeField] AudioClip titleTheme;
    [SerializeField] AudioClip[] battleTheme;
    [SerializeField] float crossfadeDuration = 1;
    int lastBattleBGM = 0;

    int nowPlaying = 0;

    public void Start()
    {
        source = GetComponents<AudioSource>();

        TitleManager.instance.onTitleEvent += (x) =>
        {
            if (x == TitleState.onTitleScreen)
            {
                //最初はクロスフェードしない
                source[nowPlaying].clip = titleTheme;
                source[nowPlaying].Play();
            }
        };

        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.inJanken)
            {
                int rand = 0;
                do
                {
                    rand = Random.Range(0, battleTheme.Length);
                }while(rand==lastBattleBGM);
                lastBattleBGM = rand;
                var clip = battleTheme[rand];
                StartCoroutine(HalfCrossfade(clip));
            }
        };
    }


    IEnumerator HalfCrossfade(AudioClip clip)
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);
        var motoVolume = defaultVolume;

        //全体のステップ
        var steps = (int)(crossfadeDuration / Time.deltaTime);

        //Stepを必ず4の倍数になるようにする。
        steps -= steps % 4;

        //clip一つ分のfull
        var fullStep = (float)steps * 3 / 4;
        //重なる範囲
        var overrapSteps = steps*2 / 4;

        var now = source[nowPlaying];
        var after = source[(nowPlaying - 1) * -1];
        nowPlaying = (nowPlaying - 1) * -1;


        after.clip = clip;
        after.volume = 0;
        after.Play();

        for (int i = 0; i < steps; i++)
        {
            var afterStep = i - steps/4;
            if (i < fullStep)
            {
                now.volume = Mathf.Lerp(motoVolume, 0, i / fullStep);
            }

            if (afterStep > 0)
            {
                task.compleate = true;
                after.volume = Mathf.Lerp(0, motoVolume, afterStep / fullStep);
            }

            yield return null;
        }

    }

}
