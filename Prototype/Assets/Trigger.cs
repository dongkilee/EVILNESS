using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.GetComponentInParent<EnemySpawn>().OnTriggerEnemySpawn();
            Destroy(this.gameObject);
        }
    }
}
