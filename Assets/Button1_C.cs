using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    string event_name = "";  //计时器结束后触发的事件

    void Start()
    {
        now_Player = GameObject.Find("Player1");  //寻找初始角色
    }

    public void change_player(int index)
    {
        now_Player = GameObject.Find($"Player{index}");
        if(now_Player == null) { Debug.Log("Change Player Error!"); }
        else
        {
            GameObject Player_Choose = GameObject.Find("Player_Choose");
            Player_Choose.transform.localPosition = new Vector3(
                now_Player.transform.localPosition.x,
                Player_Choose.transform.localPosition.y,
                Player_Choose.transform.localPosition.z
                );
        }
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
                if (event_name == "next")
                {
                    next_round();
                }             
                event_name = "";
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
        if (now_Player == null) { return; }  //角色死亡，不响应
        avoid_repeat_ability();  //使用该技能后，本回合禁用其他技能
        now_Player.GetComponent<Player_C>().ability1();  //调用当前角色对应技能
        set_timer(1.2f, "next");  //1s后进入下一回合
    }

    public void ability2()
    {
        if (now_Player == null) { return; }
        avoid_repeat_ability();
        now_Player.GetComponent<Player_C>().ability2();
        set_timer(1.2f, "next");
    }

    public void abilityX()
    {
        if (now_Player == null) { return; }
        avoid_repeat_ability();
        now_Player.GetComponent<Player_C>().abilityX();
        set_timer(1.2f, "next");
    }

    void set_timer(float value, string str)
    {
        timer = value;
        isWaiting = true;
        event_name = str;
    }
}
