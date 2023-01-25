using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "MobAtlas", menuName = "Multivia/MobStuff/MobAtlas")]
public class MobAtlas : ScriptableObject
{
    [Header("Mobs")]
    public AIClass shroom;
    public AIClass worm;
    public AIClass boss;

}
