using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SnapToScrollViewItem : MonoBehaviour
{

    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public RectTransform sampleListItem;
    public HorizontalLayoutGroup layoutGroup;
    public LevelInfoManage levelManager;
    int currentItem;
    Vector3 autoMovePointerPosition;
    bool autoMoveStartedWithPointerDown;
    float lastViewportWidth = -1f;

    Action handleMove;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLayoutPadding();
        handleMove = ManualMove;
    }

    bool selectionLatch = false;

    // Update is called once per frame
    void Update()
    {
        RectTransform viewport = GetViewport();
        if (!Mathf.Approximately(lastViewportWidth, viewport.rect.width))
            UpdateLayoutPadding();

        //Debug.Log("Current Item: " + currentItem);
        handleMove();

    }

    void ManualMove()
    {
        currentItem = GetNearestItemIndex();
        Debug.Log("Manual Move - Current Item: " + currentItem + " Content Panel Position: " + contentPanel.anchoredPosition.x);
        if (scrollRect.velocity.magnitude < 100)
        {
            if (!selectionLatch)
            {
                selectionLatch = true;
                levelManager.SetSelectedLevel(GetItem(currentItem).levelName);
            }
            MoveToItem(currentItem);
        }
        else
        {
            selectionLatch = false;
        }
    }

    void AutoMove()
    {
        Debug.Log("Auto Move - Current Item: " + currentItem + " Content Panel Position: " + contentPanel.anchoredPosition.x);
        if (scrollRect.velocity.magnitude > 100 || HasManualPointerInput())
        {
            handleMove = ManualMove;
            selectionLatch = false;
            return;
        }



        MoveToItem(currentItem);

    }

    void UpdateLayoutPadding()
    {
        RectTransform viewport = GetViewport();
        int sidePadding = Mathf.Max(0, Mathf.RoundToInt((viewport.rect.width - sampleListItem.rect.width) * 0.5f));
        layoutGroup.padding.left = sidePadding;
        layoutGroup.padding.right = sidePadding;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel);
        lastViewportWidth = viewport.rect.width;
    }

    RectTransform GetViewport()
    {
        return scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;
    }

    int GetNearestItemIndex()
    {
        RectTransform viewport = GetViewport();
        Vector3 viewportCenter = viewport.TransformPoint(viewport.rect.center);
        int nearestIndex = 0;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < contentPanel.childCount; i++)
        {
            RectTransform item = contentPanel.GetChild(i) as RectTransform;
            if (item == null || item.GetComponent<MenuLevel>() == null)
                continue;

            float distance = Mathf.Abs(item.TransformPoint(item.rect.center).x - viewportCenter.x);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    MenuLevel GetItem(int index)
    {
        return contentPanel.GetChild(index).GetComponent<MenuLevel>();
    }

    void MoveToItem(int index)
    {
        RectTransform viewport = GetViewport();
        RectTransform item = contentPanel.GetChild(index) as RectTransform;
        Vector3 viewportCenter = viewport.TransformPoint(viewport.rect.center);
        Vector3 itemCenter = item.TransformPoint(item.rect.center);
        Vector3 targetPosition = contentPanel.position + (viewportCenter - itemCenter);
        contentPanel.position = Vector3.Lerp(contentPanel.position, targetPosition, Time.deltaTime * 3f);
    }

    bool HasManualPointerInput()
    {
        if (!Input.GetMouseButton(0))
        {
            autoMoveStartedWithPointerDown = false;
            return false;
        }

        if (!autoMoveStartedWithPointerDown)
        {
            autoMoveStartedWithPointerDown = true;
            autoMovePointerPosition = Input.mousePosition;
            return false;
        }

        return IsPointerOverContent() &&
            (Input.mousePosition - autoMovePointerPosition).sqrMagnitude > 16f;
    }


    private bool IsPointerOverContent()
    {
        PointerEventData pointerData =
            new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(contentPanel))
                return true;
        }

        return false;
    }


    public void SnapToItem(string levelName)
    {
        currentItem = 0;
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            if (contentPanel.GetChild(i).GetComponent<MenuLevel>().levelName == levelName)
            {
                currentItem = i;
                break;
            }
        }
        levelManager.SetSelectedLevel(GetItem(currentItem).levelName);
        handleMove = AutoMove;
    }
}
