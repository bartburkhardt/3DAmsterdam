using ConvertCoordinates;
using Netherlands3D.T3D.Uitbouw;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WMSProjector : MonoBehaviour
{
    int tilesize = 1000;


    void Start()
    {
        MetadataLoader.Instance.BuildingOutlineLoaded += OnBuildingOutlineLoaded;
    }

    private void OnBuildingOutlineLoaded(object source, BuildingOutlineEventArgs args)
    {                

        var x = Math.Floor(args.Center.x / tilesize) * tilesize;
        var y = Math.Floor(args.Center.y / tilesize) * tilesize;
        Vector2RD rd = new Vector2RD(x + tilesize, y + tilesize);

        Vector3 pos = CoordConvert.RDtoUnity(new Vector2RD(x + tilesize / 2, y + tilesize / 2));
        transform.position = new Vector3(pos.x, 0, pos.z);
        StartCoroutine(GetTexture(rd));
    }


    IEnumerator GetTexture(Vector2RD rd) 
    { 


        var bbox = $"{rd.x},{rd.y},{rd.x+tilesize},{rd.y + tilesize}";
        //var url = $"https://service.pdok.nl/hwh/luchtfotorgb/wms/v1_0?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&BBOX={bbox}&CRS=EPSG:4326&WIDTH=1000&HEIGHT=1000&LAYERS=Actueel_orthoHR&STYLES=&FORMAT=image/jpeg&DPI=96&MAP_RESOLUTION=96&FORMAT_OPTIONS=dpi:96";
        var url = $"https://service.pdok.nl/hwh/luchtfotorgb/wms/v1_0?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&BBOX={bbox}&CRS=EPSG:28992&WIDTH=1000&HEIGHT=1000&LAYERS=Actueel_orthoHR&STYLES=&FORMAT=image/jpeg&DPI=96&MAP_RESOLUTION=96&FORMAT_OPTIONS=dpi:96";

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;


            GetComponent<Renderer>().material.mainTexture = myTexture;


        }
    }


    void Update()
    {
        
    }
}
