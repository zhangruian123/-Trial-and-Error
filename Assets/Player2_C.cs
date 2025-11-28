using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2_C : Player_C
{
    public Animation anim;

    protected override void Start()
    {
        base.Start();

        max_hp = 1500;
        current_hp = max_hp;

        max_power = 80;
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

        anim.Play("P2_attack");
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
        //技能2，恢复30%已损失生命，获得3层减伤，直到下个自己回合结束，
        //重复使用不叠加层数，刷新时间

        StartCoroutine(ability2_play());
    }

    private IEnumerator ability2_play()
    {
        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(0.3f);

        increase_power(40);

        increase_hp((max_hp - current_hp) * 3 / 10);

        set_buff(2, 3 - show_buff_level(2), 2);
        //获得2回合的减伤

        update_buff();
    }

    public override void abilityX()
    {
        //大招，对敌方造成相当于已损失生命值70%的伤害（不吃攻击BUFF）
        //若此时身上存在减伤BUFF，刷新持续时间并扩散给队友

        StartCoroutine(abilityX_play());
    }

    private IEnumerator abilityX_play()
    {
        yield return new WaitForSeconds(0.1f);

        anim.Play("P2_attack");
        yield return new WaitForSeconds(0.7f);

        decrease_power(max_power);  //消耗全部能量

        if (show_buff_level(2) > 0)  //存在减伤则扩散全体
        {
            for (int i = 1; i <= 3; i++)
            {
                GameObject friend = GameObject.Find($"Player{i}");
                if (friend != null)
                {
                    int now_defend_level = friend.GetComponent<Player_C>().show_buff_level(2);
                    friend.GetComponent<Player_C>().set_buff(2, 3 - now_defend_level, 2);
                    friend.GetComponent<Player_C>().update_buff_play();
                }
            }
        }

        int final_attack = (max_hp - current_hp) * 7 / 10;  //计算伤害
        GameObject target = GameObject.Find("Enemy1");  //寻找目标
        if (target != null)
        {
            target.GetComponent<Enemy_C>().be_injured(final_attack);
        }
        yield return new WaitForSeconds(0.3f);

        update_buff();
    }
}