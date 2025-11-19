using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_C : MonoBehaviour
{
    /*
     * 该脚本为敌人数值及技能
     */

    //血条相关
    private int max_hp;
    private int current_hp;
    public GameObject HP;  //游戏中敌人头顶的红色血条
    public GameObject HP_down;  //红色血条的背景，那个灰色的条子
    const float HP_SCALE = 0.0015f;   //血条长度系数，1.5/1000，敌人血条长度随血量变化

    //攻击相关
    private int attack = 200;

    void Start()
    {
        max_hp = 2000;
        current_hp = max_hp;
        init_hp();
    }

    void Update()
    {
        
    }

    void init_hp()
    {
        //根据最大血量初始化血条长度
        HP_down.transform.localScale = new Vector3(
            max_hp * HP_SCALE,
            HP.transform.localScale.y,
            HP.transform.localScale.z
            );
        HP.transform.localScale = HP_down.transform.localScale;
    }

    void flush_hp()
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

    public void increase_hp( int num)
    {
        //加血，但不超过上限
        current_hp = Mathf.Min( current_hp + num, max_hp );
        flush_hp();
    }

    public void decrease_hp( int num)
    {
        //扣血，但不为零
        current_hp = Mathf.Max( current_hp - num, 0);
        if (current_hp == 0)  die();  //血量为零则死亡
        flush_hp();
    }

    void die()
    {
        //死亡，在0.5s后销毁该对象，这个时间用来放个动画之类的
        Destroy(gameObject, 0.5f);
    }

    public void ability1()
    {
        //技能1，对玩家造成100%攻击力伤害

        GameObject target = GameObject.Find("Player");
        target.GetComponent<Player_C>().decrease_hp(attack);
    }

}
