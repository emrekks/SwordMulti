using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHotbarSlot : MonoBehaviour, IDropHandler
{
    public HotbarController _hotbarController;

    public GameObject hotbarSlot_;

    private Item _item;

    public GameObject insideObject;

    private void Awake()
    {
        for (int i = 0; i < _hotbarController.hotbarSlots.Count; i++)
        {
            if (this.gameObject == _hotbarController.hotbarSlots[i].gameObject)
            {
                hotbarSlot_ = this.gameObject;
            }
        }

        if (transform.childCount != 0)
        {
            insideObject = transform.GetChild(0).gameObject;   
        }
    }
    

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        
        _item = dropped.GetComponent<Item>();


        if (eventData.pointerDrag == _item.gameObject && transform.childCount < 1)
        {
            _item.afterDragParent.GetComponent<UIHotbarSlot>().insideObject = null;
            
            _item.afterDragParent = transform;

            insideObject = _item.gameObject;
        }
        
        if (eventData.pointerDrag == _item.gameObject && transform.childCount >= 1)
        {
            transform.GetChild(0).parent = _item.afterDragParent;
                
            _item.afterDragParent = transform;

            _item.firstParent.GetComponent<UIHotbarSlot>().insideObject = insideObject;

            insideObject = _item.gameObject;
        }

    }
}
