using System;
using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Interface;
using UnityEngine;

namespace Netherlands3D.T3D.Uitbouw.BoundaryFeatures
{
    public class BoundaryFeature : SquarePolygon
    {
        public UitbouwMuur Wall { get; private set; }
        public Transform featureTransform { get; private set; }

        public EditMode ActiveMode { get; private set; }

        private DistanceMeasurement[] distanceMeasurements;

        private EditUI editUI;

        private MeshFilter meshFilter;

        private void Awake()
        {
            featureTransform = transform.parent;
            distanceMeasurements = GetComponents<DistanceMeasurement>();

            meshFilter = GetComponent<MeshFilter>();

            editUI = CoordinateNumbers.Instance.CreateEditUI(this);

            SetMode(EditMode.None);
        }

        public void SetWall(UitbouwMuur wall)
        {
            this.Wall = wall;
            featureTransform.position = wall.transform.position;
            featureTransform.forward = wall.transform.forward;
        }

        protected virtual void Update()
        {
            SetButtonPositions();
        }

        private void SetButtonPositions()
        {
            var pos = featureTransform.position + transform.rotation * meshFilter.mesh.bounds.extents;
            editUI.AlignWithWorldPosition(pos);
        }

        public void SetMode(EditMode newMode)
        {
            ActiveMode = newMode;
            var distanceMeasurements = this.distanceMeasurements;
            for (int i = 0; i < distanceMeasurements.Length; i++)
            {
                distanceMeasurements[i].DrawDistanceActive = (int)newMode == i;
            }

            editUI.gameObject.SetActive((int)newMode >= 0);
        }

        public void DeleteFeature()
        {
            Destroy(editUI.gameObject);
            Destroy(gameObject);
        }

        public void EditFeature()
        {
            if (ActiveMode == EditMode.Reposition)
                SetMode(EditMode.Resize);
            else
                SetMode(EditMode.Reposition);

            editUI.UpdateSprites(ActiveMode);
        }
    }
}