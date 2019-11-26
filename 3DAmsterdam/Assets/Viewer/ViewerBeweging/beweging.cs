﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class beweging : MonoBehaviour
{
    public Camera cam;

    public float zoomSpeed = 0.5f;
    private const float zoomSpeedAir = 800f;
    private const float maxZoomOut = 2500f;
    private const float maxZoomIn = 47f;
    private const float rotationSpeed = 1f;
    private const float maxRotate = 50f;
    private const float minAngle = -10f;
    private const float maxAngle = 89f;
    private const float speedFactor = 0.5f;
    public float dragFactor = 2f;
    private const float rotationSensitivity = 5f;
    private const float maxYAngle = 80f;

    private float scroll;
    private float zoomDistance;
    private float moveSpeed;
    private float mouseHor;
    private float mouseVer;
    private float dragSpeed;

    public bool canMove = true;
    private bool canUseFunction = true;
    private bool hitCollider = false;
    public bool IsFPSmode = false;
    public bool LockFunctions = false;

    private Quaternion startRotation = Quaternion.Euler(45f, 0, 0);
    private Vector3 zoomPoint;
    private Vector3 zoomDirection;
    private Vector3 zoom;
    private Vector3 direction;
    private Vector3 dragOrigin;
    private Vector3 movDir;
    private Vector3 rotatePoint;
    private Vector3 camOffSet;

    private Vector2 currentRotation;

    private MenuFunctions menuFunctions;

    void Start()
    {
        menuFunctions = FindObjectOfType<MenuFunctions>();
        //cam.transform.rotation = startRotation;
        currentRotation = new Vector2(cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.x);
    }

    void Update()
    {
        // checkt of de muis oven een UI element zit (zo ja, dan kunnen bepaalde functies niet gebruikt worden)
        if (EventSystem.current.IsPointerOverGameObject() || IsFPSmode || LockFunctions ||
            menuFunctions.allMenus[5].activeSelf /* Kan anders geen gebied selecteren omdat wereld draggen. */)
        {
            canUseFunction = false;
        }
        else
        {
            canUseFunction = true;
        }

        // als spatie wordt ingedrukt wordt de huidige actie gestopt
        if (!(Input.GetKey(KeyCode.Space)))
        {
            StandardMovement();
            Rotation();

            if (canUseFunction)
            {
                Zooming();
                Dragging();
                FocusPoint();
            }
        }
        if(IsFPSmode)
        {
            ClampToGround();
        }
    }

    void StandardMovement()
    {
        if(IsFPSmode)
        {
            moveSpeed = 5f * Time.deltaTime;
        }
        else
        {
            moveSpeed = Mathf.Sqrt(cam.transform.position.y) * speedFactor;
        }
        

        if (canMove)
        {
            // de directie wordt gelijk gezet aan de juiste directie plus hoeveel de camera gedraaid is
            movDir = Quaternion.AngleAxis(cam.transform.eulerAngles.y, Vector3.up) * Vector3.forward;

            // vooruit/achteruit bewegen (gebaseerd op rotatie van camera)
            if (Input.GetKey(KeyCode.UpArrow)) cam.transform.position += movDir * moveSpeed;
            if (Input.GetKey(KeyCode.DownArrow)) cam.transform.position -= movDir * moveSpeed;

            if (!IsFPSmode)
            {
                // zijwaarts bewegen (gebaseerd op rotatie van camera)
                if (Input.GetKey(KeyCode.LeftArrow)) cam.transform.position -= cam.transform.right * moveSpeed;
                if (Input.GetKey(KeyCode.RightArrow)) cam.transform.position += cam.transform.right * moveSpeed;
            }
            
        }
    }

    void Rotation()
    {
        // rotatie met control knop
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            currentRotation = new Vector2(cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.x);

            canMove = false;

            // roteren naar links/rechts met control knop
            if (Input.GetKey(KeyCode.LeftArrow)) cam.transform.RotateAround(cam.transform.position, Vector3.up, -rotationSpeed);
            if (Input.GetKey(KeyCode.RightArrow)) cam.transform.RotateAround(cam.transform.position, Vector3.up, rotationSpeed);

            // de camera kan niet verder geroteerd worden dan de min en max angle
            cam.transform.rotation = Quaternion.Euler(new Vector3(ClampAngle(cam.transform.eulerAngles.x, minAngle, maxAngle),
                                                                             cam.transform.eulerAngles.y, cam.transform.eulerAngles.z));

            // roteren omhoog/omlaag
            if (Input.GetKey(KeyCode.UpArrow)) cam.transform.RotateAround(cam.transform.position, cam.transform.right, -rotationSpeed);
            if (Input.GetKey(KeyCode.DownArrow)) cam.transform.RotateAround(cam.transform.position, cam.transform.right, rotationSpeed);
        }

        // rotatie met shift knop
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentRotation = new Vector2(cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.x);

            canMove = false;

            // roteren naar links/recht met shift knop.
            if (Input.GetKey(KeyCode.LeftArrow)) cam.transform.RotateAround(cam.transform.position, Vector3.up, rotationSpeed);
            if (Input.GetKey(KeyCode.RightArrow)) cam.transform.RotateAround(cam.transform.position, Vector3.up, -rotationSpeed);
        }

        // bewegingsfunctionaliteit wordt weer actief als control/shift losgelaten wordt.
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl) ||
            Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            canMove = true;
        }




        // camera roteren doormiddel van rechter muisknop
        if (Input.GetMouseButton(1))
        {
            mouseHor = Input.GetAxis("Mouse X");
            mouseVer = Input.GetAxis("Mouse Y");

            // berekent rotatie van camera gebaseerd op beweging van muis
            currentRotation.x += mouseHor * rotationSensitivity;
            currentRotation.y -= mouseVer * rotationSensitivity;

            // de rotatie blijft tussen de 0 en 360 graden
            currentRotation.x = Mathf.Repeat(currentRotation.x, 360f);

            // zorgt dat de rotatie niet verder kan dan de min en max angle
            currentRotation.y = Mathf.Clamp(currentRotation.y, minAngle, maxAngle);

            //// de rotatie van de camera wordt aangepast
            cam.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        }


        //
        if (IsFPSmode)
        {
            // roteren naar links/rechtsmet pijlen
            if (Input.GetKey(KeyCode.LeftArrow)) cam.transform.RotateAround(cam.transform.position, Vector3.up, -rotationSpeed);
            if (Input.GetKey(KeyCode.RightArrow)) cam.transform.RotateAround(cam.transform.position, Vector3.up, rotationSpeed);
        }

    }

    void ClampToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, transform.TransformDirection(Vector3.down), out hit))
        {
            cam.transform.position = hit.point+ new Vector3(0,1.8f,0);
        }
        

    }

    void Zooming()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomPoint = cam.transform.position;

        Vector3 maxZoomPosition = new Vector3(cam.transform.position.x, maxZoomOut, cam.transform.position.z);
        Vector3 minZoomPosition = new Vector3(cam.transform.position.x, maxZoomIn, cam.transform.position.z);

        // raycast wordt afgevuurd naar de positie van de muis. als er iets wordt gedecteerd wordt dat opgeslagen in een variabel.
        if (Physics.Raycast(ray, out hit))
        {
            hitCollider = true; // checkt of er een collider geraakt wordt met raycast

            zoomPoint = hit.point;
        } else
        {
            hitCollider = false;
        }

        // de afstand tussen de camera en het zoompunt wordt berekend.
        zoomDistance = Vector3.Distance(zoomPoint, cam.transform.position);

        // de richting waar in gezoomd moet worden wordt berekend.
        zoomDirection = Vector3.Normalize(zoomPoint - cam.transform.position);

        zoom = zoomDirection * zoomDistance * scroll * zoomSpeed;

        if (hitCollider)
        {
            // er kan niet verder worden uitgezoomd dan de maximale range.
            if (cam.transform.position.y > maxZoomOut)
            {
                cam.transform.position = maxZoomPosition;
            }
            // er kan niet verder worden ingezoomd dan de minimale range.
            else if (cam.transform.position.y < maxZoomIn)
            {
                cam.transform.position = minZoomPosition;
            }
            else
            {
                // als de maximale uitzoom range bereikt is kan er alleen ingezoomd worden.
                if (cam.transform.position.y == maxZoomOut)
                {
                    if (scroll > 0) cam.transform.position += zoom;
                }
                // als de maximale inzoom range bereikt is kan er alleen uitgezoomd worden.
                else if (cam.transform.position.y == maxZoomIn)
                {
                    if (scroll < 0) cam.transform.position += zoom;
                }
                // de positie van de camera wordt aangepast.
                else
                {
                    cam.transform.position += zoom;
                }
            }
        } else // in/uit zoomen op de lucht
        {
            if (currentRotation.y < 20f)
            {
                cam.transform.position += scroll * zoomSpeedAir * movDir;
            }
        }
    }

    void Dragging()
    {
        dragSpeed = Mathf.Sqrt(cam.transform.position.y) * dragFactor;

        if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            // het punt vanaf waar gesleept wordt, wordt opgeslagen als de muis ingedrukt wordt
            if (Input.GetMouseButtonDown(0)) dragOrigin = Input.mousePosition;

            // als de muis niet ingedrukt wordt, wordt de methode verlaten
            if (!Input.GetMouseButton(0)) return;

            // de positie waar de muis heen beweegt wordt bijgehouden
            Vector2 updatedPos = cam.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

            // de bewegingsfactor voor de camera wordt berekent
            Vector3 camMove = new Vector3(updatedPos.x * dragSpeed, 0, updatedPos.y * dragSpeed);

            // de bewegingsfactor verandert gebaseerd op hoeveel de camera gedraaid is
            camMove = Quaternion.AngleAxis(cam.transform.eulerAngles.y, Vector3.up) * camMove;

            // de bewegingfactor wordt van de positie afgetrokken zodat de camera de andere kant op beweegt
            cam.transform.position -= camMove;
        }
    }

    void FocusPoint()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(2))
        {
            if (Physics.Raycast(ray, out hit))
            {
                rotatePoint = hit.point;
            }
        }

        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (cam.transform.position.y > 50f)
            {
                cam.transform.RotateAround(rotatePoint, cam.transform.right, -mouseY * 5f);
                cam.transform.RotateAround(rotatePoint, Vector3.up, mouseX * 5f);
            }
        }
    }

    private float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }
}