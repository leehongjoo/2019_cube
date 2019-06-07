using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itempickup : MonoBehaviour
{
    GameObject picktxt;
    bool ispick;
    PlayerController pct;
    private void Start()
    {
        pct = GameObject.Find("player").GetComponent<PlayerController>();
        picktxt = GameObject.Find("PlayerCanvas").transform.GetChild(1).gameObject;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")) // can pick up
        {
            picktxt.SetActive(true);
            if(Input.GetKeyDown(KeyCode.E))
            {
                //줍는 animation 시작하고 인벤토리 add해주고 파괴
                picktxt.SetActive(false);
                int pickid = int.Parse(transform.name);
                pct.Pickup(pickid);
                Destroy(gameObject);

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        picktxt.SetActive(false);
    }
}
