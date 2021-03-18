﻿using Netherlands3D.Interface.Coloring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace Netherlands3D.Interface.Layers
{
	public class LayerVisuals : MonoBehaviour
	{
		[SerializeField]
		private MaterialSlot materialSlotPrefab;

		[SerializeField]
		private ColorPicker colorPicker;

		[SerializeField]
		private HexColorField hexColorField;

		[SerializeField]
		private Slider opacitySlider;

		[SerializeField]
		private RectTransform materialSlotsContainer;

		[HideInInspector]
		public InterfaceLayer targetInterfaceLayer;

		private List<MaterialSlot> selectedMaterialSlots;

		[SerializeField]
		private Vector2 locationOffset;

		private void Awake()
		{
			colorPicker.selectedNewColor += ChangeMaterialColor;
			hexColorField.selectedNewColor += ChangeMaterialColor;
		}

		/// <summary>
		/// Change the color, and update all the color selectors to match the same color.
		/// </summary>
		/// <param name="pickedColor">The color to change to</param>
		/// <param name="selector">The selector we used to change the color</param>
		private void ChangeMaterialColor(Color pickedColor, ColorSelector selector)
		{
			if (selectedMaterialSlots.Count < 1) return;

			foreach(MaterialSlot materialSlot in selectedMaterialSlots)
			{
				materialSlot.ChangeColor(pickedColor);
				if (materialSlot.transform.GetSiblingIndex() == 0)
				{
					targetInterfaceLayer.UpdateLayerPrimaryColor();
				}
			}

			//Match other selector colors
			if (selector == colorPicker)
			{
				hexColorField.ChangeColorInput(pickedColor);
			}
			else if (selector == hexColorField)
			{
				colorPicker.ChangeColorInput(pickedColor);
			}
		}

		/// <summary>
		///	Change the opacity of all selected material slots.
		///	Swap the shader type to opaque for optimal performance on 100% opacity.
		/// Can be called directly from the opacity slider.
		/// </summary>
		/// <param name="value">Opacity from 0.0 to 1.0</param>
		public void ChangeMaterialOpacity(float opacity)
		{
			foreach (MaterialSlot materialSlot in selectedMaterialSlots)
			{
				materialSlot.ChangeOpacity(opacity);
			}
		}

		/// <summary>
		/// Open this layer visuals panel with the options for this target interface layer.
		/// </summary>
		/// <param name="interfaceLayer">Target interface layer</param>
		public void OpenWithOptionsForLayer(InterfaceLayer interfaceLayer)
		{
			this.GetComponent<RectTransform>().anchoredPosition = interfaceLayer.GetComponent<RectTransform>().anchoredPosition + locationOffset;
			targetInterfaceLayer = interfaceLayer;
			gameObject.SetActive(true);

			GenerateMaterialSlots();
		}

		/// <summary>
		/// Closes the dialog GameObject
		/// </summary>
		public void Close()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Generate all the material slots for this layer.
		/// Select the first one if none is selected yet.
		/// </summary>
		private void GenerateMaterialSlots()
		{
			ClearMaterialSlots();

			for (int i = 0; i < targetInterfaceLayer.UniqueLinkedObjectMaterials.Count; i++)
			{
				var uniqueMaterial = targetInterfaceLayer.UniqueLinkedObjectMaterials[i];

				MaterialSlot newMaterialSlot = Instantiate(materialSlotPrefab, materialSlotsContainer);
				newMaterialSlot.Init(uniqueMaterial, targetInterfaceLayer.ResetColorValues[i], this, targetInterfaceLayer.transparentShaderSourceOverride, targetInterfaceLayer.opaqueShaderSourceOverride, targetInterfaceLayer.swapTransparentMaterialSources);

				if (selectedMaterialSlots.Count < 1) SelectMaterialSlot(newMaterialSlot);
			}
		}

		/// <summary>
		/// Reset all the colors while holding shift, or just the material(s) we selected
		/// </summary>
		public void ResetColorsInSelectedMaterials(){
			var resetAll = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			if (resetAll)
			{
				//Simple reset all material slots, instead of our selected list
				var allMaterialSlots = materialSlotsContainer.GetComponentsInChildren<MaterialSlot>();
				foreach (MaterialSlot materialSlot in allMaterialSlots)
				{
					materialSlot.ResetColor();
				}
			}			
			foreach (MaterialSlot materialSlot in selectedMaterialSlots)
			{
				materialSlot.ResetColor();
			}
			hexColorField.ChangeColorInput(selectedMaterialSlots[0].GetMaterialColor);
			colorPicker.ChangeColorInput(selectedMaterialSlots[0].GetMaterialColor);
			targetInterfaceLayer.UpdateLayerPrimaryColor();

		}

		/// <summary>
		/// Clear all the current material slots.
		/// </summary>
		private void ClearMaterialSlots()
		{
			ClearMaterialSlotsSelection();

			foreach (Transform materialSlot in materialSlotsContainer)
			{
				Destroy(materialSlot.gameObject);
			}
		}

		/// <summary>
		/// Clears our multiselection list or create it if it doesnt exist yet
		/// </summary>
		private void ClearMaterialSlotsSelection()
		{
			if (selectedMaterialSlots == null)
			{
				selectedMaterialSlots = new List<MaterialSlot>();
			}
			else
			{
				selectedMaterialSlots.Clear();
			}
		}

		/// <summary>
		/// Selection of a specific material slot. Only clear materialslots list if we dont do multiselect
		/// </summary>
		/// <param name="selectedMaterialSlot">Selected material slot</param>
		public void SelectMaterialSlot(MaterialSlot selectedMaterialSlot, bool multiSelect = false)
		{
			if (!multiSelect)
			{
				selectedMaterialSlots.Clear();
				//If we are not multiselecting, make sure we only select this one
				var materialSlots = materialSlotsContainer.GetComponentsInChildren<MaterialSlot>();
				foreach (MaterialSlot slot in materialSlots)
					slot.Selected = (slot == selectedMaterialSlot) ? true : false;
			}
			selectedMaterialSlots.Add(selectedMaterialSlot);
			
			if (!multiSelect)
			{
				colorPicker.ChangeColorInput(selectedMaterialSlot.GetMaterialColor);
				hexColorField.ChangeColorInput(selectedMaterialSlot.GetMaterialColor);
				opacitySlider.value = selectedMaterialSlot.materialOpacity;
			}
		}
	}
}