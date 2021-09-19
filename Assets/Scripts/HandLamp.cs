using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HandLamp : MonoBehaviour
{
    Image[] lamps;
    [SerializeField, Range(0, 1)] int targetPlayer;
    JankenPlayer player;

    void Start()
    {
        lamps = GetComponentsInChildren<Image>();
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.viewInitialize)
            {
                player = JankenManager.instance.nowSession.GetPlayer(targetPlayer);
            }
        };

        JankenManager.instance.OnJankenEvent += (x) =>
        {
            if(x == JankenState.TurnStart)
            {
                OnStartTurn();
            }
        };
    }

    void OnStartTurn()
    {
        var last = (int)player.lastHand - 1;
        for (int i = 0; i < lamps.Length; i++)
        {
            if (last == i)
            {
                lamps[last].DOFade(0, 0).Play();
            }
            else
            {
                lamps[i].DOFade(1, 0).Play();
            }
        }
    }
}
