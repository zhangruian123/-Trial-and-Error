using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button1_C : MonoBehaviour
{
    /*
     * 该脚本控制按钮事件，按下按钮时寻找当前角色，调用其对应技能
     */ 

    public GameObject now_Player;  //当前角色
    public GameObject Canvas;

    float timer = 0f;
    bool isWaiting = false;  //计时器

    void Start()
    {
        now_Player = GameObject.Find("Player");  //寻找角色，目前只有一个角色
    }

    void Update()
    {
        //计时器
        if (isWaiting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f) {
                timer = 0f;
                isWaiting = false;
                next_round();
            }
        }
    }

    public void next_round()
    {
        //调用Round_C中对应函数
        Canvas.GetComponent<Round_C>().next_round();
    }

    void avoid_repeat_ability()
    {
        //强制禁用全局按钮，即按下一个按钮后其他按钮被禁用，一回合只出手一次
        Canvas.GetComponent<Round_C>().button_forbid();
    }

    public void ability1()
    {
        if(now_Player == null) { return; }  //角色死亡，不响应
        avoid_repeat_ability();  //使用该技能后，本回合禁用其他技能
        now_Player.GetComponent<Player_C>().ability1();  //调用当前角色对应技能
        timer = 1.2f;
        isWaiting = true;  //1.2s后进入下一回合
    }

    public void ability2()
    {
        if (now_Player == null) { return; }
        avoid_repeat_ability();
        now_Player.GetComponent<Player_C>().ability2();
        timer = 1.2f;
        isWaiting = true;
    }

    public void abilityX()
    {
        if (now_Player == null) { return; }
        avoid_repeat_ability();
        now_Player.GetComponent<Player_C>().abilityX();
        timer = 1.2f;
        isWaiting = true;
    }
}
