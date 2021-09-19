using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

//見せる、消す、伝える(int id)
[RequireComponent(typeof(Image))]
public class SelectionButtonPanel : MonoBehaviour, IInteraptor
{
    static List<SelectionButtonPanel> panels = new List<SelectionButtonPanel>();
    [SerializeField] new SelectionPanelName name;
    [SerializeField] bool setInteraptor = true;
    public virtual bool compleate { get; private set; }
    Image panel;
    public DownerButton[] buttons { get { return _buttons; } private set { _buttons = value; } }
    DownerButton[] _buttons;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip choosedSound;
    [SerializeField] AudioClip closeSound;
    [SerializeField] float showDuration;
    [SerializeField, Range(0, 1f)] float showAlpha;
    [SerializeField] bool hideOnChoosed = true;
    [SerializeField] bool executeOnDown = true;
    public event System.Action<int> OnSelect;

    public static SelectionButtonPanel GetPanel(SelectionPanelName name)
    {
        return panels.Find(x => x.name == name);
    }

    protected virtual void Awake()
    {
        panel = GetComponent<Image>();
        buttons = GetComponentsInChildren<DownerButton>();
        panels.Add(this);


        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var num = i;
            if (executeOnDown)
            {
                buttons[i].onPointerDown += () => OnClick(num);
            }
            else
            {
                buttons[i].onClick.AddListener(() => OnClick(num));
            }

            buttons[i].interactable = false;
            buttons[i].image.DOFade(0, 0).Play();
        }
    }

    void OnClick(int id)
    {
        if (OnSelect != null)
        {
            OnSelect(id); //enumには最初にdefaultがあるので+1
        }
        BattleAnimKantoku.audioSource.PlayOneShot(choosedSound);
        if (hideOnChoosed)
        {
            Hide(id);
        }
    }

    [Sirenix.OdinInspector.Button]
    public void Show()
    {
        compleate = false;
        if (setInteraptor)
        {
            JankenManager.instance.RegisterInterapt(this);
        }
        var sq = DOTween.Sequence();

        ShowScreen(ref sq);
        ShowButtons(ref sq);

        sq.onPlay += () =>
        {
            var source = BattleAnimKantoku.audioSource;
            source.PlayOneShot(openSound);
        };

        sq.Play();
    }

    protected virtual Tween ShowScreen(ref Sequence sq)
    {
        var tw = panel.DOFade(showAlpha, showDuration).SetEase(Ease.OutQuint);
        sq.Append(tw);

        return sq;
    }

    protected virtual Tween ShowButtons(ref Sequence sq)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var btw = buttons[i].image.DOFade(showAlpha, showDuration / 16).SetDelay((showDuration * i / 16));
            buttons[i].interactable = true;
            sq.Join(btw);
        }

        return sq;
    }

    public void DisableButton(int id)
    {
        buttons[id - 1].interactable = false;
    }

    public virtual void Hide(int id,float delay = 0)
    {
        var sq = DOTween.Sequence();

        HideScreen(ref sq);
        HideButtons(ref sq, id);

        sq.onPlay += () =>
        {
            BattleAnimKantoku.audioSource.PlayOneShot(closeSound);
        };
        sq.onComplete += () => compleate = true;

        sq.SetDelay(delay);
        sq.Play();
    }

    protected virtual Tween HideScreen(ref Sequence sq)
    {
        var tw = panel.DOFade(0, showDuration ).SetEase(Ease.OutQuint);
        sq.Append(tw);

        return sq;
    }

    protected virtual Tween HideButtons(ref Sequence sq, int id)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            var tw = buttons[i].image.DOFade(0, showDuration / 2);

            sq.Join(tw);
        }

        return sq;
    }



    void OnDisable()
    {
        panels.Remove(this);
    }
}

public enum SelectionPanelName
{
    none,
    HandSelection,
    PlayerSelection
}