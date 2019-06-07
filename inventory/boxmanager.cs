using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxmanager : MonoBehaviour
{
    public storage st;
    bool storage_on;
    // Update is called once per frame

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                storage_on = !storage_on;
                if (storage_on)
                {
                    st.callstorage();
                }
                else
                {
                    st.returnposition();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        st.returnposition();
        storage_on = false;
    }
}
