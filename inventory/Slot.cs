using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    public int slot_id;
    public ItemLocation slot_location;
    Invenotry AccInven;
    storage AccSM;

    // Start is called before the first frame update
    void Start()
    {
        AccInven = GameObject.Find("InventoryPanel").GetComponent<Invenotry>();
        AccSM = GameObject.Find("StoragePanel").GetComponent<storage>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemProp droppeditem = eventData.pointerDrag.GetComponent<ItemProp>(); //drop하는 아이템
        //Transform item = transform.GetChild(0); //슬롯 안에 있는 아이템
        if (slot_location == ItemLocation.Inventory) // inventory일때만 
        {// invenslot -> invenslot,   storageslot -> invenslot
            Debug.Log("slot_location.inventory");
            if (droppeditem.mylocation == ItemLocation.Inventory) // invenslot -> invenslot
            {
                if (AccInven.items[slot_id] == null) // 비어있는 slot이면       //inven만 적용됨
                {// droppeditem을 비어있는곳에 넣고 원래자리를 비어줌
                    AccInven.items[slot_id] = eventData.pointerDrag;
                    droppeditem.transform.SetParent(AccInven.slots[slot_id].transform); //setparent
                    droppeditem.transform.localPosition = Vector2.zero;

                    AccInven.items[droppeditem.slotAddress] = null; //items[i]
                    droppeditem.slotAddress = slot_id; //slotaddress
                }
                else // 비어있는게 아닐때  swap해줌
                {
                    GameObject originitem = AccInven.items[slot_id];
                    GameObject dropitem = eventData.pointerDrag;
                    int originslotid = slot_id;
                    int dropslotid = droppeditem.slotAddress;

                    AccInven.items[originslotid].transform.SetParent(AccInven.slots[dropslotid].transform); //set parent
                    AccInven.items[originslotid].GetComponent<ItemProp>().slotAddress = dropslotid;  //slotaddress    
                    AccInven.items[originslotid].transform.localPosition = Vector2.zero;
                    eventData.pointerDrag.transform.SetParent(AccInven.slots[originslotid].transform);
                    dropitem.GetComponent<ItemProp>().slotAddress = originslotid;
                    AccInven.items[dropslotid] = AccInven.items[originslotid]; //items[i]
                    AccInven.items[originslotid] = dropitem;
                    droppeditem.transform.localPosition = Vector2.zero;
                }
            }
            else//storage -> invenslot ,  itemlocation을 바꿔줘야됨
            {
                if (AccInven.items[slot_id] == null) // 비어있는 slot이면 
                {// droppeditem을 비어있는곳에 넣고 원래자리를 비어줌
                    AccInven.items[slot_id] = eventData.pointerDrag;
                    droppeditem.transform.SetParent(AccInven.slots[slot_id].transform); //setparent
                    droppeditem.transform.localPosition = Vector2.zero;
                    droppeditem.mylocation = ItemLocation.Inventory;
                    AccInven.items[droppeditem.slotAddress] = null; //items[i]
                    droppeditem.slotAddress = slot_id; //slotaddress
                }
                else // 비어있는게 아닐때
                {
                    //storage ->inventory는 안됨
                }
            }
        }
        else if(slot_location == ItemLocation.Storage)  //else  storage일때 자리바꾸기 안되게 설정
        {

        }
    }
}
