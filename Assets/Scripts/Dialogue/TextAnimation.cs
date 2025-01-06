using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class TextAnimation : ScriptableObject
{
    public class BOB
    {
        public Vector2 translation;
        public float rotation;
        public float scale;

        public BOB()
        {
            translation = Vector2.zero;
            rotation = 0f;
            scale = 1f;
        }
    }
    public string animationName = "New Animation";
    public abstract BOB Animate(int characterIndex);
    public abstract void AnimateAll(int start, int lenth, TextMeshProUGUI text);
    //public abstract IEnumerator CorutineText(int start, int lenth, TextMeshProUGUI text);
}
