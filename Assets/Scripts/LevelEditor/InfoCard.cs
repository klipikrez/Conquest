using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCard : MonoBehaviour
{
    public string infoText = "";
    public Animator anim;

    public void ShowInfoCard()
    {
        Debug.Log("aaa");
        anim.Play("infoClick");
        ErrorManager.Instance.SendSucsess(infoText);
    }
}
