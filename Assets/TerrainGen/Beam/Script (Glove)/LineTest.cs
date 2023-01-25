using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LineTest : MonoBehaviour
{
    public LineRenderer line;
    public Transform player;
    [SerializeField] float checkRange;
    public Vector3 mousePos;

    public bool toggleOn;

    public LayerMask testLayer;

    // Start is called before the first frame update
    void Start()
    {
        toggleOn = false;
    }

    public void ActivateGlove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            toggleOn = true;
            OnMouseDown();
            Debug.Log(mousePos.x);
            Debug.Log(mousePos.y);
        }
        if (context.canceled)
        {
            toggleOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleOn) 
        {
            mousePos = Input.mousePosition;
            
            line.SetPosition(0, player.position);
            line.SetPosition(1, mousePos);
        }

      

        /* Collider[] block = Physics.OverlapSphere(transform.position, checkRange, testLayer);
         for (int i = 0; i < block.Length; i++)
         {

         }*/


    }
    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        //Destroy(gameObject);
    }
}
