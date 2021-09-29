using System;
using System.Collections;
using System.Collections.Generic;
using Netherlands3D;
using Netherlands3D.Cameras;
using Netherlands3D.Interface;
using Netherlands3D.ObjectInteraction;
using Netherlands3D.T3D.Uitbouw;
using UnityEngine;
using UnityEngine.InputSystem;
using Netherlands3D.JavascriptConnection;

public enum InteractableState
{
    Default = 0,
    Hover = 1,
    Active = 2,
}

public class DragableAxis : Interactable
{
    [SerializeField]
    private float maxClickDragDistance = 50f;

    //[SerializeField]
    private Uitbouw uitbouw;
    private Vector3 offset;

    public Vector3 DeltaPosition { get; private set; }
    public bool IsDragging { get; private set; }

    [SerializeField]
    private ColorPalette interactionColors;

    public static DragableAxis CreateDragableAxis(GameObject prefab, Vector3 position, Quaternion rotation, Uitbouw linkedUitbouw)
    {
        var axisObject = Instantiate(prefab, position, rotation, linkedUitbouw.transform);
        var axis = axisObject.GetComponent<DragableAxis>();
        axis.SetUitbouw(linkedUitbouw);
        return axis;
    }

    public void SetUitbouw(Uitbouw linkedUitbouw)
    {
        uitbouw = linkedUitbouw;
    }

    private void Update()
    {
        ProcessInteractionState();

        if (IsDragging)
        {
            CalculateDeltaPosition();
            //transform.position += DeltaPosition;
        }
    }

    private void ProcessInteractionState()
    {
        if (IsHovered())
        {
            if (Input.GetMouseButtonDown(0))
            {
                //start drag
                TakeInteractionPriority();
                IsDragging = true;
                SetHighlight(InteractableState.Active);
                RecalculateOffset();
            }
            else if (!Input.GetMouseButton(0))
            {
                SetHighlight(InteractableState.Hover);
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            //not dragging and not hovering
            SetHighlight(InteractableState.Default);
        }

        if (Input.GetMouseButtonUp(0))
        {
            //end drag
            StopInteraction();
            IsDragging = false;
            offset = Vector3.zero;
            DeltaPosition = Vector3.zero;
        }
    }

    public void RecalculateOffset()
    {
        Vector3 aimedPosition = GetPointerPositionInWorld();
        var projectedLocalPoint = Vector3.Project((aimedPosition - transform.position), uitbouw.transform.right);
        offset = projectedLocalPoint;
    }

    private void CalculateDeltaPosition()
    {
        Vector3 aimedPosition = GetPointerPositionInWorld();
        var projectedPoint = Vector3.Project((aimedPosition - transform.position), uitbouw.transform.right) + transform.position;
        DeltaPosition = projectedPoint - offset - transform.position;
    }

    private void SetHighlight(InteractableState status) //0: normal, 1: hover, 2: selected
    {
        if (!interactionColors)
            return;

        //switch (status)
        //{
        //    case InteractableState.Default:
        //        ChangePointerStyleHandler.ChangeCursor(ChangePointerStyleHandler.Style.AUTO);
        //        break;
        //    case InteractableState.Hover:
        //        ChangePointerStyleHandler.ChangeCursor(ChangePointerStyleHandler.Style.GRAB);
        //        print("grab");
        //        break;
        //    case InteractableState.Active:
        //        print("grabing");
        //        ChangePointerStyleHandler.ChangeCursor(ChangePointerStyleHandler.Style.GRABBING);
        //        break;
        //}

        var renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.material.color = interactionColors[status.ToString()];
        }
    }

    public Vector3 GetPointerPositionInWorld(Vector3 optionalPositionOverride = default)
    {
        var pointerPosition = Mouse.current.position.ReadValue();
        if (optionalPositionOverride != default) pointerPosition = optionalPositionOverride;

        var cameraComponent = CameraModeChanger.Instance.ActiveCamera;
        var screenRay = cameraComponent.ScreenPointToRay(pointerPosition);
        uitbouw.GroundPlane.Raycast(screenRay, out float distance);
        var samplePoint = screenRay.GetPoint(Mathf.Min(maxClickDragDistance, distance));
        samplePoint.y = Config.activeConfiguration.zeroGroundLevelY;

        return samplePoint;
    }
}
