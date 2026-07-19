using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnDino : MonoBehaviour
{
    public GameObject creature;
    public Transform spawnPoint;


    void Spawn()
    {
        GameObject newProj = Instantiate(creature, spawnPoint.position, spawnPoint.rotation);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Spawn();
            Destroy(this.gameObject);
        }
    }


}
