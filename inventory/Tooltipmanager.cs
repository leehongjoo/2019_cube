using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tooltipmanager : MonoBehaviour
{
    Vector3 originalpos;
    Text descriptiontxt;
    Vector2 pos;
    private void Start()
    {
        pos = new Vector2(320, 100);
        originalpos = transform.position;
        descriptiontxt = transform.GetComponentInChildren<Text>();
    }
    public void Itemshow(ItemProp itemgo)
    {
        Item item = itemgo.myitem;
        descriptiontxt.text = item.Description;
    }
    public void Calltooltip(ItemProp item)
    {
        Itemshow(item);
        transform.position = pos;
    }
    public void hidetooltip()
    {
        transform.position = originalpos;
    }
}
