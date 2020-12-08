﻿using Amsterdam3D.CameraMotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amsterdam3D.Interface
{
    public class ObjectProperties : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectPropertiesPanel;
        [SerializeField]
        private Transform generatedFieldsContainer;

        [SerializeField]
        private RenderTexture thumbnailRenderTexture;

        public DisplayBAGData displayBagData;

        public static ObjectProperties Instance = null;

        [SerializeField]
        private Text titleText;

        [Header("Generated field prefabs:")]
        [SerializeField]
        private GameObject subtitlePrefab;
        [SerializeField]
        private DataKeyAndValue dataFieldPrefab;
        [SerializeField]
        private GameObject seperatorLinePrefab;
        [SerializeField]
        private NameAndURL urlPrefab;

        private Camera thumbnailRenderer;

        void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}

			//Properties panel is disabled at startup
			objectPropertiesPanel.SetActive(false);

			CreateThumbnailRenderCamera();
		}

		private void CreateThumbnailRenderCamera()
		{
			//Our render camera for thumbnails. 
            //We disable it so we cant manualy render a single frame using Camera.Render();
			thumbnailRenderer = new GameObject().AddComponent<Camera>();
			thumbnailRenderer.fieldOfView = 30;
			thumbnailRenderer.farClipPlane = 5000;
			thumbnailRenderer.targetTexture = thumbnailRenderTexture;
			thumbnailRenderer.enabled = false;
		}

		public void OpenPanel(string title)
        {
            ClearGeneratedFields();

            objectPropertiesPanel.SetActive(true);
            titleText.text = title;
        }
        public void ClosePanel()
        {
            objectPropertiesPanel.SetActive(false);
        }

        public void RenderThumbnailFromPosition(Vector3 from, Vector3 to)
        {
            thumbnailRenderer.transform.position = from;
            thumbnailRenderer.transform.LookAt(to);
            thumbnailRenderer.Render();
        }

        public void AddSubtitle(string titleText)
        {
            Instantiate(subtitlePrefab, generatedFieldsContainer).GetComponent<Text>().text = titleText;
        }

        public void AddDataField(string keyTitle, string valueText)
        {
            Instantiate(dataFieldPrefab, generatedFieldsContainer).SetTexts(keyTitle, valueText);
        }
        public void AddSeperatorLine()
        {
            Instantiate(seperatorLinePrefab, generatedFieldsContainer);
        }
        public void AddURLText(string urlText, string urlPath)
        {
            Instantiate(urlPrefab, generatedFieldsContainer).SetURL(urlText,urlPath);
        }

        public void ClearGeneratedFields()
        {
            foreach (Transform field in generatedFieldsContainer)
            {
                Destroy(field.gameObject);
            }
        }
    }
}