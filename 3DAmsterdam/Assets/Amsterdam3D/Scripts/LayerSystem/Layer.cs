﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LayerSystem
{
    public class Layer : MonoBehaviour
    {
        [SerializeField]
        public Material DefaultMaterial;
        public Material HighlightMaterial;
        public int tileSize = 1000;
        public int layerPriority = 0;
        public List<DataSet> Datasets = new List<DataSet>();
        public Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

        void Start()
        {
            foreach (DataSet dataset in Datasets)
            {
                dataset.maximumDistanceSquared = dataset.maximumDistance * dataset.maximumDistance;
            }
        }

        public void UnHighlightAll()
        {
            StartCoroutine(PrivateHighlight("null"));
        }

        public void Highlight(string id)
        {
            StartCoroutine(PrivateHighlight(id));
        }

        public void Hide(string id) 
        {
            StartCoroutine(PrivateHide(id));
        }

        public void UnhideAll() 
        {
            StartCoroutine(PrivateHide("null"));
        }

        private IEnumerator PrivateHide(string id) 
        {
            transform.GetComponentInParent<TileHandler>().pauseLoading = true;
            ObjectData objectdata;
            Vector2[] UVs;
            foreach (KeyValuePair<Vector2Int, Tile> kvp in tiles)
            {
                objectdata = kvp.Value.gameObject.GetComponent<ObjectData>();
                if (objectdata != null)
                {
                    if (objectdata.ids.Contains(id) == false)
                    {
                        if (objectdata.hideIDs.Count == 0)
                        {
                            continue;
                        }
                    }

   
                    if (id == "null")
                    {
                        objectdata.hideIDs.Clear();
                        objectdata.SetUVs();
                    }
                    else
                    {
                        objectdata.hideIDs.Add(id);
                        objectdata.mesh = objectdata.gameObject.GetComponent<MeshFilter>().mesh;
                        objectdata.SetHideUVs();
                    }
                    //objectdata.gameObject.GetComponent<MeshFilter>().mesh.uv2 = UVs;
                    yield return null;
                }
            }
            transform.GetComponentInParent<TileHandler>().pauseLoading = false;
        }

        private IEnumerator PrivateHighlight(string id)
        {
            transform.GetComponentInParent<TileHandler>().pauseLoading = true;
            ObjectData objectdata;
            Vector2[] UVs;
            foreach (KeyValuePair<Vector2Int, Tile> kvp in tiles)
            {
                objectdata = kvp.Value.gameObject.GetComponent<ObjectData>();
                if (objectdata != null)
                {
                    if (objectdata.ids.Contains(id)==false)
                    {
                        if (objectdata.highlightIDs.Count == 0)
                        {
                            continue;
                        }
                    }
                    
                    objectdata.highlightIDs.Clear();
                    if (id == "null")
                    {
                        objectdata.SetUVs();
                    }
                    else
                    {
                        objectdata.highlightIDs.Add(id);
                        objectdata.mesh = objectdata.gameObject.GetComponent<MeshFilter>().mesh;
                        objectdata.SetUVs();
                    }
                    //objectdata.gameObject.GetComponent<MeshFilter>().mesh.uv2 = UVs;
                    yield return null;
                }
            }
            transform.GetComponentInParent<TileHandler>().pauseLoading = false;
            
        }
    }
}
