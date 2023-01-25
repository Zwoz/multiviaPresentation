using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

public class LaserScript : MonoBehaviour
{
    [SerializeField] bool toggleOn;
    public bool jetPackActive;
    public Camera cam;
    public float castDistance = 1.0f;
    public Tilemap map;
    [SerializeField]Collider2D jetPackCol;
    public LineRenderer lineRenderer;
    public Transform glovePos;
    public GameObject startVFX;
    public GameObject endVFX;
    [SerializeField] GameObject objectTierOne;

    public Transform rayCastPoint;
    private Quaternion rotation;
    [SerializeField] LayerMask layerMask;
    [SerializeField] List<ParticleSystem> particales;
    public LaserTier state;
    public int gloveTier = 1;
    Vector2 mousePos;
    private Item item;
    public Vector2 laserHitVector;
    public Vector2 endpos;
    public enum LaserTier
    {
        TierOne,
        TierTwo,
        TierThree,
        TierFour
    }

    // Start is called before the first frame update
    void Start()
    {
        FillLists();
        for (int i = 0; i < particales.Count; i++)
        {
            particales[i].Stop();

        }
        DisableLaser();

        toggleOn = false;

    }


    public void DebugSwap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FillLists();
        }

    }
    void OnClick(InputValue action)
    {

        if (action.isPressed)
        {

            toggleOn = true;
            if (gloveTier == 1 && item != null)
            {
                if (item.type == ItemType.Tool)
                {
                    lineRenderer.enabled = true;
                    for (int i = 0; i < particales.Count; i++)
                    {
                        particales[i].Play();

                    }
                }

            }
            if (jetPackActive)
            {
                jetPackCol.enabled = true;
            }
            else
                jetPackCol.enabled = false;



        }
        else
        {
            toggleOn = false;
            if (gloveTier == 1)
            {
                lineRenderer.enabled = false;
                for (int i = 0; i < particales.Count; i++)
                {
                    particales[i].Stop();

                }
            }
            jetPackCol.enabled = false;

        }

    }
    void OnJetpack(InputValue action)
    {
        if (action.isPressed && jetPackActive)
        {
            jetPackActive = false;
        }
        else if (action.isPressed && !jetPackActive)
        {
            jetPackActive = true;
        }
    }
    public void DeactivateLaser()
    {
        objectTierOne.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        item = InventoryManager.instance.GetSelectedItem(false);
        if (toggleOn && item != null)
        {
            if (item.type == ItemType.Tool)
            {
                EnableLaser();

                UpdateLaser();

            }


            RotateToMouse();

  
            if (gloveTier == 1)
            {
                objectTierOne.SetActive(true);
            }
            else
            {
                objectTierOne.SetActive(false);
            }
        }

        else if (!toggleOn)
        {
            DisableLaser();
        }

    }

    void EnableLaser()
    {
        if (gloveTier == 1)
        {
            lineRenderer.enabled = true;

        }
        else
        {
            lineRenderer.enabled = false;
        }

    }

    void UpdateLaser()
    {
        // using old input system just for this, I cba checking how to do it with the new one
        if (gloveTier == 1)
        {
            item = InventoryManager.instance.GetSelectedItem(false);
            lineRenderer.SetPosition(0, (Vector2)glovePos.position);
            startVFX.transform.position = (Vector2)glovePos.position;
            lineRenderer.SetPosition(1, mousePos);
            Vector3 direction = mousePos - (Vector2)transform.position;
            Vector3 direction2 = mousePos - (Vector2)transform.position;
            RaycastHit2D hit = Physics2D.Raycast(glovePos.position, direction2.normalized, direction2.magnitude, layerMask);
            RaycastHit2D hit2 = Physics2D.Raycast(glovePos.position, direction, castDistance, layerMask);
            if (hit)
            {
                lineRenderer.SetPosition(1, hit.point);
                Vector2 modifiedHitPoint = hit.point;
                float angle = Vector2.SignedAngle(new Vector2(0.5f, -0.5f), direction);
                //print(angle);
                //print(tilePos);
                if (angle < 0)
                {
                    modifiedHitPoint.x -= 0.1f;
                    modifiedHitPoint.y -= 0.1f;
                }
                Vector3Int tile = map.layoutGrid.WorldToCell(modifiedHitPoint);
                Vector3 tilePos = map.layoutGrid.GetCellCenterLocal(tile);
                endpos = glovePos.position + direction;
                laserHitVector = tilePos;
                Debug.DrawRay((Vector2)glovePos.position, direction, Color.green);
                
            }
            endVFX.transform.position = lineRenderer.GetPosition(1);
        }

    }

    void DisableLaser()
    {
        lineRenderer.enabled = false;
        if (gloveTier == 1)
        {
            lineRenderer.enabled = false;
            for (int i = 0; i < particales.Count; i++)
            {
                particales[i].Stop();

            }
        }

    }

    void RotateToMouse()
    {
        Vector2 direction = cam.ScreenToWorldPoint(Input.mousePosition) - glovePos.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotation.eulerAngles = new Vector3(0, 0, angle);
        glovePos.rotation = rotation;
    }

    void FillLists()
    {
        if (gloveTier == 1)
        {
            for (int i = 0; i < startVFX.transform.childCount; i++)
            {
                var ps = startVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    particales.Add(ps);
                }
            }

            for (int i = 0; i < endVFX.transform.childCount; i++)
            {
                var ps = endVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    particales.Add(ps);
                }
            }
        }
 
    }

}
