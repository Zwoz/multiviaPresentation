using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "liquid tile", menuName = "LiquidTile")]
public class LiquidTileClass : TileClass
{
    [field: Tooltip("Speed in secs the liquid flows")]
    [field: SerializeField] public float flowSpeed { get; private set; }


}
