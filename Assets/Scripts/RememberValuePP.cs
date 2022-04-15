using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RememberValuePP : MonoBehaviour
{
    public Slider inputSlider;
    public bool isFloat;
    public Toggle inputCheck;
    public string inputName;

    // Start is called before the first frame update
    void Awake()
    {/*
        if (inputName != null)
            if (inputSlider != null)
            {
                if (isFloat)
                {
                    inputSlider.value = PlayerPrefs.GetFloat(inputName);
                    Debug.Log(inputName + ": " + PlayerPrefs.GetFloat(inputName));
                }
                else
                {
                    inputSlider.value = PlayerPrefs.GetInt(inputName);
                    Debug.Log(inputName + ": " + PlayerPrefs.GetInt(inputName));
                }
            }
            else
            {
                if (inputCheck != null)
                {
                    inputCheck.isOn = PlayerPrefs.GetInt(inputName) == 1 ? true : false;
                    Debug.Log(inputName + ": " + (PlayerPrefs.GetInt(inputName) == 1 ? true : false));
                }
            }*/
    }

    // Update is called once per frame
    void Update()
    {

    }
}
