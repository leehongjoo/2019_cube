using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputPanelManager : MonoBehaviour
{
    public InputField inputfield;
    public ItemProp itemprop;
    Vector2 originalpos;
    int inputint;
    int maxamount;

    private void Start()
    {
        originalpos = transform.position;
    }

    public void CallInputPanel(Vector2 pos, ItemProp itemp)
    {
        transform.position = pos;
        itemprop = itemp;
        maxamount = itemprop.the_number;
    }
    public void Clickenterbutton()
    {
        Debug.Log("click");
        if(inputfield.text == "")
        {
            inputint = 1;
        }
        else
        {
            inputint = int.Parse(inputfield.text);
        }
        if (itemprop.mylocation == ItemLocation.Inventory)
        {
            if(inputint <= maxamount)
            {
                itemprop.FromInventoryToStorage(inputint);
            }
            else
            {
                itemprop.FromInventoryToStorage(maxamount);
            }
        }
        else if (itemprop.mylocation == ItemLocation.Storage)
            itemprop.FromStorageToInventory(inputint);
        //else?
        transform.position = originalpos;
    }
    public void maxbutton()
    {
        inputfield.text = maxamount.ToString();
        inputfield.placeholder.GetComponent<Text>().text = maxamount.ToString();
    }
}
