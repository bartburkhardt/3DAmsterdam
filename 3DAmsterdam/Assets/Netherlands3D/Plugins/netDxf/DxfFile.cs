﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using netDxf;
using netDxf.Entities;
using netDxf.Blocks;
using netDxf.Tables;
using ConvertCoordinates;
using System.IO;
using System.Runtime.InteropServices;

public class DxfFile 
{
    private DxfDocument doc;
    private Layer Laag;

    public void SetupDXF()
    {
        doc = new DxfDocument();
        doc.DrawingVariables.InsUnits = netDxf.Units.DrawingUnits.Meters;
    }

    public void AddLayer(List<Vector3RD>triangleVertices,string layerName)
    {
        // TODO 
        // check if there are 3 triangles or less, if that is the case a polyfaceMesh cannot be built, seperate triangles have to be added to the dxf.


        Laag = new Layer(layerName);
        Laag.Color = netDxf.AciColor.LightGray;
        doc.Layers.Add(Laag);
        PolyfaceMesh pfm;
        // create Mesh
        List<PolyfaceMeshVertex> pfmVertices = new List<PolyfaceMeshVertex>();
           pfmVertices.Capacity= triangleVertices.Count;
        List<PolyfaceMeshFace> pfmFaces = new List<PolyfaceMeshFace>();
        pfmFaces.Capacity = triangleVertices.Count/3;
        int facecounter = 0;
        Debug.Log(triangleVertices.Count);
        int vertexIndex = 0;

        AddTriangles(triangleVertices, layerName);
        //for (int i = 0; i < triangleVertices.Count; i+=3)
        //{

            //AddTriangles(new List<Vector3RD>() { triangleVertices[i], triangleVertices[i + 1], triangleVertices[i + 2] },layerName);

            //vertexIndex = facecounter * 3;
            //pfmVertices.Add(new PolyfaceMeshVertex(triangleVertices[i].x, triangleVertices[i].y, triangleVertices[i].z));
            //pfmVertices.Add(new PolyfaceMeshVertex(triangleVertices[i + 1].x, triangleVertices[i + 1].y, triangleVertices[i + 1].z));
            //pfmVertices.Add(new PolyfaceMeshVertex(triangleVertices[i + 2].x, triangleVertices[i + 2].y, triangleVertices[i + 2].z));
            //PolyfaceMeshFace pfmFace = new PolyfaceMeshFace(new List<short>() { (short)(vertexIndex), (short)(vertexIndex + 1), (short)(vertexIndex + 2) });
            //pfmFaces.Add(pfmFace);
            //facecounter++;
            //if (facecounter % 10000 == 0)
            //{
            //    pfm = new PolyfaceMesh(pfmVertices, pfmFaces);
            //    pfm.Layer = Laag;
            //    doc.AddEntity(pfm);
            //    pfmVertices.Clear();
            //    pfmFaces.Clear();
            //    facecounter = 0;
            //}
        //}
        //pfm = new PolyfaceMesh(pfmVertices, pfmFaces);
        //pfm.Layer = Laag;
        //doc.AddEntity(pfm);






    }
    public void Save()
    {
#if UNITY_EDITOR

        doc.Save("D:/testDXFBinary.dxf", true);
        
        return;
#endif
        MemoryStream stream = new MemoryStream();
        if (doc.Save(stream))
        {
            DownloadFile(stream.ToArray(), stream.ToArray().Length, "testfile.dxf"); ;
        }
        else
        {
            Debug.Log("cant write file");
        }
    }

    public void AddTriangles(List<Vector3RD> triangleVertices, string layerName)
    {
        

        for (int i = 0; i < triangleVertices.Count; i+=3)
        {
            netDxf.Vector3 vertex1 = new netDxf.Vector3(triangleVertices[i].x, triangleVertices[i].y, triangleVertices[i].z);
            netDxf.Vector3 vertex2 = new netDxf.Vector3(triangleVertices[i+1].x, triangleVertices[i+1].y, triangleVertices[i+1].z);
            netDxf.Vector3 vertex3 = new netDxf.Vector3(triangleVertices[i+2].x, triangleVertices[i+2].y, triangleVertices[i+2].z);
            Face3d face = new Face3d(vertex1, vertex2, vertex3);
            doc.AddEntity(face);
            face.Layer = Laag;
        }
        

    }

    [DllImport("__Internal")]
    private static extern void DownloadFile(byte[] array, int byteLength, string fileName);
}
