using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamButton : MonoBehaviour
{

    public Image selecotr;

    public int team = 0;
    public CreateTeamButtons master;




    public void Deselelect()
    {
        //Background.color = new Color(0, 0, 0, 0);
        selecotr.color = new Color(1, 1, 1, 0);
    }

    public void Select()
    {
        //Background.color = new Color(0, 0, 0, 1);

        master.DeselectAll();
        selecotr.color = new Color(1, 1, 1, 1);
        EditorOptions.Instance.team = team;
    }



}
