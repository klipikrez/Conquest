using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class towerEditorToggle : MonoBehaviour
{
    public slider slider;
    public Toggle toggle;
    public void ResetOverride()
    {
        slider.sliderElement.value = (slider.specialValue[0]);
        toggle.SetIsOnWithoutNotify(true);
    }

    public void UpdateValue()
    {
        Debug.Log(slider.text.text + "   ---   " + slider.specilaText + "  ====  " + (slider.text.text == slider.specilaText));

        toggle.SetIsOnWithoutNotify(slider.text.text == slider.specilaText);


    }

}
