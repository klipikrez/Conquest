using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectLevel : MonoBehaviour
{

    [System.Serializable]
    public class Level
    {
        public int biome;
        public GameObject LevelObj;
    }

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
        if (current >= 5)
        {
            current = 1;
        }
        else
        {
            if (current <= 0)
            {
                current = 4;
            }
        }

        txt.text = current.ToString();
        animator.SetInteger("state", current);


    }

}
