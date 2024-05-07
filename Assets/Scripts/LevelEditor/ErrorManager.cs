using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorManager : MonoBehaviour
{
    public GameObject prefab;
    public static ErrorManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    public void SendError(string text)
    {
        Error err = Instantiate(prefab, transform).GetComponent<Error>();
        err.Initiate(text, messageType.Error);
    }
    public void SendSucsess(string text)
    {
        Error err = Instantiate(prefab, transform).GetComponent<Error>();
        err.Initiate(text, messageType.Sucsess);
    }
}
