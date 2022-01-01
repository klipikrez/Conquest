using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectLevel : MonoBehaviour
{


    public Animator animator;
    public TextMeshProUGUI txt;
    int current = 0;
    public Level[] levels;

    public void ChangeBiomeNumber(int i)
    {
        current += i;
        ChangeBiome();
    }

    void ChangeBiome()
    {
        if (current >= levels.Length)
        {
            current = 0;
        }
        else
        {
            if (current <= 0)
            {
                current = levels.Length-1;
            }
        }

        txt.text = current.ToString();
        //animator.SetInteger("state", current);
        //animator.Play(levels[current].anim.name);
        animator.CrossFade(levels[current].anim.name, 1);

    }

}
