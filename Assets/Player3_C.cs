using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class Player3_C : Player_C
{
    public GameObject Pea_prefab;
    GameObject[] pea;
    float[] timers;

    protected override void Start()
    {
        base.Start();

        max_hp = 1800;
        current_hp = max_hp;

        max_power = 80;
        current_power = 0;

        attack = 50;

        pea = new GameObject[3];
        timers = new float[3];
    }

    protected override void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            if (pea[i] != null)
            {
                pea[i].transform.Translate(Vector3.down * 6f * Time.deltaTime);
                timers[i] += Time.deltaTime;
                if (timers[i] >= 0.66f)
                {
                    Destroy(pea[i]);
                    pea[i] = null;
                    timers[i] = 0;
                }
            }
        }
    }

    private void a_pea_down_on(Vector3 location)
    {
        for(int i = 0; i < 3; i++)
        {
            if (pea[i] == null)
            {
                pea[i] = Instantiate(Pea_prefab, location + Vector3.up * 4f, Quaternion.identity);
                break;
            }
        }  
    }

    public override void ability1()
    {
        //技能1，对敌方造成100%攻击力的伤害

        StartCoroutine(ability1_play());
    }

    private IEnumerator ability1_play()
    {
        yield return new WaitForSeconds(0.1f);

        increase_power(40);  //加40能量

        int final_attack = calculate_damage_for_attack(1.0f);  //计算伤害

        GameObject target = GameObject.Find("Enemy1");  //寻找目标
        if (target != null)
        {
            a_pea_down_on(target.transform.position);
        }
        yield return new WaitForSeconds(0.66f);

        target.GetComponent<Enemy_C>().be_injured(final_attack);
        yield return new WaitForSeconds(0.34f);

        update_buff();  //更新BUFF持续时间
    }

    public override void ability2()
    {
        //技能2，为血量最少友军恢复自身最大生命值25%的生命
        //若其满血，则寻找第二少的，以此类推

        StartCoroutine(ability2_play());
    }

    private IEnumerator ability2_play()
    {
        yield return new WaitForSeconds(0.1f);

        increase_power(40);

        int p1 = 0;
        if (GameObject.Find("Player1") != null) p1 =
                GameObject.Find("Player1").GetComponent<Player_C>().show_hp();
        int p2 = 0;
        if (GameObject.Find("Player2") != null) p2 =
                GameObject.Find("Player2").GetComponent<Player_C>().show_hp();
        int p3 = 0;
        if (GameObject.Find("Player3") != null) p3 =
                GameObject.Find("Player3").GetComponent<Player_C>().show_hp();
        int[] sort = new int[3];  //存放升序的序号
        if (p1 <= p2 && p1 <= p3)
        {
            sort[0] = 1;
            sort[1] = (p2 <= p3) ? 2 : 3;
            sort[2] = (p2 <= p3) ? 3 : 2;
        }
        else if (p2 <= p1 && p2 <= p3)
        {
            sort[0] = 2;
            sort[1] = (p1 <= p3) ? 1 : 3;
            sort[2] = (p1 <= p3) ? 3 : 1;
        }
        else
        {
            sort[0] = 3;
            sort[1] = (p1 <= p2) ? 1 : 2;
            sort[2] = (p1 <= p2) ? 2 : 1;
        }

        GameObject friend = null;
        for (int i = 0; i < 3; i++)
        {
            friend = GameObject.Find($"Player{sort[i]}");
            if (friend != null)
            {
                if (!friend.GetComponent<Player_C>().if_max_hp())
                {
                    a_pea_down_on(friend.transform.position);
                    break;
                }
            }
        }
        yield return new WaitForSeconds(0.66f);

        friend.GetComponent<Player_C>().increase_hp(max_hp / 4);
        yield return new WaitForSeconds(0.34f);

        update_buff();  //更新BUFF持续时间
    }

    public override void abilityX()
    {
        //大招，为全队恢复自身40%最大生命的生命

        StartCoroutine(abilityX_play());
    }

    private IEnumerator abilityX_play()
    {
        yield return new WaitForSeconds(0.1f);

        decrease_power(max_power);  //消耗全部能量

        GameObject[] friends = new GameObject[3];
        for (int i = 1; i <= 3; i++)
        {
            friends[i - 1] = GameObject.Find($"Player{i}");
            if (friends[i - 1] != null)
            {
                a_pea_down_on(friends[i - 1].transform.position);
            }
        }
        yield return new WaitForSeconds(0.66f);

        for(int i = 0; i < 3; i++)
        {
            if (friends[i]  != null)
            {
                friends[i].GetComponent<Player_C>().increase_hp(max_hp * 4 / 10);
            }      
        }
        yield return new WaitForSeconds(0.34f);

        update_buff();  //更新BUFF持续时间
    }
}