using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1_C : Player_C
{
    public Animation anim;

    protected override void Start()
    {
        base.Start();

        max_hp = 1000;
        current_hp = max_hp;

        max_power = 120;
        current_power = 0;

        attack = 100;
    }

    public override void ability1()
    {
        //技能1，对敌方造成100%攻击力的伤害

        StartCoroutine(ability1_play());
    }

    private IEnumerator ability1_play()
    {
        yield return new WaitForSeconds(0.1f);

        anim.Play("P1_attack");
        yield return new WaitForSeconds(0.7f);

        increase_power(40);  //加40能量

        int final_attack = calculate_damage_for_attack(1.0f);  //计算伤害

        GameObject target = GameObject.Find("Enemy1");  //寻找目标
        if (target != null)
        {
            target.GetComponent<Enemy_C>().be_injured(final_attack);
        }
        yield return new WaitForSeconds(0.3f);

        update_buff();  //更新BUFF持续时间
    }

    public override void ability2()
    {
        //技能2，获得4层攻击BUFF，持续时间3回合
        //重复使用层数叠加并刷新持续时间，最多叠加20层

        StartCoroutine(ability2_play());
    }

    private IEnumerator ability2_play()
    {
        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(0.3f);

        increase_power(40);

        int now_attack_level = show_buff_level(1);
        if (now_attack_level < 16) set_buff(1, 4, 3);
        else set_buff(1, 20 - now_attack_level, 3);
        update_buff_play();

        update_buff();
    }

    public override void abilityX()
    {
        //大招，对敌方造成200%攻击力的伤害，刷新攻击BUFF持续时间

        StartCoroutine(abilityX_play());
    }

    private IEnumerator abilityX_play()
    {
        yield return new WaitForSeconds(0.1f);

        anim.Play("P1_attack");
        yield return new WaitForSeconds(0.7f);

        decrease_power(max_power);  //消耗全部能量

        if (show_buff_level(1) > 0) set_buff(1, 0, 3);
        //存在攻击BUFF时，刷新持续时间

        int final_attack = calculate_damage_for_attack(2.0f);
        GameObject target = GameObject.Find("Enemy1");  //寻找目标
        if (target != null)
        {
            target.GetComponent<Enemy_C>().be_injured(final_attack);
        }
        yield return new WaitForSeconds(0.3f);

        update_buff();
    }
}