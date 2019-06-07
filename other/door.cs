using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    Animator ani;
    public GameObject pressRtxt;
    bool ispress;
    private void Start()
    {
        ani = transform.parent.parent.GetChild(0).GetComponent<Animator>();
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!ispress)
                pressRtxt.SetActive(true);
            if(Input.GetKeyUp(KeyCode.R) && !ispress) // 이미 열린상태에서는 또 못누름
            {
                ispress = true;
                pressRtxt.SetActive(false);
                ani.Play("door_open");
                Invoke("door_close", 5f);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        pressRtxt.SetActive(false);
    }

    void door_close()
    {
        ani.Play("door_close");
        ispress = false;
    }

}
