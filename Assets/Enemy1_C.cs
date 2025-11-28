using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy1_C : Enemy_C
{
    public LineRenderer linerenderer;

    protected override void Start()
    {
        max_hp = 6000;
        current_hp = max_hp;
        init_hp();

        attack = 200;

        BUFF = new buff[BUFF_NUMS];

        linerenderer_initialize();
    }

    private void linerenderer_initialize()
    {
        linerenderer.positionCount = 2;
        linerenderer.material = new Material(Shader.Find("Sprites/Default"));
        linerenderer.enabled = false;
    }

    protected override void Update()
    {
        
    }

    public override void movement()
    {
        /*
         * 在4,8,12……回合对全体造成150%攻击力伤害
         * 在2,6,10,14……非4倍数偶数回合对血量最高角色造成200%攻击力伤害
         * 在1,3,5……奇数回合对随机角色造成100%攻击力伤害
         */

        if (localround % 4 == 0) ability2();
        else if (localround % 2 == 0) ability3();
        else ability1();
        localround++;
        update_buff();
    }

    public override void ability1()
    {
        //技能1，对随机角色造成100%攻击力伤害

        StartCoroutine(ability1_play());
    }

    private IEnumerator ability1_play()
    {
        yield return new WaitForSeconds(0.1f);

        linerenderer.startWidth = 0.2f;
        linerenderer.endWidth = 0.2f;
        linerenderer.startColor = Color.white;
        linerenderer.endColor = Color.white;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 6f;
        linerenderer.SetPosition(0, startPos);
        linerenderer.SetPosition(1, endPos);
        linerenderer.enabled = true;
        yield return new WaitForSeconds(0.45f);

        linerenderer.enabled = false;
        yield return new WaitForSeconds(0.1f);

        GameObject target = null;
        for (int i = 0; i < 30; i++)
        {
            int randomInt = Random.Range(1, 4);
            target = GameObject.Find($"Player{randomInt}");
            if (target != null)
            {
                break;
            }
        }

        endPos = target.transform.position;
        startPos = endPos  + Vector3.up * 6f;
        linerenderer.SetPosition(0, startPos);
        linerenderer.SetPosition(1, endPos);
        linerenderer.enabled = true;

        int final_attack = calculate_damage_for_attack(1.0f);
        target.GetComponent<Player_C>().be_injured(final_attack);
        yield return new WaitForSeconds(0.45f);

        linerenderer.enabled = false;
        update_buff();
    }

    public override void ability2()
    {
        //技能2，对全体存活角色造成150%攻击力伤害

        StartCoroutine(ability2_play());
    }

    private IEnumerator ability2_play()
    {
        yield return new WaitForSeconds(0.1f);

        linerenderer.startWidth = 0.3f;
        linerenderer.endWidth = 0.3f;
        linerenderer.startColor = Color.black;
        linerenderer.endColor = Color.black;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.left * 20f;
        linerenderer.SetPosition(0, startPos);
        linerenderer.SetPosition(1, endPos);
        linerenderer.enabled = true;

        for (int i = 1; i <= 3; i++)
        {
            GameObject target = GameObject.Find($"Player{i}");
            if (target != null)
            {
                int final_attack = calculate_damage_for_attack(0.5f);
                target.GetComponent<Player_C>().be_injured(final_attack);
            }
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 1; i <= 3; i++)
        {
            GameObject target = GameObject.Find($"Player{i}");
            if (target != null)
            {
                int final_attack = calculate_damage_for_attack(0.5f);
                target.GetComponent<Player_C>().be_injured(final_attack);
            }
        }
        yield return new WaitForSeconds(0.5f);

        for (int i = 1; i <= 3; i++)
        {
            GameObject target = GameObject.Find($"Player{i}");
            if (target != null)
            {
                int final_attack = calculate_damage_for_attack(0.5f);
                target.GetComponent<Player_C>().be_injured(final_attack);
            }
        }
        linerenderer.enabled = false;
        update_buff();
    }

    public override void ability3()
    {
        //技能3，对血量最高角色造成200%攻击力伤害

        StartCoroutine(ability3_play());
    }

    private IEnumerator ability3_play()
    {
        yield return new WaitForSeconds(0.1f);

        linerenderer.startWidth = 0.2f;
        linerenderer.endWidth = 0.2f;
        linerenderer.startColor = Color.black;
        linerenderer.endColor = Color.black;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * 6f;
        linerenderer.SetPosition(0, startPos);
        linerenderer.SetPosition(1, endPos);
        linerenderer.enabled = true;
        yield return new WaitForSeconds(0.45f);

        linerenderer.enabled = false;
        yield return new WaitForSeconds(0.1f);

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

        GameObject target = null;
        for (int i = 2; i >= 0; i--)
        {
            target = GameObject.Find($"Player{sort[i]}");
            if (target != null)
            {
                break;
            }
        }

        endPos = target.transform.position;
        startPos = endPos + Vector3.up * 6f;
        linerenderer.SetPosition(0, startPos);
        linerenderer.SetPosition(1, endPos);
        linerenderer.enabled = true;

        int final_attack = calculate_damage_for_attack(2.0f);
        target.GetComponent<Player_C>().be_injured(final_attack);
        yield return new WaitForSeconds(0.45f);

        linerenderer.enabled = false;
        update_buff();
    }
}