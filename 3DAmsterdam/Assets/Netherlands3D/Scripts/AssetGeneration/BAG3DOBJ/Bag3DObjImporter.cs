﻿using ConvertCoordinates;
using Netherlands3D;
using Netherlands3D.AssetGeneration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Bag3DObjImporter : MonoBehaviour
{
	[SerializeField]
	private string bag3DSourceFilesFolder = "";
	[SerializeField]
	private string filter = "*.obj";

	[Header("Optional. Leave blank to create all tiles")]
	[SerializeField]
	private string exclusivelyGenerateTilesWithSubstring = "";

	[SerializeField]
	private bool skipImportOfObjectsOutsideRDBounds = false;

	[Tooltip("LOD level name")]
	[SerializeField]
	private string lodLevel = "2.2";

	[Tooltip("Width and height in meters")]
	[SerializeField]
	private int tileSize = 1000; //1x1 km

	[SerializeField]
	private Vector2 tileOffset;

	[SerializeField]
	private string removePrefix = "NL.IMBAG.Pand.";

	[Header("Tile generation settings")]
	[SerializeField]
	private bool skipExistingFiles = true;

	[SerializeField]
	private bool renderInViewport = true;

	private ObjLoad objLoader;

	[SerializeField]
	private int parsePerFrame = 1000;

	[SerializeField]
	private Material defaultMaterial;

	[SerializeField]
	private GameObject enableOnFinish;

	[System.Serializable]
	public class RemapObjectNames
	{
		public string sourceName = "";
		public string newName = "";
	}

	private void Start()
	{
		StartCoroutine(ParseObjFiles());
	}

	private IEnumerator ParseObjFiles()
	{
		//Read files list 
		var info = new DirectoryInfo(bag3DSourceFilesFolder);
		var fileInfo = info.GetFiles(filter);
		print("Found " + fileInfo.Length + " obj files.");
		//First create gameobjects for all the buildings we parse
		for (int i = 0; i < fileInfo.Length; i++)
		{
			var file = fileInfo[i];
			print((i+1) + "/" + fileInfo.Length + " " + file.Name);

			var objString = File.ReadAllText(file.FullName);

			//Start a new ObjLoader
			if (objLoader) Destroy(objLoader);
			objLoader = this.gameObject.AddComponent<ObjLoad>();
			//objLoader.ObjectUsesRDCoordinates = true; //automatic

			objLoader.BottomLeftBounds = Config.activeConfiguration.BottomLeftRD;
			objLoader.TopRightBounds = Config.activeConfiguration.TopRightRD;
			objLoader.IgnoreObjectsOutsideOfBounds = skipImportOfObjectsOutsideRDBounds;
			objLoader.MaxSubMeshes = 1;
			objLoader.SplitNestedObjects = true;
			objLoader.WeldVertices = true;
			objLoader.EnableMeshRenderer = renderInViewport;
			objLoader.SetGeometryData(ref objString);

			var objLinesToBeParsed = 100;
			while (objLinesToBeParsed > 0)
			{
				objLinesToBeParsed = objLoader.ParseNextObjLines(parsePerFrame);
				yield return new WaitForEndOfFrame();
			}
			objLoader.Build(defaultMaterial);
			print(i + "/" + fileInfo.Length + " " + file.Name + " done.");
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
		yield return BakeIntoTiles();

		if (enableOnFinish) enableOnFinish.SetActive(true);
	}

	private IEnumerator BakeIntoTiles()
	{	
		print("Added all gameobjects from obj files.");
		print("Baking objects into tiles");
		var xTiles = Mathf.RoundToInt(((float)Config.activeConfiguration.TopRightRD.x - (float)Config.activeConfiguration.BottomLeftRD.x) / (float)tileSize);
		var yTiles = Mathf.RoundToInt(((float)Config.activeConfiguration.TopRightRD.y - (float)Config.activeConfiguration.BottomLeftRD.y) / (float)tileSize);

		var totalTiles = xTiles * yTiles;
		int currentTile = 0;

		//Walk the tilegrid
		var tileRD = new Vector2Int(0, 0);
		for (int x = 0; x < xTiles; x++)
		{
			tileRD.x = (int)Config.activeConfiguration.BottomLeftRD.x + (x * tileSize);
			for (int y = 0; y < yTiles; y++)
			{
				currentTile++;
				tileRD.y = (int)Config.activeConfiguration.BottomLeftRD.y + (y * tileSize);
				string tileName = "buildings_" + tileRD.x + "_" + tileRD.y + "." + lodLevel;

				//If we supplied a filter we check if this tile contains this substring in order to be (re)generated
				if (exclusivelyGenerateTilesWithSubstring != "" && !tileName.Contains(exclusivelyGenerateTilesWithSubstring))
				{
					print("Skipping tile because we supplied a specific name we want to replace.");
					if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
					continue;
				}

				//Skip files if we enabled that option and it exists
				string assetFileName = TileCombineUtility.unityMeshAssetFolder + tileName + ".asset";
				if (skipExistingFiles && File.Exists(Application.dataPath + "/../" + assetFileName))
				{
					print("Skipping existing tile: " + Application.dataPath + "/../" + assetFileName);
					if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
					continue;
				}

				//Spawn our tile container
				GameObject newTileContainer = new GameObject();
				newTileContainer.transform.position = CoordConvert.RDtoUnity(tileRD + tileOffset);
				newTileContainer.name = tileName;
				//And move children in this tile
				int childrenInTile = 0;


				MeshRenderer[] remainingBuildings = GetComponentsInChildren<MeshRenderer>(true);
				foreach (MeshRenderer meshRenderer in remainingBuildings)
				{
					meshRenderer.gameObject.name = meshRenderer.gameObject.name.Replace(removePrefix, "");
					var childCenterPoint = CoordConvert.UnitytoRD(meshRenderer.bounds.center);
					if(childCenterPoint.x < tileRD.x+tileSize && childCenterPoint.x > tileRD.x && childCenterPoint.y < tileRD.y + tileSize && childCenterPoint.y > tileRD.y)
					{
						//This child object center falls within this tile. Lets move it in there.
						meshRenderer.transform.SetParent(newTileContainer.transform, true);
						childrenInTile++;
					}
				}

				if (childrenInTile == 0)
				{
					Destroy(newTileContainer);
					print($"<color=#FFBD38>No children found for tile {tileName}</color>");
					continue;
				}

				//And when we are done, bake it.
				print($"<color=#00FF00>Baking tile {tileName} with {childrenInTile} buildings</color>");
				if (!Application.isBatchMode) yield return new WaitForEndOfFrame();

				TileCombineUtility.CombineSource(newTileContainer, newTileContainer.transform.position, renderInViewport, defaultMaterial, true);
				print("Finished tile " + tileName);
				if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
			}
		}

		print("All done!");

		if (!Application.isBatchMode) yield return new WaitForEndOfFrame();
	}
}
