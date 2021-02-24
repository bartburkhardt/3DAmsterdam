using System.Collections;
using System.Collections.Generic;
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

        buildMap[0].assetBundleName = "uncompressed.assetbundle";
        BuildPipeline.BuildAssetBundles("Assets/TestAssetBundles", buildMap, BuildAssetBundleOptions., BuildTarget.WebGL);
    }
}
