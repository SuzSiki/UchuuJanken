using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(Image))]
public class HPBar : MonoBehaviour
{
    static List<HPBar> hPBars = new List<HPBar>();
    Image bar;
    TMP_Text hp; 
    [SerializeField] int id;
    JankenPlayer target;
    [SerializeField] float decreaseDuration = 1f;


    void Start()
    {
        hp = GetComponentInChildren<TMP_Text>();
        hPBars.Add(this);
        bar = GetComponent<Image>();
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.viewInitialize)
            {
                target = JankenManager.instance.nowSession.GetPlayer(id);
                UpdateBar();
            }
        };
    }

    public void UpdateBar()
    {
        var normRemaining = (float)target.hp / JankenManager.instance.settingProfile.defaultHP;
        bar.fillAmount = normRemaining;

        System.Action<float> setTargetValue = (x) => bar.fillAmount = x;
        var tw = DOTween.To(()=>bar.fillAmount,(x) => bar.fillAmount = x , normRemaining, decreaseDuration);
        hp.text = target.hp +"/"+JankenManager.instance.settingProfile.defaultHP;
        tw.Play();
    }

    static public HPBar GetBar(int id)
    {
        return hPBars[id];
    }

    void OnDisable()
    {
        hPBars.Remove(this);
    }
}