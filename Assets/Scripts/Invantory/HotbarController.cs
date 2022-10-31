using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarController : NetworkBehaviour
{
    private KeyCode[] _hotbarKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

    public List<UIHotbarSlot> hotbarSlots = new List<UIHotbarSlot>();

    public Item selectedItem;

    public List<GameObject> _itemsPrefab = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            _itemsPrefab.Add(hotbarSlots[i].insideObject.GetComponent<Item>().selecetedGameobject);
        }
    }
    
    public  void Update()
    {
        if (!Object.HasStateAuthority)
        {
            return;
        }
        
        if (Input.GetKeyDown(_hotbarKeys[0]))
        {
            ChangeWeapon(0);
        }
        
        if (Input.GetKeyDown(_hotbarKeys[1]))
        {
            ChangeWeapon(1);
        }
        
        if (Input.GetKeyDown(_hotbarKeys[2]))
        {
            ChangeWeapon(2);
        }
        
        if (Input.GetKeyDown(_hotbarKeys[3]))
        {
            ChangeWeapon(3);
        }
    }
    

    private void ChangeWeapon(int i)
    {
        Rpc_ChangeWeapon(i);
    }

    [Rpc]
    public void Rpc_ChangeWeapon(int i)
    {
        if (hotbarSlots[i].insideObject != null)
        {
            GameObject item = hotbarSlots[i].insideObject;

            selectedItem = item.GetComponent<Item>();

            selectedItem.selecetedGameobject.gameObject.SetActive(true);

            SetActiveFalseItem();
        }
    }
    private void SetActiveFalseItem()
    {
        for (int j = 0; j < hotbarSlots.Count - 1; j++)
        {
            if (selectedItem.selecetedGameobject != _itemsPrefab[j].gameObject)
            {
                _itemsPrefab[j].gameObject.SetActive(false);
            }
        }
    }
}
