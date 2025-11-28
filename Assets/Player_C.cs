using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public struct buff
{
    public int level;
    public int time;
}

public class Player_C : MonoBehaviour
{
    /*
     * 该脚本为角色基类，包含基础数值和通用接口
     * 子类需要完成start,update等函数和ability函数的实例化
     */

    //血条相关
    protected int max_hp;
    protected int current_hp;
    public GameObject HP;  //游戏中角色头顶的绿色血条
    public GameObject HP_down;  //绿色血条后面的灰色条子
    protected const float HP_Length = 1.5f;  //角色固定血条长度，该数值是绿条x方向的缩放系数

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

    //特效相关
    public GameObject Fire_prefab;
    public GameObject Fire;

    public LineRenderer Shield;

    protected virtual void Start()
    {
        init_hp();
        init_power();

        fire_init();  //初始化火焰特效
        shield_init();

        BUFF = new buff[BUFF_NUMS];
    }

    private void fire_init()
    {
        Fire = Instantiate(Fire_prefab, new Vector3(transform.position.x, transform.position.y, 1),
            Quaternion.Euler(-90f, 0f, 0f));
        Fire.SetActive(false);
        Fire.transform.SetParent(transform);
        Fire.transform.localScale = Vector3.one;
    }

    private void shield_init()
    {
        Shield = gameObject.AddComponent<LineRenderer>();
        Shield.useWorldSpace = false;
        Shield.enabled = false;

        Shield.material = new Material(Shader.Find("Sprites/Default"));
        Shield.startWidth = 0.07f;
        Shield.endWidth = 0.07f;
        Shield.startColor = Color.white;
        Shield.endColor = Color.blue;

        Shield.positionCount = 4;
        Shield.loop = true;
        Vector3 Pos0 = Vector3.left * 0.8f;
        Vector3 Pos1 = Vector3.up * 0.8f;
        Vector3 Pos2 = Vector3.right * 0.8f;
        Vector3 Pos3 = Vector3.down * 0.8f;
        Shield.SetPosition(0, Pos0);
        Shield.SetPosition(1, Pos1);
        Shield.SetPosition(2, Pos2);
        Shield.SetPosition(3, Pos3);
    }

    protected virtual void Update()
    {
        
    }


    //----------血量相关接口----------//
    protected void init_hp()
    {
        //初始化血条长度
        HP_down.transform.localScale = new Vector3(
            HP_Length,
            HP.transform.localScale.y,
            HP.transform.localScale.z
            );
        HP.transform.localScale = HP_down.transform.localScale;
    }

    protected void flush_hp()
    {
        //扣血回血时刷新血条长度
        float dx = (HP.transform.localScale.x - 
            (float)current_hp / (float)max_hp * HP_Length) / 2;
        HP.transform.localPosition = new Vector3(
            HP.transform.localPosition.x - dx,
            HP.transform.localPosition.y,
            HP.transform.localPosition.z
            );
        HP.transform.localScale = new Vector3(
            (float)current_hp / (float)max_hp * HP_Length,
            HP.transform.localScale.y,
            HP.transform.localScale.z
            );
    }

    public void increase_hp(int num)
    {
        if(num < 0) { Debug.Log("Wrong HP Changing!"); return; }
        current_hp = Mathf.Min(current_hp + num, max_hp);
        flush_hp();
    }

    public void decrease_hp(int num)
    {
        if (num < 0) { Debug.Log("Wrong HP Changing!"); return; }
        current_hp = Mathf.Max(current_hp - num, 0);
        if (current_hp == 0)  die();
        flush_hp();
    }

    protected void die()
    {
        Destroy(gameObject);
    }

    public int show_hp() {  return current_hp; }

    public bool if_max_hp() { return (current_hp == max_hp); }


    //----------能量相关接口----------
    protected void init_power()
    {
        //初始化能量条
        Power.transform.localScale = new Vector3(
            0,
            Power.transform.localScale.y,
            Power.transform.localScale.z
            );
    }

    protected void flush_power()
    {
        //加减能量时改变能量条长度
        float dx = (Power.transform.localScale.x -
            (float)current_power / (float)max_power * HP_Length) / 2;
        Power.transform.localPosition = new Vector3(
            Power.transform.localPosition.x - dx,
            Power.transform.localPosition.y,
            Power.transform.localPosition.z
            );
        Power.transform.localScale = new Vector3(
            (float)current_power / (float)max_power * HP_Length,
            Power.transform.localScale.y,
            Power.transform.localScale.z
            );
    }

    public bool if_max_power() { return (current_power == max_power); }

    public void increase_power(int num)
    {
        current_power = Mathf.Min(current_power + num, max_power);
        flush_power();
    }

    public void decrease_power(int num)
    {
        current_power = Mathf.Max(current_power - num, 0);
        flush_power();
    }


    //----------BUFF相关接口----------
    public void set_buff(int type, int l, int t)
    {
        //对BUFF[type]进行修改，将其等级提升l级，时间设置为t
        //为“重复使用提升BUFF层数，刷新持续时间”的需求设计
        if(type >= BUFF_NUMS || type < 0) { Debug.Log("BUFF Error!");return; }

        BUFF[type].level = BUFF[type].level + l;
        //不对BUFF层数是否为负，以及上限进行约束，交给释放技能判断

        if(t < 0) { Debug.Log("BUFF Error!"); return; }
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
        for(int i = 0; i < BUFF_NUMS; i++)
        {
            int current_time = BUFF[i].time;
            if (current_time < 0) { Debug.Log("BUFF Time Error!");return;}
            else if(current_time == 0) { continue; }
            else if(current_time == 1)
            {
                BUFF[i].time = 0;
                BUFF[i].level = 0;
            }
            else
            {
                BUFF[i].time = current_time - 1;
            }
        }
        update_buff_play();
    }

    public void update_buff_play()
    {
        //特效相关
        if (show_buff_level(1) > 0) { Fire.SetActive(true); }
        else { Fire.SetActive(false); }

        if (show_buff_level(2) > 0) { Shield.enabled = true; }
        else { Shield.enabled = false; }
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


    //-----技能相关接口（虚函数）-----
    public virtual void ability1()
    {
        Debug.Log("Wrong Inherit!");
    }

    public virtual void ability2()
    {
        Debug.Log("Wrong Inherit!");
    }

    public virtual void abilityX()
    {
        Debug.Log("Wrong Inherit!");
    }
}
