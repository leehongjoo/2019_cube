using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trap : MonoBehaviour
{
    public GameObject explosion;
    PlayerController pct;
    private void Start()
    {
        pct = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void Update()
    {
    
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "item")
        {
            Instantiate(explosion, transform.position, transform.rotation);
            
            if(other.gameObject.tag == "Player")
                pct.health = 0;
        }
    }
}
