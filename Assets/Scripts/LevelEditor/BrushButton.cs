using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushButton : MonoBehaviour
{
    Texture2D tex;
    //public Image Background;
    public Image selecotr;
    public RawImage BrushImage;
    public CreateBrushButtons master;
    public void SetTexture(Texture2D texture2D)
    {
        tex = texture2D;
        BrushImage.texture = tex;
    }
    // Start is called before the first frame update
    void Start()
    {

    }




    public void Deselelect()
    {
        //Background.color = new Color(0, 0, 0, 0);
        selecotr.color = new Color(1, 1, 1, 0);
    }

    public void Select()
    {
        //Background.color = new Color(0, 0, 0, 1);

        SetEditorBrush();
        selecotr.color = new Color(1, 1, 1, 1);
    }

    public void SetEditorBrush()
    {
        EditorOptions.Instance.SetBrushImage(tex);
        master.DeselectAll();
    }
}
