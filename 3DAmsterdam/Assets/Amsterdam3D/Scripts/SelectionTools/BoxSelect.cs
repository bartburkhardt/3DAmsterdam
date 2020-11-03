﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Amsterdam3D.CameraMotion;
using System.Linq;
using Assets.Amsterdam3D.Scripts.Camera;
using UnityEngine.Events;


namespace Amsterdam3D.SelectionTools
{

    public class BoxSelect : SelectionTool
    {

        [SerializeField]
        private GameObject selectionBoxPrefab;
        private RectTransform selectionBox;

        private Vector2 startPos;
        private Vector3 startPosWorld;
        private Vector2 newSizeData = new Vector2();

        private RayCastBehaviour raycastBehaviour;


        private bool inBoxSelect;
        private bool enabled;


        public override void EnableTool()
        {
            raycastBehaviour = FindObjectOfType<RayCastBehaviour>();
            GameObject selectionBoxObj = Instantiate(selectionBoxPrefab);
            selectionBox = selectionBoxObj.GetComponent<RectTransform>();
            selectionBox.SetParent(Canvas.transform);
            selectionBoxObj.SetActive(true);
            inBoxSelect = false;
            enabled = true;

        }

        private void OnEnable()
        {
            toolType = ToolType.Box;
        }

        public void Update()
        {

            if (enabled)
            {
                if (!inBoxSelect)
                {

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {

                            if (raycastBehaviour.RayCast(out startPosWorld))
                            {
                                startPos = Input.mousePosition;
                                selectionBox.gameObject.SetActive(true);
                                inBoxSelect = true;
                            }

                        }
                    }
                }
                else
                {
                    selectionBox.sizeDelta = new Vector3(Mathf.Abs((Input.mousePosition.x - startPos.x)), Mathf.Abs(Input.mousePosition.y - startPos.y), 1);
                    selectionBox.position = startPos + new Vector2((Input.mousePosition.x - startPos.x) / 2, (Input.mousePosition.y - startPos.y) / 2);
                    if (Input.GetMouseButtonUp(0))
                    {
                        
                        selectionBox.gameObject.SetActive(false);
                        inBoxSelect = false;
                        Vector2 currentMousePos = Input.mousePosition;
                        if (((startPos.x - currentMousePos.x < 10) && (startPos.x - currentMousePos.x > -10)) || ((startPos.y - currentMousePos.y < 10) && (startPos.y - currentMousePos.y > -10))) 
                        {
                            return;
                        }

                        Vector3 currentWorldPos;
                        raycastBehaviour.RayCast(out currentWorldPos);
                        Vector3 min = new Vector3();
                        Vector3 max = new Vector3();
                        if (currentWorldPos.z < startPosWorld.z)
                        {
                            min.z = currentWorldPos.z;
                            max.z = startPosWorld.z;

                        }
                        else
                        {
                            min.z = startPosWorld.z;
                            max.z = currentWorldPos.z;
                        }

                        if (currentWorldPos.x < startPosWorld.x)
                        {
                            min.x = currentWorldPos.x;
                            max.x = startPosWorld.x;
                        }
                        else
                        {
                            min.x = startPosWorld.x;
                            max.x = currentWorldPos.x;
                        }
                        if (currentWorldPos.y < startPosWorld.y)
                        {
                            min.y = currentWorldPos.y;
                            max.y = startPosWorld.y;
                        }
                        else
                        {
                            min.y = startPosWorld.y;
                            max.y = currentWorldPos.y;
                        }


                        Debug.Log("Min: " + min + " Max: " + max);
                        bounds.min = min;
                        bounds.max = max;
                        vertexes.Add(min);
                        vertexes.Add(max);
                        onSelectionCompleted?.Invoke();
                    }
                }
            }
        }

        public override void DisableTool()
        {
            enabled = false;
            inBoxSelect = false;
            selectionBox.gameObject.SetActive(false);
        }
    }
}