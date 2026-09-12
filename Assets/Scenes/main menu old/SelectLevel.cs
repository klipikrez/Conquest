using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Tymski;

public class SelectLevel : MonoBehaviour
{
    public SceneReference scene;
    public string levelName;
    public Animator animator;
    public int level = 0;
    public void Play()
    {

        ScenesManager.Instance.LoadLevel(scene, levelName);

    }

    public void ChangeBiomeNumber(int i)
    {
        level += i;
        if (level >= 7) level = 0;
        else if (level < 0) level = 6;
        SetCamera();
    }

    public void SetCamera()
    {
        switch (level)
        {
            case 0:
                {
                    animator.CrossFade("1-0", 1);
                    break;
                }
            case 1:
                {
                    animator.CrossFade("1-1", 1);
                    break;
                }
            case 2:
                {
                    animator.CrossFade("riverna", 1);
                    break;
                }
            case 3:
                {
                    animator.CrossFade("severna", 1);
                    break;
                }
            case 4:
                {
                    animator.CrossFade("raverna", 1);
                    break;
                }
            case 5:
                {
                    animator.CrossFade("kan", 1);
                    break;
                }
            default:
                {
                    animator.CrossFade("default", 1);
                    break;
                }
        }
    }

}
