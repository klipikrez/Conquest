using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class slider : MonoBehaviour
{
    public string textString;
    public float[] specialValue = { 0, 1 };
    public string specilaText;
    Slider sliderElement;
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
        if (specilaText != null)
        {


            if (specialValue[0] <= value && specialValue[1] >= value)
            {
                text.text = specilaText;
                text2.text = specilaText;
            }
            else
            {
                if (text.text != textString)
                {
                    text.text = textString;
                    text2.text = textString;
                }
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
    void FUNKCIJA(float value)
    {
        akoOvoRadi.anchoredPosition = -new Vector3(436.22f - 436.22f * (value / sliderElement.maxValue), 0, 0);
        NemaSanse.anchoredPosition = new Vector3(436.22f - 436.22f * (value / sliderElement.maxValue), 0, 0);
        percent.text = (value).ToString("0");
    }
}
