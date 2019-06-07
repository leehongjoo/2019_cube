using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> database = new List<Item>();
    private void Awake()
    {
        database = new List<Item>(Resources.LoadAll<Item>("ItemBases"));
    }
    public Item FetchItem(int id)
    {
        for(int i=0; i < database.Count; i++)
        {
            if (database[i].item_id == id)
                return database[i];
        }
        return null;
    }
}
