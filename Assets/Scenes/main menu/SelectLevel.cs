using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{


    public Animator animator;
    public TextMeshProUGUI nimbe;
    public TextMeshProUGUI teks;
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
            current = levels.Length - 1;
        }
        else
        {
            if (current < 0)
            {
                current = 0;
            }
        }

        nimbe.text = current.ToString();
        teks.text = levels[current].LevelName;
        //animator.SetInteger("state", current);
        //animator.Play(levels[current].anim.name);
        animator.CrossFade(levels[current].anim.name, 1);

    }

    public void Play()
    {
        SceneManager.LoadScene(levels[current].levelScene);

    }

}
