using UnityEngine;
using DG.Tweening;

public class CharacterSelectionButtonScreen : SelectionButtonPanel
{
    Camera mainCamera;
    public override bool compleate{get{return true;}}
    
    protected void Start()
    {
        mainCamera = Camera.main;
        mainCamera.depth = -2;
        var sq = DOTween.Sequence();

        //最初は真っ暗にしておく
        base.ShowScreen(ref sq).Play();
        

        JankenManager.instance.OnSystemEvent += (x) =>
         {
             if (x == SystemState.inJanken)
             {
                 ShowJanken();
             }
         };
    }

    protected override Tween ShowScreen(ref Sequence sq)
    {
        //蓋を消す
        var tw = base.HideScreen(ref sq);
        return tw;
    }

    protected override Tween HideScreen(ref Sequence sq)
    {
        //画面を一度真っ暗にする
        var tw = base.ShowScreen(ref sq);

        return tw;
    }

    void ShowJanken()
    {
        var task = new InteraptTask();
        JankenManager.instance.RegisterInterapt(task);
        mainCamera.depth = 0;
        var sq = DOTween.Sequence();
        sq.onComplete += () => task.compleate = true;
        base.HideScreen(ref sq).Play();
    }
}