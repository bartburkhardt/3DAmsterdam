using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CompareCompressionOutcome
{
    const string assetPath = "Assets/TestAssetBundles/testmesh.mesh";

    [MenuItem("3D Amsterdam/Compression/Export selected test mesh")]
    public static void MeshToAsset()
    {
        GameObject gameObjectTarget = Selection.activeGameObject;
        var targetExportMeshFilter = gameObjectTarget.GetComponent<MeshFilter>();
        Mesh targetExportMesh = targetExportMeshFilter.mesh;

        string assetPath = "Assets/TestAssetBundles/testmesh.mesh";

        //Turn this into an asset
        AssetDatabase.CreateAsset(targetExportMesh, assetPath);
    }

    [MenuItem("3D Amsterdam/Compression/Build assetbundles from test mesh")]
    public static void AssetToAssetBundles()
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        string[] assetNames = new string[1];
        assetNames[0] = assetPath;
        buildMap[0].assetNames = assetNames;

        buildMap[0].assetBundleName = "default_LZMA.assetbundle";
        BuildPipeline.BuildAssetBundles("Assets/TestAssetBundles", buildMap, BuildAssetBundleOptions.None, BuildTarget.WebGL);

        buildMap[0].assetBundleName = "chunk_based_LZ4.assetbundle";
        BuildPipeline.BuildAssetBundles("Assets/TestAssetBundles", buildMap, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);

        buildMap[0].assetBundleName = "uncompressed.assetbundle";
        BuildPipeline.BuildAssetBundles("Assets/TestAssetBundles", buildMap, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.WebGL);
    }

    [MenuItem("3D Amsterdam/Compression/Convert to uncompressed")]
    public static void ConvertToUncompressed()
    {
        var info = new DirectoryInfo("C:/Users/Sam/Desktop/BrotliCheckTiles/tiles");
        var fileInfo = info.GetFiles();

        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();

        foreach (FileInfo file in fileInfo)
        {
            if (file.Name.Contains(".br") || file.Name.Contains("_uncompressed")) continue; //skip default assetbundles

            Debug.Log("Converting " + file.Name);

            //if (file.Length < 39000000) continue;

            var filePath = file.FullName;
            var myLoadedAssetBundle = AssetBundle.LoadFromFile(filePath);
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Not an assetbundle: " + filePath);
            }
            else
            {
                Mesh[] meshesInAssetbundle;
                meshesInAssetbundle = myLoadedAssetBundle.LoadAllAssets<Mesh>();
                Mesh sourceMesh = meshesInAssetbundle[0];

               //Copy the mesh
                Mesh mesh = new Mesh();
                mesh.indexFormat = sourceMesh.indexFormat;
                mesh.vertices = sourceMesh.vertices;
                mesh.uv = sourceMesh.uv;
                mesh.uv2 = sourceMesh.uv2;
                mesh.uv3 = sourceMesh.uv3;
                mesh.uv4 = sourceMesh.uv4;
                mesh.normals = sourceMesh.normals;
                mesh.triangles = sourceMesh.triangles;
                mesh.subMeshCount = sourceMesh.subMeshCount;
                if (sourceMesh.subMeshCount > 0)
                {
                    for (int i = 0; i < sourceMesh.subMeshCount; i++)
                    {
                        //submeshes
                        mesh.SetTriangles(sourceMesh.GetTriangles(i), i);
                    }
                }

                var assetFileName = "Assets/TestAssetBundles/" + file.Name + ".mesh";
                var assetBundleFileName = "Assets/TestAssetBundles/" + file.Name + "_uncompressed";
                //Turn this into an asset
                AssetDatabase.CreateAsset(mesh, assetFileName);
  
                AssetBundleBuild newBuild = new AssetBundleBuild();
                string[] assetNames = new string[1];
                assetNames[0] = assetFileName;
                newBuild.assetNames = assetNames;
                newBuild.assetBundleName = assetBundleFileName;
                buildMap.Add(newBuild);

                myLoadedAssetBundle.Unload(true);
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
        }

        BuildPipeline.BuildAssetBundles("Assets/TestAssetBundles", buildMap.ToArray(), BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.WebGL);
    }
}
