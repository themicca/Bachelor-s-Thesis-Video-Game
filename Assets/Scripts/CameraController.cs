using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public int movementSpeed = 5;
    public int movementTime = 5;
    public float dragSpeed = 0.5f;
    public Vector3 zoomAmount;
    public float minZoom = 1;
    public float maxZoom = 6;
    public float speedZoom = 30;
    float targetZoom;
    int screenWidth, screenHeight;
    float posX, posY, posZ;

    Vector3 previousMousePosition;

    Vector3 dragOrigin;
    void Start()
    {
        previousMousePosition = Input.mousePosition;
        targetZoom = Camera.main.orthographicSize;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        posX = transform.position.x;
        posY = transform.position.y;
        posZ = transform.position.z;
    }

    void Update()
    {
        HandleMovementInput();
        HandleMouseInput();
    }

    private Collider2D GetRayTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        return hit2D.collider;
    }

    void HandleMouseInput() 
    {
        // zoom
        targetZoom -= Input.mouseScrollDelta.y;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        float newSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetZoom, speedZoom * Time.deltaTime);
        Camera.main.orthographicSize = newSize;
        /*if (Input.mouseScrollDelta.y != 0 && newSize > minZoom && newSize < maxZoom)
        {
            Collider2D collider = GetRayTarget();
            Vector3 collPos = new (0,0,0);
            if (collider != null)
            {
                collPos = collider.transform.position;
            } else
            {
                collPos.x = Mathf.Clamp(Input.mousePosition.x, 0, Settings.gridSizeX - 2);
                collPos.y = Mathf.Clamp(Input.mousePosition.y, 0, Settings.gridSizeY - 4);
            }
            transform.position = collPos;
        }*/
            

        // mouse edge
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        posX = transform.position.x;
        posY = transform.position.y;

        if (Input.mousePosition.x >= screenWidth - 5)
        {
            posX += movementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= 0)
        {
            posX -= movementSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y >= screenHeight - 5)
        {
            posY += movementSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y <= 0)
        {
            posY -= movementSpeed * Time.deltaTime;
        }
        posX = Mathf.Clamp(posX, 0, Settings.gridSizeX - 2);
        posY = Mathf.Clamp(posY, 0, Settings.gridSizeX - 2);
        transform.position = new Vector3(posX, posY, posZ);

        // mouse drag
        /*if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }
        if (!Input.GetMouseButton(0)) return;
        if (previousMousePosition == Input.mousePosition) return;
        previousMousePosition = Input.mousePosition;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 newPosition = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, posZ);
        //Camera.main.transform.Translate(newPosition, Space.World);
        transform.position = newPosition;
        //transform.position = Vector3.Lerp(transform.position, newPosition, movementTime);*/
    }

    void HandleMovementInput() 
    {
        posX = transform.position.x;
        posY = transform.position.y;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            posY += movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            posY -= movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            posX += movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            posX -= movementSpeed * Time.deltaTime;
        }

        posX = Mathf.Clamp(posX, 0, Settings.gridSizeX - 2);
        posY = Mathf.Clamp(posY, 0, Settings.gridSizeY - 4);
        transform.position = new Vector3(posX, posY, posZ);
    }
}