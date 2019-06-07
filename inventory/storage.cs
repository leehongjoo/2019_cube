using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class storage : MonoBehaviour
{
    public GameObject SlotParent;
    public int slotamount;
    Vector3 originalposition;
    public List<GameObject> storageitems = new List<GameObject>();
    public List<GameObject> storageslots = new List<GameObject>();

    GameObject storageitem;
    GameObject storageslot;
    public bool OnStorage;
    public bool isstorage;

    private ItemDatabase database;

    private void Start()
    {
        originalposition = transform.position;
        storageitem = Resources.Load<GameObject>("prefabs/item") as GameObject;
        storageslot = Resources.Load<GameObject>("prefabs/slot") as GameObject;
        database = GameObject.Find("Database").GetComponent<ItemDatabase>();
        for(int i = 0; i<slotamount; i++)
        {
            storageitems.Add(null);
        }
        AddStorage();
        Additem(1, 1);
        Additem(1, 1);
        Additem(1, 1);
        Additem(0, 2);
        Additem(0, 2);
    }

    public void AddStorage()
    {
        for(int i=0; i<slotamount; i++)
        {
            storageslots.Add(Instantiate(storageslot));
            storageslots[i].transform.name = i.ToString();
            storageslots[i].transform.SetParent(SlotParent.transform);
            storageslots[i].GetComponent<Slot>().slot_id = i;
            storageslots[i].GetComponent<Slot>().slot_location = ItemLocation.Storage;
        }
    }
    public void Additem(int id, int number)
    {
        Item itemtoadd = database.FetchItem(id);
        if (CheckEqualtem(itemtoadd) && itemtoadd.IsStackable) //같은아이템 있고 그아이템이 스택쌓을수있으면
        {// 숫자만 늘려주면됨
            for (int i = 0; i < storageslots.Count; i++) // 다시 같은아이템 찾아줌
            {
                if (storageitems[i] != null)
                {
                    ItemProp data = storageitems[i].GetComponent<ItemProp>();
                    if (data.myitem.item_id == id) //같은 아이템 찾으면
                    {
                        data.the_number += number; //+1하고
                        data.GetComponentInChildren<Text>().text = data.the_number.ToString(); //text 업데이트
                        break;
                    }
                }
            }
        }
        else if (!itemtoadd.IsStackable) //스택쌓을수없는경우
        {
            for (int i = 0; i < slotamount; i++)
            {
                if (storageitems[i] == null)
                {
                    GameObject itemobj = Instantiate(storageitem);
                    ItemProp prop = itemobj.GetComponent<ItemProp>();
                    itemobj.transform.SetParent(storageslots[i].transform);
                    prop.myitem = itemtoadd;
                    prop.the_number = 1; //무조곤 한개임
                    itemobj.GetComponentInChildren<Text>().text = "";
                    prop.mylocation = ItemLocation.Storage; //storage
                    prop.slotAddress = i; //뺴도될듯
                    itemobj.transform.position = Vector2.zero;
                    itemobj.transform.localPosition = Vector2.zero;
                    itemobj.GetComponent<Image>().sprite = itemtoadd.item_icon;
                    storageitems[i] = itemobj;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < slotamount; i++)
            {
                if (storageitems[i] == null)
                {
                    GameObject itemobj = Instantiate(storageitem);
                    ItemProp prop = itemobj.GetComponent<ItemProp>();
                    itemobj.transform.SetParent(storageslots[i].transform);
                    prop.myitem = itemtoadd;
                    prop.the_number = number;
                    prop.mylocation = ItemLocation.Storage; // 위치느 storage
                    prop.slotAddress = i; //뺴도될듯
                    if (prop.the_number > 1)
                        itemobj.GetComponentInChildren<Text>().text = prop.the_number.ToString();
                    itemobj.transform.position = Vector2.zero;
                    itemobj.transform.localPosition = Vector2.zero;
                    itemobj.GetComponent<Image>().sprite = itemtoadd.item_icon;
                    
                    storageitems[i] = itemobj;
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
            for (int i = 0; i < storageslots.Count; i++)
            {
                if (storageitems[i] != null)
                {
                    ItemProp data = storageitems[i].GetComponent<ItemProp>();
                    if (data.myitem.item_id == id)
                    {
                        data.the_number -= number;
                        if (data.the_number <= 0)
                        {
                            Destroy(data.gameObject);
                            storageitems.RemoveAt(i);
                            storageitems.Insert(i, null);
                            /*if (checkItemRemove(itemtominus))
                            {
                                removeitem(itemtominus);
                            }*/
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
        for (int i = 0; i < storageslots.Count; i++)// 
        {
            if (storageitems[i] != null) //item리스트에 뭔가있으면 하나하나 비교해봄
            {
                if (storageitems[i].GetComponentInChildren<ItemProp>().myitem.item_id == item.item_id) //id가 같으면 같은 물건
                    return true;
            }
        }
        return false;
    }
    public void callstorage()
    {
        OnStorage = true;
        transform.position = new Vector2(200, 230);
    }
    public void returnposition()
    {
        transform.position = originalposition;
        OnStorage = false;
    }
}
