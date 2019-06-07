using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ItemProp : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Item myitem;
    public Image myimage;
    public int slotAddress;
    public int the_number;
    public ItemLocation mylocation;
    private Transform originalparent;
    Invenotry AccInven;
    storage AccSM;
    Tooltipmanager Acctt;
    InputPanelManager Accim;
    PlayerController pct;

    public Transform equip_point;
    void Start()
    {
        pct = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        equip_point = GameObject.FindGameObjectWithTag("equip_point").transform;
        AccInven = GameObject.Find("InventoryPanel").GetComponent<Invenotry>();
        AccSM = GameObject.Find("StoragePanel").GetComponent<storage>();
        Acctt = GameObject.Find("tooltip").GetComponent<Tooltipmanager>();
        Accim = GameObject.Find("inputpanel").GetComponent<InputPanelManager>();
    }
    public void MouseOn() //event trigger로 해봄
    {
        Acctt.Calltooltip(this);  // 툴팁정보를 넘겨주고 위치로 이동
    }
    public void MouseOut() //unity에 event trigger 추가
    {
        Acctt.hidetooltip();  //툴팁 숨김
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalparent = transform.parent;  //parent 저장
        if(myitem != null)
        {
            transform.SetParent(transform.parent.parent.parent.parent); // item -> slot -> slotparent -> Inventory or Storage Panel
            transform.position = eventData.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData) // 마우스 움직임대로
    {
        if(myitem !=null)
        {
            transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(transform.parent.name == "PlayerCanvas") // inven or storage Panel -> PlayerCanvas
        {
            transform.SetParent(originalparent); // 원래 parent로
            transform.localPosition = Vector2.zero;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (mylocation == ItemLocation.Inventory && !AccSM.OnStorage) // item is in the inven
            {
                Acctt.hidetooltip();
                Debug.Log("consume");
                ConsumeItem();
            }
            else if (mylocation == ItemLocation.Inventory && AccSM.OnStorage)
            {
                if (myitem.IsStackable)
                {
                    Accim.CallInputPanel(eventData.position, this);
                    Acctt.hidetooltip();
                }
                else
                {
                    FromInventoryToStorage(1);
                    Acctt.hidetooltip();
                }
            }
            else if (mylocation == ItemLocation.Storage)
            {
                if (myitem.IsStackable)
                {
                    Accim.CallInputPanel(eventData.position, this);
                    Acctt.hidetooltip();
                }
                else
                {
                    FromStorageToInventory(1);
                    Acctt.hidetooltip();
                }
            }
            //else if itemHOme == Equipped
        }
    }
    void ConsumeItem()
    {
        AccInven.minusitem(myitem.item_id, 1);
        GameObject pickupitem = Instantiate(myitem.item_gameobject,equip_point);
        pickupitem.name = myitem.item_id.ToString();
        pickupitem.transform.localPosition = Vector3.zero;
        pickupitem.transform.rotation = new Quaternion(0, 0, 0, 0);
        pct.Setequip(pickupitem, true); 
    }
    void FromInventoryToEquip() // inven -> equip
    {
        Debug.Log("FromInventoryToEquip");
    }
    void FromEquipToInventory() // equip -> inven
    {
        Debug.Log("FromEquipToInventory");
    }
    public void FromInventoryToStorage(int i)
    {
        Debug.Log(" FromInventoryToStorage");
        AccInven.minusitem(myitem.item_id, i);
        AccSM.Additem(myitem.item_id, i);
    }
    public void FromStorageToInventory(int i)
    {
        AccSM.minusitem(myitem.item_id, i);
        AccInven.Additem(myitem.item_id, i);
    }
}
public enum ItemLocation
{
    Inventory = 0, Dropped = 2, Equipped =3, Storage =4
}

