using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Invenotry : MonoBehaviour
{
    public GameObject SlotsParent;
    public int slotamount;

    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> slots = new List<GameObject>();

    public GameObject inventoryitem;
    public GameObject inventoryslot;
    ItemDatabase database;

    private void Start()
    {
        inventoryitem = Resources.Load<GameObject>("prefabs/item") as GameObject;
        inventoryslot = Resources.Load<GameObject>("prefabs/slot") as GameObject;
        database = GameObject.Find("Database").GetComponent<ItemDatabase>();
        for(int i=0; i<slotamount; i++)
        {
            items.Add(null);
        }
        AddInventory();
        Additem(1, 1);
        Additem(1, 1);
        Additem(1, 1);
        Additem(0, 2);
        Additem(0, 2);
    }

    public void AddInventory()
    {
        for(int i=0; i < slotamount; i++)
        {
            slots.Add(Instantiate(inventoryslot));
            slots[i].transform.name = i.ToString();
            slots[i].transform.SetParent(SlotsParent.transform);
            slots[i].GetComponent<Slot>().slot_id = i;
            slots[i].GetComponent<Slot>().slot_location = ItemLocation.Inventory;
        }
    }
    public void Additem(int id, int number)
    {
        Item itemtoadd = database.FetchItem(id);
        if(CheckEqualtem(itemtoadd) && itemtoadd.IsStackable) //같은아이템 있고 그아이템이 스택쌓을수있으면
        {// 숫자만 늘려주면됨
            for(int i=0; i<slots.Count; i++) // 다시 같은아이템 찾아줌
            {
                if(items[i] != null)
                {
                    ItemProp data = items[i].GetComponent<ItemProp>();
                    if(data.myitem.item_id  == id) //같은 아이템 찾으면
                    {
                        data.the_number += number; //+1하고
                        data.GetComponentInChildren<Text>().text = data.the_number.ToString(); //text 업데이트
                        break;
                    }
                }
            }
        }
        else if(!itemtoadd.IsStackable) //같은아이템잇지만 스택쌓을수없는경우
        {
            for (int i = 0; i < slotamount; i++)
            {
                if (items[i] == null)
                {
                    GameObject itemobj = Instantiate(inventoryitem);
                    ItemProp prop = itemobj.GetComponent<ItemProp>();
                    itemobj.transform.SetParent(slots[i].transform);
                    prop.myitem = itemtoadd;
                    prop.the_number = 1; //무조곤 한개
                    itemobj.GetComponentInChildren<Text>().text = "";
                    prop.mylocation = ItemLocation.Inventory;
                    prop.slotAddress = i;
                    itemobj.transform.position = Vector2.zero;
                    itemobj.transform.localPosition = Vector2.zero;
                    itemobj.GetComponent<Image>().sprite = itemtoadd.item_icon;
                    items[i] = itemobj;
                    break;
                }
            }
        }
        else
        {
            for(int i =0; i<slotamount; i++)
            {
                if(items[i] == null)
                {
                    GameObject itemobj = Instantiate(inventoryitem);
                    ItemProp prop = itemobj.GetComponent<ItemProp>();
                    itemobj.transform.SetParent(slots[i].transform);
                    prop.myitem = itemtoadd;
                    prop.the_number = number;
                    prop.mylocation = ItemLocation.Inventory;
                    prop.slotAddress = i;
                    if (prop.the_number > 1)
                        itemobj.GetComponentInChildren<Text>().text = prop.the_number.ToString();
                    itemobj.transform.position = Vector2.zero;
                    itemobj.transform.localPosition = Vector2.zero;
                    itemobj.GetComponent<Image>().sprite = itemtoadd.item_icon;
                    items[i] = itemobj;
                    break;
                }
            }
        }
    }
    public void minusitem(int id, int number)
    {
        Item itemtominus = database.FetchItem(id);
        if (CheckEqualtem(itemtominus))
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (items[i] != null)
                {
                    ItemProp data = items[i].GetComponent<ItemProp>();
                    if (data.myitem.item_id == id)
                    {
                        data.the_number -= number;
                        if (data.the_number <= 0)
                        {
                            Destroy(data.gameObject);
                            items.RemoveAt(i);
                            items.Insert(i, null);
                        }
                        else
                            data.GetComponentInChildren<Text>().text = data.the_number.ToString();
                        break;
                    }
                }
            }
        }
    }
    bool CheckEqualtem(Item item)
    {
        for(int i=0; i< slots.Count; i++)// 
        {
            if(items[i] != null) //item리스트에 뭔가있으면 하나하나 비교해봄
            {
                if (items[i].GetComponentInChildren<ItemProp>().myitem.item_id == item.item_id) //id가 같으면 같은 물건
                    return true;
            }
        }
        return false;
    }

}
