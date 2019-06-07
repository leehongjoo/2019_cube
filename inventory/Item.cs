using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Item : ScriptableObject
{
    public string item_name;
    public Sprite item_icon;
    public GameObject item_gameobject;
    public int item_damage;
    public int item_id;
    [TextArea(3, 10)]
    public string Description;
    [TextArea(3, 10)]
    public string item_effect;
    public bool IsStackable;

}
