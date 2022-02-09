using ConvertCoordinates;
using Netherlands3D.T3D.Uitbouw;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WMSProjector : MonoBehaviour
{
    public int tilesize = 100;
    public int pixels = 2000;
    public int projectorHeight = 20;

    public string url;
    public int textureWidth;

    Renderer ren;

    void Start()
    {
        ren = GetComponent<Renderer>();

        this.transform.localScale = new Vector3( tilesize, tilesize, tilesize);
        MetadataLoader.Instance.BuildingOutlineLoaded += OnBuildingOutlineLoaded;
    }

    private void OnBuildingOutlineLoaded(object source, BuildingOutlineEventArgs args)
    {

        //var x = Math.Floor(args.Center.x / tilesize) * tilesize;
        //var y = Math.Floor(args.Center.y / tilesize) * tilesize;
        //Vector2RD rd = new Vector2RD(x + tilesize, y + tilesize);
        //Vector3 pos = CoordConvert.RDtoUnity(new Vector2RD(x + tilesize / 2, y + tilesize / 2));
        //transform.position = new Vector3(pos.x, 0, pos.z);

        transform.position = CoordConvert.RDtoUnity(args.Center);
        StartCoroutine(GetTexture(args.Center));
    }


    IEnumerator GetTexture(Vector2RD rd) 
    {
        Texture2D t = new Texture2D(pixels, pixels, TextureFormat.DXT1, false, true);

        double tilehalf = tilesize /2;

        var bbox = $"{rd.x-tilehalf},{rd.y-tilehalf},{rd.x+tilehalf},{rd.y+tilehalf}";
        //var url = $"https://service.pdok.nl/hwh/luchtfotorgb/wms/v1_0?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&BBOX={bbox}&CRS=EPSG:4326&WIDTH={pixels}&HEIGHT={pixels}&LAYERS=Actueel_orthoHR&STYLES=&FORMAT=image/jpeg&DPI=96&MAP_RESOLUTION=96&FORMAT_OPTIONS=dpi:96";
        url = $"https://service.pdok.nl/hwh/luchtfotorgb/wms/v1_0?SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&BBOX={bbox}&CRS=EPSG:28992&WIDTH={pixels}&HEIGHT={pixels}&LAYERS=Actueel_orthoHR&STYLES=&FORMAT=image/jpeg&DPI=96&MAP_RESOLUTION=96&FORMAT_OPTIONS=dpi:96";

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            t = ((DownloadHandlerTexture)www.downloadHandler).texture;
            textureWidth = t.width;
            GetComponent<Renderer>().material.mainTexture = t;
        }
    }


}
