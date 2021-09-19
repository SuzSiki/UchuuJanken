using UnityEngine;
using System.Collections.Generic;

public class PlayerSetter : Singleton<PlayerSetter>
{
    public JankenPlayer[] players{get{return _players;}}
    [SerializeField,Sirenix.OdinInspector.InlineEditor] JankenPlayer[] _players;
    [SerializeField] Transform[] playerPlaces;
    List<IJankenInputHandler> inputHandlers = new List<IJankenInputHandler>();
    bool waitingForPlayer = false;

    void Start()
    {
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.playerRegister)
            {
                inputHandlers.Clear();
                waitingForPlayer = true;
            }
        };

        JankenManager.instance.OnJankenEvent += (x) =>
        {
            if (x == JankenState.ChooseHandStart)
            {
                foreach (var handler in inputHandlers)
                {
                    handler.OnHandSelect();
                }

            }
        };
    }


    public void SetAIPlayer(int id, int choice)
    {
        if (waitingForPlayer)
        {
            if (choice == -1)
            {
                choice = RandomChoose();
            }

            var instance = Instantiate(players[choice]);
            var handler = new AIInputHandler(instance);
            inputHandlers.Add(handler);
            SetPlayer(id, instance);
        }
    }

    public void SetHumanPlayer(int id, int choice)
    {
        if (waitingForPlayer)
        {
            if(choice == -1)
            {
                choice = RandomChoose();
            }

            var instance = Instantiate(players[choice]);
            var handler = new PlayerInputHandler(instance);
            inputHandlers.Add(handler);
            SetPlayer(id, instance);
        }
    }

    int RandomChoose()
    {
        var gundom = Random.Range(0, players.Length + 1) - 1;
        if (gundom < 0)
        {
            gundom = 0;
        }

        return gundom;
    }

    void SetPlayer(int id, JankenPlayer instance)
    {
        if (inputHandlers.Count == 2)
        {
            waitingForPlayer = false;
        }

        JankenManager.instance.RegisterPlayer(instance);
        instance.transform.SetParent(playerPlaces[id], false);
    }
}

public enum GameMode
{
    none,
    pvc,
    cvc,
    network
}