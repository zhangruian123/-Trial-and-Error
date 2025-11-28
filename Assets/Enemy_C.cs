using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_C : MonoBehaviour
{
    /*
     * 该脚本为敌人基类
     */

    //血条相关
    protected int max_hp;
    protected int current_hp;
    public GameObject HP;  //游戏中敌人头顶的红色血条
    public GameObject HP_down;  //红色血条的背景，那个灰色的条子
    protected const float HP_SCALE = 0.0005f;   //血条长度系数，0.5/1000，敌人血条长度随血量变化

    //能量相关，当前能量等于最大能量时可以解锁大招
    protected int max_power;
    protected int current_power;
    public GameObject Power;  //游戏中的蓝色能量条

    //攻击相关
    protected int attack;

    //BUFF相关 
    protected const int BUFF_NUMS = 8;  //BUFF总数量，可变化
    protected buff[] BUFF;
    /*
     * index  0     1     2     3     4     5     6     7
     * 属性   x    攻击  减伤   x     x     x     x     x
     * 
     * 攻击：每层提升10%造成的伤害
     * 减伤：每层降低10%受到的伤害
     */

    //本地回合，即怪物第localround次出手
    protected int localround = 1;

    protected virtual void Start()
    {
        Debug.Log("Wrong Iherit!");
    }

    protected virtual void Update()
    {
        
    }


    //-----血量相关接口-----
    protected void init_hp()
    {
        //根据最大血量初始化血条长度
        HP_down.transform.localScale = new Vector3(
            max_hp * HP_SCALE,
            HP.transform.localScale.y,
            HP.transform.localScale.z
            );
        HP.transform.localScale = HP_down.transform.localScale;
    }

    protected void flush_hp()
    {
        //受到伤害或治疗时刷新血量长度
        float dx = (HP.transform.localScale.x - current_hp * HP_SCALE) / 2;
        HP.transform.localPosition = new Vector3(
            HP.transform.localPosition.x - dx,
            HP.transform.localPosition.y,
            HP.transform.localPosition.z
            );
        HP.transform.localScale = new Vector3(
            current_hp * HP_SCALE,
            HP.transform.localScale.y,
            HP.transform.localScale.z
            );
    }

    public void increase_hp(int num)
    {
        if (num < 0) { Debug.Log("Wrong HP Changing!"); return; }
        current_hp = Mathf.Min(current_hp + num, max_hp);
        flush_hp();
    }

    public void decrease_hp(int num)
    {
        if (num < 0) { Debug.Log("Wrong HP Changing!"); return; }
        current_hp = Mathf.Max(current_hp - num, 0);
        if (current_hp == 0) die();
        flush_hp();
    }

    protected void die()
    {
        Destroy(gameObject);
    }

    public int show_hp() { return current_hp; }

    public bool if_max_hp() { return (current_hp == max_hp); }


    //----------能量相关接口----------
    //理论上来说，怪物行动序列由movement控制，能量机制仅为与角色形式统一，实则没用
    //待定


    //----------BUFF相关接口----------
    public void set_buff(int type, int l, int t)
    {
        //对BUFF[type]进行修改，将其等级提升l级，时间设置为t
        //为“重复使用提升BUFF层数，刷新持续时间”的需求设计
        if (type >= BUFF_NUMS || type < 0) { Debug.Log("BUFF Error!"); return; }

        BUFF[type].level = BUFF[type].level + l;
        //不对BUFF层数是否为负，以及上限进行约束，交给释放技能判断

        if (t < 0) { Debug.Log("BUFF Error!"); return; }
        BUFF[(type)].time = t;
        //要求持续时间不能小于0
    }

    public int show_buff_level(int type)
    {
        if (type >= BUFF_NUMS || type < 0) { Debug.Log("BUFF Error!"); return -1; }
        return BUFF[type].level;
    }

    public int show_buff_time(int type)
    {
        if (type >= BUFF_NUMS || type < 0) { Debug.Log("BUFF Error!"); return -1; }
        return BUFF[type].time;
    }

    public void update_buff()
    {
        //回合结束时调用，对持续时间非零的BUFF减1，并对恰好减为0的BUFF重置层数
        for (int i = 0; i < BUFF_NUMS; i++)
        {
            int current_time = BUFF[i].time;
            if (current_time < 0) { Debug.Log("BUFF Time Error!"); return; }
            else if (current_time == 0) { continue; }
            else if (current_time == 1)
            {
                BUFF[i].time = 0;
                BUFF[i].level = 0;
            }
            else
            {
                BUFF[i].time = current_time - 1;
            }
        }
    }


    //-----伤害计算公式-----
    public int calculate_damage_for_attack(float multiplier)
    {
        //传入参数：技能倍率
        //攻击力模型的伤害计算公式:基础攻击力 * 倍率 * （1 + 攻击BUFF层数 * 0.1）

        float res = attack * multiplier * (1 + BUFF[1].level * 0.1f);
        return (int)res;
    }

    public void be_injured(int injury)
    {
        //传入参数：受到伤害
        //实际扣血计算公式：受到伤害 * （1 - 减伤BUFF层数 * 0.1）

        float res = injury * (1 - BUFF[2].level * 0.1f);

        //扣血
        decrease_hp(Mathf.Max((int)res, 0));
    }


    //-----行动相关接口-----
    public virtual void movement()
    {
        Debug.Log("Movement Inherit Error!");
    }


    //-----技能相关接口-----
    public virtual void ability1()
    {
        Debug.Log("Ability Error!");
    }

    public virtual void ability2()
    {
        Debug.Log("Ability Error!");
    }

    public virtual void ability3()
    {
        Debug.Log("Ability Error!");
    }
}
