using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;
using System;

public class BuildingSystem : MonoBehaviour
{
    LaserHit lh;
    public Camera mainCamera;
    LaserScript ls;
    [SerializeField] private TileBase highlightTile;
    [SerializeField] private Tilemap[] mainTilemap;
    [SerializeField] private Tilemap highlightTilemap;
    [SerializeField] private GameObject lootPrefab;
    TerrainGeneration tGen;


    private Vector3Int playerPos;
    private Vector3Int highlightedTilePos;
    private bool highlighted;
    private bool isMouseButtonDown = false;
    public  Item item;
    public bool insideRange;
    public bool toolInHand;
    bool mining;
    bool miningDelay;
    private void Start()
    {
        
        lh = FindObjectOfType<LaserHit>();
        ls = FindObjectOfType<LaserScript>();
        tGen = FindObjectOfType<TerrainGeneration>();
    }
    void Update(){
        item = InventoryManager.instance.GetSelectedItem(false);
        playerPos = mainTilemap[0].WorldToCell(transform.position);
        if(item != null){
            HighlightTile(item);
        }
        else{
            highlighted = false;
        }
        if (isMouseButtonDown && item != null)
        {
            UpdatePos(item);
        }
        if(!isMouseButtonDown)
        { 
                mining = false;
        }
    }
    void OnClick(InputValue action)
    {
        if (action.isPressed)
        {
            isMouseButtonDown = true;
        }
        else
        {
            isMouseButtonDown = false;

        }
    }


    void FixedUpdate()
    {

        if (isMouseButtonDown)
        {
            if (item.type == ItemType.BuildingBlock && highlighted)
            {
                build(highlightedTilePos.x,highlightedTilePos.y, item);
            }
            else if (item.type == ItemType.Tool && !mining && !ls.jetPackActive)
            {
                //highlightedTilePos.x, highlightedTilePos.y
                StartCoroutine(MineTile((int)ls.laserHitVector.x, (int)ls.laserHitVector.y));
                //Destroy(highlightedTilePos);
            }

        }
    }
    private void UpdatePos(Item currentItem)
    {

            Vector3Int mouseGridPos = GetMouseOnGridPos();
           if (InRange(playerPos, mouseGridPos, (Vector3Int)currentItem.laserRange))
        {
            insideRange = true;

        }
        else
        {
            insideRange = false;

        }
            
        
        
    }

    private Vector3Int GetMouseOnGridPos(){
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCellPos = mainTilemap[0].WorldToCell(mousePos);
        mouseCellPos.z = 0;
        //Debug.Log("mousePos: " + mousePos);
        //Debug.Log("mouseCellPos: "+ mouseCellPos);
        return mouseCellPos;
    }

    private void HighlightTile (Item currentItem)
    {
        Vector3Int mouseGridPos = GetMouseOnGridPos();

        if (highlightedTilePos != mouseGridPos){
            highlightTilemap.SetTile(highlightedTilePos, null);
            for (int i = 0; i < mainTilemap.Length; i++)
            {
                if (InRange(playerPos, mouseGridPos, (Vector3Int)currentItem.range))
                {
                    if (CheckCondition(mainTilemap[i].GetTile<CustomTile>(mouseGridPos), currentItem))
                    {
                        highlightTilemap.SetTile(mouseGridPos, highlightTile);
                        highlightedTilePos = mouseGridPos;
                        highlighted = true;
                    }
                    else
                    {
                        highlighted = false;
                    }
                }
                else
                {
                    highlighted = false;
                }

            }
            
        }
    }

    private bool InRange(Vector3Int positionA, Vector3Int positionB, Vector3Int range){
        Vector3Int distance = positionA - positionB;

        if (Mathf.Abs(distance.x) >= range.x || Mathf.Abs(distance.y) >= range.y){
            return false;
              
        }
        return true;
    }

    private bool CheckCondition(CustomTile tile, Item currentItem){   
        if (currentItem.type == ItemType.BuildingBlock){
            if (!tile){
                return true;
            }
            
        }
        else if (currentItem.type == ItemType.Tool){
            if (tile){
                return true;
            }
        }
        return false;
    }

    private void build(int x, int y, Item itemToBuild){
        if(tGen.GetTile(0, x, y) == null && tGen.GetTile(3, x, y) == null)
        {  
        
        InventoryManager.instance.GetSelectedItem(true);
        highlightTilemap.SetTile(highlightedTilePos, null);
        
        if (itemToBuild.tileClass.isIlluminate)
            tGen.PlaceTile(itemToBuild.tileClass, x, y, true);
        else
            tGen.PlaceTile(itemToBuild.tileClass, x, y);
        }
        else
            return;
    }

    private void Destroy(Vector3Int position){
        
        StartCoroutine(MineTile(position.x,position.y));
    }
    IEnumerator MineTile(int x, int y) {
        mining = true;
        Vector3Int position = new Vector3Int((int)ls.laserHitVector.x,(int) ls.laserHitVector.y);
        for (int i = 0; i < mainTilemap.Length; i++)
        {
            CustomTile tile = mainTilemap[i].GetTile<CustomTile>(position);
            var heldItem = InventoryManager.instance.GetSelectedItem(false);
            if (tile != null)
            {

                while (mining)
                {

                    if (!isMouseButtonDown)
                    {
                        tile.item.tileHealth = tile.item.tileMaxHealth;
                        mining = false;
                        StopAllCoroutines();
                    }
                    if (tile.item.tileHealth > 0 && !miningDelay)
                    {
                        miningDelay = true;
                        yield return new WaitForSeconds(0.1f);
                        tile.item.tileHealth -= heldItem.toolBlockDamage;
                        miningDelay = false;

                    }
                    else if (tile.item.tileHealth <= 0)
                    {
                        if (tile.tileClass.isIlluminate)
                            tGen.RemoveTile(tile.tileClass.tileLayer, x, y, true);
                        else
                            tGen.RemoveTile(tile.tileClass.tileLayer, x, y);

                        Vector3 pos = mainTilemap[i].GetCellCenterWorld(position);
                        GameObject loot = Instantiate(lootPrefab, pos, Quaternion.identity);
                        loot.GetComponent<Loot>().Initialize(tile.item);
                        highlightTilemap.SetTile(position, null);
                        highlighted = false;
                        tile.item.tileHealth = tile.item.tileMaxHealth;
                        mining = false;

                    }
                    else
                        yield return new WaitForSeconds(0.2f);


                }
            }

        }
        

   
    }

}
