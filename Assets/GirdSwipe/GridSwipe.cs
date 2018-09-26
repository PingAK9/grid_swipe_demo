using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class UnityEventInt : UnityEvent<int> { }

public class GridSwipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    float valuePos;
    public Transform content;
    List<SwipeItem> listItem;
    public Transform pagination;
    List<Toggle> listPagination;
    public float minScale;
    public float spacing;
    public Button nextButton;
    public Button prevButton;
    Canvas myCanvas;

    private void Start()
    {
        Canvas[] c = gameObject.GetComponentsInParent<Canvas>();
        myCanvas = c[c.Length - 1];
        nextButton.onClick.AddListener(OnNext);
        prevButton.onClick.AddListener(OnBack);
        currentPage = -1;
        OnRefresh();
    }
    void OnRefresh()
    {
        listItem = new List<SwipeItem>();
        for (int i = 0; i < content.childCount; i++)
        {
            SwipeItem _item = content.GetChild(content.childCount - i - 1).GetComponent<SwipeItem>();
            if (_item != null)
            {
                listItem.Add(_item);
            }
            else
            {
                Debug.Log("Gameobject missing component SwipeItem");
            }
        }
        listPagination = new List<Toggle>();
        if (pagination != null)
        {
            for (int i = 0; i < pagination.childCount; i++)
            {
                Toggle _tg = pagination.GetChild(i).GetComponent<Toggle>();
                if (_tg != null)
                {
                    listPagination.Add(_tg);
                }
            }
        }
        SetPosition(valuePos);
    }

    void SetPosition(float value) {
        value = Mathf.Clamp(value, 0, listItem.Count - 1);
        valuePos = value;
        for (int i = 0; i < listItem.Count; i++)
        {
            float _offset = value - i;
            if (_offset > -1 && _offset < 1)
            {
                listItem[i].SetPosition(_offset, minScale, spacing);
            }
            else if(_offset <= -1)
            {
                listItem[i].SetPosition(-1, minScale, spacing);
            }
            else
            {
                listItem[i].SetPosition(1, minScale, spacing);
            }
        }
        OnChangePage(Mathf.RoundToInt(valuePos));
    }

    /// <summary>
    public void OnNext()
    {
        if (valuePos >= (listItem.Count-1))
        {
            return;
        }
        isOn = true;
        animStartTime = Time.realtimeSinceStartup;
        animDeltaTime = 0;
        oldPage = valuePos;
        nextPage = (int)(oldPage + 1);
        animationDuration = animationDurationConst * (nextPage - oldPage);
    }
    public void OnBack()
    {
        if (valuePos <= 0)
        {
            return;
        }
        isOn = true;
        animStartTime = Time.realtimeSinceStartup;
        animDeltaTime = 0;
        oldPage = valuePos;
        float intOld = Mathf.Floor(oldPage);
        if (intOld == oldPage)
        {
            nextPage = (int)(oldPage - 1);
        }else
        {
            nextPage = intOld;
        }
        animationDuration = animationDurationConst * (oldPage - nextPage);
    }
    float nextPage;
    float oldPage;
    const float animationDurationConst = 0.2f;
    float animationDuration = 0.5f;
    private float animStartTime;
    private float animDeltaTime;
    bool isOn = false;
    private void Update()
    {
        if (isOn)
        {
            if (animDeltaTime <= animationDuration)
            {
                animDeltaTime = Time.realtimeSinceStartup - animStartTime;
                // effect
                float _pos = Anim.Liner(oldPage, nextPage, animDeltaTime, animationDuration);
                SetPosition(_pos);
            }
            else
            {
                isOn = false;
                SetPosition(nextPage);
            }
        }
        else if (isDown)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, null, out pos);
            float endDrag = myCanvas.transform.TransformPoint(pos).x;
            float offsetPos = (endDrag - startDrag) / offsetDrag;
            float endPos = rootPos - offsetPos;
            SetPosition(endPos);
        }
    }
    #region Swipe
    bool isDown = false;
    float startDrag;
    float rootPos;
    public float offsetDrag = 100;
    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        rootPos = valuePos;
        currentPage = Mathf.RoundToInt(valuePos);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, null, out pos);
        startDrag = myCanvas.transform.TransformPoint(pos).x;
    }
    protected const float timeMove = 0.2f;
    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, null, out pos);
        float endDrag = myCanvas.transform.TransformPoint(pos).x;
        float offsetPos = (endDrag - startDrag) / offsetDrag;
        float endPos = rootPos - offsetPos;
        float round = Mathf.Round(endPos);
        if (round > endPos)
        {
            OnNext();
        }
        else
        {
            OnBack();
        }
    }
    #endregion


    public UnityEventInt OnChangePageFinish;

    public int currentPage { get; private set; }
    void OnChangePage(int _round)
    {
        if (_round != currentPage)
        {
            currentPage = _round;
            if (OnChangePageFinish != null) OnChangePageFinish.Invoke(currentPage);
            UpdatePagination();
        }
    }
    void UpdatePagination()
    {
        if (listPagination != null)
        {
            for (int i = 0; i < listPagination.Count; i++)
            {
                listPagination[i].isOn = currentPage == i;
            }
        }
    }
}
