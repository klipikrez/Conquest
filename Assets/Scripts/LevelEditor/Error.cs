using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
public enum messageType
{
    Error = 0,
    Sucsess = 1,
}
public class Error : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public RawImage image;
    public Texture2D[] errorImages;
    // Start is called before the first frame update
    public void Initiate(string text, messageType type)
    {
        image.texture = errorImages[(int)type];
        textUI.text = text;
        SoundManager.Instance.PlayAudioClip(((int)type == 0) ? 9 : 7);
    }
    float time = 8;
    float timer = 0;
    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;
        if (timer > time)
            Destroy(gameObject);
    }
}
