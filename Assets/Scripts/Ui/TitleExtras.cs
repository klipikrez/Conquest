using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleExtras : MonoBehaviour
{
    public SnapToScrollViewItem snapToScrollViewItem;
    public TMP_Text title;

    // Update is called once per frame
    void Update()
    {
        title.text = snapToScrollViewItem.gameObject.transform.GetChild(0).GetChild(0).GetChild(snapToScrollViewItem.currentItem).name;
    }
}
