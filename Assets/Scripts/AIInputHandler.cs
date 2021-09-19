using UnityEngine;
using System.Collections;

public class AIInputHandler : IJankenInputHandler
{
    JankenPlayer master;
    [SerializeField] float thinkingTime = 1.5f;

    public AIInputHandler(JankenPlayer player)
    {
        master = player;
    }

    public void OnHandSelect()
    {
        master.StartCoroutine(ThinkingTime());
    }

    IEnumerator ThinkingTime()
    {
        var rough = Random.Range(-0.5f, 0.5f);
        yield return new WaitForSeconds(thinkingTime + rough);

        var last = master.lastHand;
        var choices = System.Enum.GetValues(typeof(QuantumHand)) as QuantumHand[];
        var choice = Random.Range(1, choices.Length);

        while (choice == (int)last)
        {
            choices = System.Enum.GetValues(typeof(QuantumHand)) as QuantumHand[];
            choice = Random.Range(1, choices.Length);
        }

        master.SetHand(choices[choice]);
    }

}