using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AmountBar : MonoBehaviour
{
    public RectTransform rect;
    public Image img;
    public TextMeshProUGUI numberTMP;

    public void UpdateValue(float value1, float value2, float value, Color color)
    {
        //Debug.Log(gameObject.name + "     " + -value1 + " - " + value2 + " || " + value);
        //Debug.Log(((RectTransform)transform.parent.transform).sizeDelta.x);
        numberTMP.text = ((int)value).ToString();
        SetLeft(rect, ((RectTransform)transform.parent.transform).rect.width * value1);
        SetRight(rect, ((RectTransform)transform.parent.transform).rect.width * (1 - value2));
        if (img.color != color)
        {
            img.color = color;
        }
    }

    public void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public void SetBottom(RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
