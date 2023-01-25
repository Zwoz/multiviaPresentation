using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSprite : MonoBehaviour
{
    TerrainGeneration tGen;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {

        tGen = GameObject.FindObjectOfType<TerrainGeneration>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        /*        if (this.name == "rCol")
                {*/
        Debug.Log("dddd");
           // tGen.ChangeSprite(tGen.tileAtlas.dirt.tileSprites);
        /*}*/
        
    }

}
