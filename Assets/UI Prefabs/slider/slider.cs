using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq.Expressions;

public class slider : MonoBehaviour
{
    public bool showDecimal = false;
    public string textString;
    public float[] specialValue = { 0, 1 };
    public string specilaText;
    public Slider sliderElement;
    public TMP_Text percent;
    public TMP_Text text;
    public TMP_Text text2;
    public TMP_InputField inputField;
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
    public void UpdateValue(string value)
    {
        if (!float.TryParse(value, out var val)) return;
        if (val > sliderElement.maxValue) val = sliderElement.maxValue;
        if (val < sliderElement.minValue) val = sliderElement.minValue;
        sliderElement.value = val;
        FUNKCIJA(val);
        //UpdateValue(val);
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
        akoOvoRadi.anchoredPosition = -new Vector3(akoOvoRadi.rect.width - akoOvoRadi.rect.width * ((value - sliderElement.minValue) / (sliderElement.maxValue - sliderElement.minValue)), 0, 0);
        NemaSanse.anchoredPosition = new Vector3(akoOvoRadi.rect.width - akoOvoRadi.rect.width * ((value - sliderElement.minValue) / (sliderElement.maxValue - sliderElement.minValue)), 0, 0);
        inputField.text = (value).ToString(showDecimal ? "0.0" : "0");
        //akoOvoRadi.offsetMin = new Vector2(0, akoOvoRadi.offsetMin.y);
    }

    public void SetText(string text)
    {
        percent.text = text;
    }
}
