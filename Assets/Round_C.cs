using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Round_C : MonoBehaviour
{
    /*
     * 该脚本控制与回合相关事件，包括回合数，回合文本，怪物自动出招，按钮状态控制等
     */

    private int round = 1;  //回合数
    private int last_player = 1;  //上次出手角色
    private Button[] allButtons;  //所有按钮 
    public GameObject Canvas;

    float timer = 0f;  
    bool isWaiting = false;  //计时器
    string event_name = "";  //计时器结束后触发的事件

    public void next_round()
    {
        //show_hp_for_debug();
        round = round + 1;
        Canvas.GetComponent<RoundText_C>().change_round_text(round);  //改变回合文本
        
        if (round % 2 == 1)
        {
            //我方回合时，切换下一个玩家
            Canvas.GetComponent<Button1_C>().change_player(next_player());  
        }

        button_control();  //控制按钮状态，敌方回合时禁用按钮，我方回合时启用

        //敌方回合时，自动控制敌人出招
        if (round % 2 == 0)
        {
            GameObject Enemy = GameObject.Find("Enemy1");
            if (Enemy == null) { event_name = ""; return; }  //若敌人已死则直接结束
            Enemy.GetComponent<Enemy_C>().movement();  //否则，调用敌人技能
            set_timer(1.2f, "next");  //1s后下一回合
        }
    }

    public int show_round() { return round; }

    public int next_player()
    {
        for (int i = 0; i < 3; i++)
        {
            int next_index = last_player + 1;
            if(next_index > 3) next_index -= 3;
            last_player = next_index;

            GameObject next_player = GameObject.Find($"Player{next_index}");
            if (next_player != null )
            {
                return next_index;
            }
        }
        Debug.Log("Can't Find Player!");
        return -1;
    }

    void set_timer(float value, string str)
    {
        timer = value;
        isWaiting = true;
        event_name = str;
    }

    void Start()
    {
        allButtons = GetComponentsInChildren<Button>(true);  //初始化，获取所有按钮
    }

    void Update()
    {
        //用于实现计时器
        if (isWaiting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
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

    void button_control()
    {
        bool whose_round = (round % 2 == 1);  //判断谁的回合

        bool if_max_power = false;  //判断是否满能量
        GameObject now_player = Canvas.GetComponent<Button1_C>().now_Player;
        if (now_player != null)
            if_max_power = now_player.GetComponent<Player_C>().if_max_power();
        
        foreach (Button button in allButtons)
        {
            if (button != null)
            {
                if (button.name == "ButtonX")  //对于大招，同时满足两个条件
                    button.interactable = whose_round && if_max_power;
                else  //对于其他按钮，满足己方回合
                    button.interactable = whose_round;
            }
        }
    }

    public void button_forbid()
    {
        //强制禁用全局按钮
        foreach (Button button in allButtons)
        {
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }

    public void show_hp_for_debug()
    {
        
        Debug.Log($"after round{round}:");
        for (int i = 1; i <= 3; i++)
        {
            GameObject target = GameObject.Find($"Player{i}");
            if (target != null)
            {
                int target_hp = target.GetComponent<Player_C>().show_hp();
                Debug.Log($"the hp of Player{i} is {target_hp}");
            }
        }
        GameObject enemy = GameObject.Find("Enemy1");
        if (enemy != null)
        {
            int target_hp = enemy.GetComponent<Enemy_C>().show_hp();
            Debug.Log($"the hp of Enemy is {target_hp}");
        }
    }
}
