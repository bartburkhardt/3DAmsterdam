﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Netherlands3D.Interface.SidePanel
{
	public class AddToNumberInput : MonoBehaviour, IScrollHandler
	{
		[SerializeField]
		private InputField inputField;

		[SerializeField]
		private float addAmount = 1.0f;

		[SerializeField]
		private float modifierAmount = 15.0f;

		/// <summary>
		/// Add a value to the numeric (text) input field
		/// </summary>
		/// <param name="multiply"></param>
		public void AddValue(float multiply)
		{
			var add = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? modifierAmount * multiply : addAmount * multiply;
			if (double.TryParse(inputField.text, out double inputValue))
			{
				inputValue += add;
				inputField.text = inputValue.ToString(CultureInfo.InvariantCulture);

				//Force a value change event
				inputField.onValueChanged.Invoke(null);
			}
		}

		/// <summary>
		/// Adds value based on scroll event delta
		/// </summary>
		/// <param name="eventData">The scroll event data</param>
		public void OnScroll(PointerEventData eventData)
		{
			if (eventData.scrollDelta.y > 0)
			{
				AddValue(1.0f);
			}
			else if (eventData.scrollDelta.y < 0)
			{
				AddValue(-1.0f);
			}
		}
	}
}