using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{

    [Header("Gameplay")]
    public TileBase tile;
    public TileClass tileClass;
    public ItemType type;
    public ActionType actionType;
    public Vector2Int range = new Vector2Int(5, 4);
    public Vector2Int laserRange = new Vector2Int(5, 4);
    public float tileHealth = 100;
    public float tileMaxHealth = 100;
    public int toolBlockDamage;
    [Header("UI")]
    public bool stackable = true;
    [Header("Both")]
    public Sprite image;
}

public enum ItemType
{
    BuildingBlock, Tool, Consumable, Weapon
}

public enum ActionType
{
    Dig, Mine, Consume, place
}

