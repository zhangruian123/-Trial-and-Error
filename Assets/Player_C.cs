using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_C : MonoBehaviour
{
    /*
     * 该脚本为角色数值及技能
     */

    //血条相关
    private int max_hp;
    private int current_hp;
    public GameObject HP;  //游戏中角色头顶的绿色血条
    public GameObject HP_down;  //绿色血条后面的灰色条子
    const float HP_Length = 1.5f;  //角色固定血条长度，该数值是绿条x方向的缩放系数

    //攻击相关
    private int attack = 200;

    //能量相关，当前能量等于最大能量时可以解锁大招
    private int max_power;
    private int current_power;
    public GameObject Power;  //游戏中蓝色的能量条

    void Start()
    {
        max_hp = 1000;
        current_hp = max_hp;
        max_power = 120;
        current_power = 0;
        init_hp();
        init_power();
    }

    void Update()
    {

    }

    void init_hp()
    {
        //初始化血条长度
        HP_down.transform.localScale = new Vector3(
            HP_Length,
            HP.transform.localScale.y,
            HP.transform.localScale.z
            );
        HP.transform.localScale = HP_down.transform.localScale;
    }

    void flush_hp()
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

    void init_power()
    {
        //初始化能量条
        Power.transform.localScale = new Vector3(
            0,
            Power.transform.localScale.y,
            Power.transform.localScale.z
            );
    }

    void flush_power()
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

    public bool if_max_power()
    {
        //是否满能量
        if(current_power == max_power) return true;
        else return false;
    }

    public void increase_hp(int num)
    {
        //加血但不超上限
        current_hp = Mathf.Min(current_hp + num, max_hp);
        flush_hp();
    }

    public void decrease_hp(int num)
    {
        //扣血但不为负
        current_hp = Mathf.Max(current_hp - num, 0);
        if (current_hp == 0)  die();
        flush_hp();
    }

    void die()
    {
        Destroy(gameObject, 0.5f);  //0.5s后死亡
    }

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

    public void ability1()
    {
        //技能1，对敌方造成100%攻击力的伤害

        increase_power(40);  //加40能量
        GameObject target = GameObject.Find("Enemy");
        target.GetComponent<Enemy_C>().decrease_hp(attack);
    }

    public void ability2()
    {
        //技能2，恢复1/2最大生命

        increase_power(40);
        increase_hp(max_hp / 2);
    }

    public void abilityX()
    {
        //大招，对敌方造成300%攻击力的伤害

        decrease_power(max_power);  //消耗全部能量
        GameObject target = GameObject.Find("Enemy");
        target.GetComponent<Enemy_C>().decrease_hp(attack * 3);
    }
}
