using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement owner;
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent(out ThirdPersonMovement player))
        {
            if(player != owner)
            {
                Debug.Log(player.gameObject.name);
                owner.Attacked(player.Object.StateAuthority);
            }
        }
    }
}
