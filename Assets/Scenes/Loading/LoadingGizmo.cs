using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingGizmo : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    string loadingString = "";
    private void Start()
    {



    }

    void Gizmo()
    {
        loadingText.text = "Loading:" + loadingString + ")";
        loadingString += ".";
    }

    public void StartInvokeRepeating()
    {
        InvokeRepeating("Gizmo", 0.1f, 0.05f);
    }
    public void CancleInvokeRepeating()
    {
        CancelInvoke();
    }

}
