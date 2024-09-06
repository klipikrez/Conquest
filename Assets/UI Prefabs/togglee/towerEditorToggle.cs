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
        EditorOptions.Instance.ClearOverride(slider.textString);
    }

    public void UpdateValue()
    {


        toggle.SetIsOnWithoutNotify(slider.text.text == slider.specilaText);
        EditorOptions.Instance.UpdateOverrideValue(slider.textString, slider.sliderElement.value);

    }





}
