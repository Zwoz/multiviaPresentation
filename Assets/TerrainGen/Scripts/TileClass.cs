using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    [field: SerializeField] public TileBase tile { get; private set; }
    public Enums.TileLayer tileLayer;
    [field: SerializeField] public bool isIlluminate { get; private set; }
}
