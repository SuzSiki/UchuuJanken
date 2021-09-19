using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections;

//名前でSpriteを見つける原初のServer
public class SpriteServer:Singleton<SpriteServer>
{
    Sprite[] sprites;
    
    void Start()
    {
        JankenManager.instance.OnSystemEvent += (x) =>
        {
            if(x == SystemState.systemInitialize)
            {
                StartCoroutine((Initialize()));
            }
        };
    }

    IEnumerator Initialize()
    {
        var task = new InteraptTask();
        var spriteTask = LoadSprites();
        yield return new WaitUntil(()=>spriteTask.IsCompleted);
        task.compleate = true;
    }

    async Task LoadSprites()
    {
        sprites = await new LoadAllAsync<Sprite>("Sprite/");
    }
}