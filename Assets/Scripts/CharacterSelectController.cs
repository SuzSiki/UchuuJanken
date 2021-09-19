using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class CharacterSelectController : MonoBehaviour, IInteraptor
{
    public bool compleate { get; private set; }
    ThreadPanel[] threads;
    [SerializeField] SelectionButtonPanel characterButtonPanel;
    [SerializeField] CharacterDisplayField field;
    SessionProfile profile { get { return TitleManager.instance.profile; } }
    ConfarmButton button;

    void Start()
    {
        threads = GetComponentsInChildren<ThreadPanel>();

        button = GetComponentInChildren<ConfarmButton>();
        TitleManager.instance.onTitleEvent += (x) =>
        {
            if (x == TitleState.onCharacterSelect)
            {
                Initialize();
            }
        };

        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if (x == SystemState.inJanken)
            {

            }
        };
    }

    void Initialize()
    {
        button.PressButtonToContinue();
        characterButtonPanel.Show();

        profile.mode = GameMode.pvc;

        characterButtonPanel.OnSelect += (x) =>
        {
            OnCharacterSelect(x);
        };

        var count = 0;
        foreach (var button in characterButtonPanel.buttons)
        {
            button.targetGraphic.color = JankenManager.instance.settingProfile.handColors[count];
            count++;
        }

        foreach (var thread in threads)
        {
            thread.Show();
        }

        OnCharacterSelect(0);
    }

    void OnCharacterSelect(int id)
    {
        var player = PlayerSetter.instance.players[id];
        profile.player0 = id;
        field.LoadPlayer(player);
    }
}