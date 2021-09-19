using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
[RequireComponent(typeof(Button))]
public class ConfarmButton : MonoBehaviour
{
    Button _button;
    public Button button { get { return _button; } private set { _button = value; } }
    [SerializeField] float fadeDuration = 0.7f;
    [SerializeField] AudioClip clip;
    static InteraptTask task = new InteraptTask();
    [SerializeField] KeyCode[] listenKeys = new KeyCode[] { KeyCode.Return, KeyCode.Space };

    void Awake()
    {
        button = GetComponent<Button>();
        task.compleate = true;
    }

    void Start()
    {
        button.onClick.AddListener(OnPushed);
        button.enabled = false;
        button.image.DOFade(0, 0).Play();
    }
    public void PressButtonToContinue()
    {
        task.compleate = false;
        JankenManager.instance.RegisterInterapt(task);
        ShowButton();
    }

    void Update()
    {
        if (!task.compleate)
        {
            foreach (var key in listenKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    button.onClick.Invoke();
                    OnPushed();
                }
            }
        }
    }

    public void OnPushed()
    {
        task.compleate = true;
        var source = BattleAnimKantoku.audioSource;
        source.PlayOneShot(clip);
        HideButton();
    }

    public void ShowButton()
    {
        button.enabled = true;
        button.image.DOFade(1, fadeDuration).Play();
    }

    public void HideButton()
    {
        button.enabled = false;
        button.image.DOFade(0, fadeDuration).Play();
    }
}