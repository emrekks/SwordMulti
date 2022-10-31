using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Sword : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent(out ThirdPersonMovement player))
        {
            if(player != this)
            {
                if (Object.HasStateAuthority)
                {
                    player.gameObject.GetComponent<ThirdPersonMovement>().GetDamage(20);
                }
            }
        }
    }
}
