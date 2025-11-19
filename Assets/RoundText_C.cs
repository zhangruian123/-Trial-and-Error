using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundText_C : MonoBehaviour
{
    public TextMeshProUGUI roundText;
    void Start()
    {
        roundText.text = $"Round 1";
    }
    public void change_round_text(int round)
    {
        roundText.text = $"Round {round}";
    }
    void Update()
    {
        
    }
}
