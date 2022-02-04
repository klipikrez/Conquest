using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Tymski;

public class SelectLevel : MonoBehaviour
{
    public SceneReference scene;


    public void Play()
    {

        ScenesManager.Instance.Load(scene);

    }

}
