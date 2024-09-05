using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialSlides : MonoBehaviour
{
    public GameObject[] slides;
    public void StartSlides()
    {
        gameObject.SetActive(true);
        foreach (GameObject slide in slides)
        {
            slide.SetActive(true);
        }
    }
}
