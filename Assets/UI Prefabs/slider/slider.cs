using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class slider : MonoBehaviour
{
    public string textString;
    public float[] specialValue = { 0, 1 };
    public string specilaText;
    [NonSerialized]
    public Slider sliderElement;
    public TMP_Text percent;
    public TMP_Text text;
    public TMP_Text text2;
    public RectTransform akoOvoRadi;
    public RectTransform NemaSanse;

    // Start is called before the first frame update
    private void Start()
    {
        if (textString == null || textString == "")
        {
            textString = text2.text;
        }
        if (sliderElement == null)
        {
            sliderElement = gameObject.GetComponent<Slider>();
        }
        UpdateValue(sliderElement.value);
    }
    public void UpdateValue(float value)
    {



        if (specialValue[0] - 0.01f <= value && specialValue[1] + 0.01f >= value)
        {
            if (specilaText != null && specilaText != "")
            {
                text.text = specilaText;
                text2.text = specilaText;
            }
        }
        else
        {
            if (text.text != textString)
            {
                text.text = textString;
                text2.text = textString;
            }
        }


        if (sliderElement == null)
        {
            sliderElement = gameObject.GetComponent<Slider>();
        }
        //ovo je najgluplja stvar koju sam napisao u poslednje vreme, al radi :D
        //nije vredno dokumentarisati
        FUNKCIJA(value);



    }
    public void FUNKCIJA(float value)
    {
        akoOvoRadi.anchoredPosition = -new Vector3(436.22f - 436.22f * (value / (sliderElement.maxValue - sliderElement.minValue)), 0, 0);
        NemaSanse.anchoredPosition = new Vector3(436.22f - 436.22f * (value / (sliderElement.maxValue - sliderElement.minValue)), 0, 0);
        percent.text = (value).ToString("0.0");
    }
}
