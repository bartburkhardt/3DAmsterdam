﻿using Amsterdam3D.CameraMotion;
using ConvertCoordinates;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Amsterdam3D.Interface.Search
{
    public class SearchResult : MonoBehaviour
    {
        private const string REPLACEMENT_STRING = "{ID}";
        private const string lookupUrl = "https://geodata.nationaalgeoregister.nl/locatieserver/v3/lookup?id={ID}";

        private SearchResults parentList;
        public SearchResults ParentList { get => parentList; set => parentList = value; }

        [SerializeField]
        private Text textField;

        private string resultText;
        private string id;

        private LookupData lookupData;

        public string ResultText { 
            get => resultText;
            set
            {
                resultText = value;
                textField.text = value;
            }
        }
        public string ID
        {
            get => id;
            set
            {
                id = value;
                gameObject.name = id;
            }
        }

        public void ClickedResult(){
            ParentList.AutocompleteSearchText(ResultText);
            StartCoroutine(FindLocationByIDLookup());
        }

        IEnumerator FindLocationByIDLookup()
        {
            string uri = lookupUrl.Replace(REPLACEMENT_STRING, id);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    Debug.Log("Error: " + webRequest.error);
                }
                else
                {
                    string jsonStringResult = webRequest.downloadHandler.text;
                    lookupData = JsonUtility.FromJson<LookupData>(jsonStringResult);
                    int itemsFound = lookupData.response.numFound;
                    if (itemsFound == 0)
                    {
                        IDNotFound();
                        yield break;
                    }

                    string locationData = lookupData.response.docs[0].centroide_ll;
                    Vector3 targetLocation = ExtractUnityLocation(ref locationData);

                    ParentList.ShowResultsList(false);
                    CameraControls.Instance.MoveAndFocusOnLocation(targetLocation);
                }
            }
        }

        private void IDNotFound(){
            Debug.Log("ID not found. This might be a problem at the API side.");
        }

        private static Vector3 ExtractUnityLocation(ref string locationData)
        {
            locationData = locationData.Replace("POINT(", "").Replace(")", "").Replace("\"", "");
            string[] lonLat = locationData.Split(' ');

            double.TryParse(lonLat[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double lon);
            double.TryParse(lonLat[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat);

            Vector3 unityLocation = CoordConvert.WGS84toUnity(lon, lat);
            return unityLocation;
        }
    }
}