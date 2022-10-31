using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public enum ItemType
{
    Fist,
    Sword,
    Pot
}

public class Item : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public GameObject[] itemPrefab; //3d item gameobject
    
    public GameObject[] refPrefab; //hot bar icon gameobject

    public GameObject selecetedGameobject; //oyuncunun elindeki 3d silahi gosteriyor
    
    public GameObject refSelecetedGameobject; //secili olan ui itemini gosteriyor

    public Transform afterDragParent; //suruklendikten sonra itemin hangi slotun childi olacagini belirliyor
    
    public Transform firstParent; //suruklenmeden onceki parenti tutuyor
    
    public ItemType itemType; //turune gore degisken tutuyor

    private UIHotbarSlot _hotbarSlot;

    private void Awake()
    {
        ItemSelect();
        
        _hotbarSlot = GetComponentInParent<UIHotbarSlot>();
       
        firstParent = refSelecetedGameobject.transform.parent;
    }


    public void ItemSelect()
    {
        switch(itemType)
        {
            case ItemType.Sword: 
                selecetedGameobject = itemPrefab[0];
                refSelecetedGameobject = refPrefab[0];
                break;
                
            case ItemType.Pot:
                selecetedGameobject = itemPrefab[1];
                refSelecetedGameobject = refPrefab[1];
                 break;
                
            case ItemType.Fist:
                selecetedGameobject = itemPrefab[2];
                refSelecetedGameobject = refPrefab[2];
                break;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        firstParent = refSelecetedGameobject.transform.parent;
        afterDragParent = refSelecetedGameobject.transform.parent;
        refSelecetedGameobject.transform.SetParent(transform.parent.parent);
        refSelecetedGameobject.transform.GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        refSelecetedGameobject.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        refSelecetedGameobject.transform.SetParent(afterDragParent);
        refSelecetedGameobject.transform.GetComponent<Image>().raycastTarget = true;
    }
    
}

