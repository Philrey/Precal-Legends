using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyEventReceiver : MonoBehaviour
{
    public void takeHit() {
        Debug.Log("Take Hit Received");
        gameObject.GetComponentInParent<enemy_controller>().hitTarget();
    }
}
