using ConvertCoordinates;
using Netherlands3D;
using Netherlands3D.Interface;
using Netherlands3D.Interface.SidePanel;
using Netherlands3D.LayerSystem;
using System;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;
using Netherlands3D.Interface.Layers;

public class PerceelRenderer : MonoBehaviour
{
    public Material LineMaterial;
    public static PerceelRenderer Instance;
    public GameObject TerrainLayer;
    public GameObject BuildingsLayer;

    public Transform HuisRichtingCube1;
    public Transform HuisRichtingCube2;
    public Transform HuisRichtingCube3;
    private Transform huisRichtingCube;


    public GameObject Uitbouw;
    public InterfaceLayer BuildingInterfaceLayer;

    [SerializeField]
    private Text MainTitle;

    [SerializeField]
    private Transform GeneratedFieldsContainer;

    //public delegate void BuildingMetaDataLoadedEventHandler(object source, ObjectDataEventArgs args);
    //public event BuildingMetaDataLoadedEventHandler BuildingMetaDataLoaded;

    //public delegate void PerceelDataLoadedEventHandler(object source, PerceelDataEventArgs args);
    //public event PerceelDataLoadedEventHandler PerceelDataLoaded;

    private float terrainFloor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PropertiesPanel.Instance.SetDynamicFieldsTargetContainer(GeneratedFieldsContainer);
        MainTitle.text = "Uitbouw plaatsen";

        PropertiesPanel.Instance.AddSpacer();
        PropertiesPanel.Instance.AddActionCheckbox("Toon alle gebouwen", true, (action) =>
        {
            BuildingInterfaceLayer.ToggleLinkedObject(action);
        });
        PropertiesPanel.Instance.AddSpacer();

        MetadataLoader.Instance.PerceelDataLoaded += Instance_PerceelDataLoaded;
    }

    private void Instance_PerceelDataLoaded(object source, PerceelDataEventArgs args)
    {
        StartCoroutine(RenderPolygons(args.Perceel));
    }
    /*
    public IEnumerator GetAndRenderPerceel(Vector3RD position)
    {
        yield return null;

        Debug.Log($"GetAndRenderPerceel x:{position.x} y:{position.y}");

        var bbox = $"{ position.x - 0.5},{ position.y - 0.5},{ position.x + 0.5},{ position.y + 0.5}";
        var url = $"https://geodata.nationaalgeoregister.nl/kadastralekaart/wfs/v4_0?SERVICE=WFS&REQUEST=GetFeature&VERSION=2.0.0&TYPENAMES=kadastralekaartv4:perceel&STARTINDEX=0&COUNT=1&SRSNAME=urn:ogc:def:crs:EPSG::28992&BBOX={bbox},urn:ogc:def:crs:EPSG::28992&outputFormat=json";

        UnityWebRequest req = UnityWebRequest.Get(url);        
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        {
            WarningDialogs.Instance.ShowNewDialog("Perceel data kon niet opgehaald worden");
        }
        else
        {
           
        }
    }

    IEnumerator UpdateSidePanelAddress(string bagId)
    {
        Debug.Log($"UpdateSidePanelAddress");

        var url = $"https://api.bag.kadaster.nl/lvbag/individuelebevragingen/v2/adressen?pandIdentificatie={bagId}";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("X-Api-Key", "l772bb9814e5584919b36a91077cdacea7");
        req.SetRequestHeader("Accept-Crs", "epsg:28992");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        {
            WarningDialogs.Instance.ShowNewDialog("Perceel data kon niet opgehaald worden");
        }
        else
        {
            var json = JSON.Parse(req.downloadHandler.text);
            var addresses = json["_embedded"]["adressen"];

            foreach(JSONObject adres in addresses)
            {
                var kortenaam = adres["korteNaam"].Value;
                var huisnummer = adres["huisnummer"].Value;
                var postcode = adres["postcode"].Value;
                var plaats = adres["woonplaatsNaam"].Value;
                PropertiesPanel.Instance.AddTextfield($"{kortenaam} {huisnummer}\n{postcode} {plaats}");
            }
        }

    }
    */
    /*
    ActionButton btn;
    Transform uitbouwTransform;
    Vector3 startPosition;

    
    void UpdateSidePanelPerceelData(JSONNode json)
    {
        Debug.Log("UpdateSidePanelPerceelData");

        JSONNode feature = json["features"][0];

        var perceelGrootte = $"Perceeloppervlakte: {feature["properties"]["kadastraleGrootteWaarde"]} m2";
        PropertiesPanel.Instance.AddLabel(perceelGrootte);

        btn = PropertiesPanel.Instance.AddActionButtonBigRef("Plaats uitbouw", (action) =>
        {
            PlaatsUitbouw();
        });
        
        
        var perceelGrootte = $"Perceeloppervlakte: {wfs.features[0].properties.kadastraleGrootteWaarde} m2";
        PropertiesPanel.Instance.AddLabel(perceelGrootte);

        btn = PropertiesPanel.Instance.AddActionButtonBigRef("Plaats uitbouw", (action) =>
        {
            var rd = new Vector2RD(wfs.features[0].properties.perceelnummerPlaatscoordinaatX, wfs.features[0].properties.perceelnummerPlaatscoordinaatY);
            var pos = CoordConvert.RDtoUnity(rd);
            InstantiateUitbouw(pos);

        });
    }*/

    public void PlaatsUitbouw(Vector2RD rd)
    {
        var pos = CoordConvert.RDtoUnity(rd);

        var uitbouw = Instantiate(Uitbouw, pos, Quaternion.identity);

        //uitbouwTransform = uitbouw.transform;

        //snap uitbouw tegen gebouw
        //uitbouw.transform.rotation = huisRichtingCube.rotation;
        //var halfHuisRichtingCube = (huisRichtingCube.localScale.z / 2);
        //var halfUitbouw = (uitbouw.transform.localScale.z / 2);
        //var snappedPos = huisRichtingCube.position - (huisRichtingCube.forward * (halfHuisRichtingCube + halfUitbouw));
        //snappedPos.y = terrainFloor + uitbouw.transform.localScale.y / 2;
        //uitbouw.transform.position = snappedPos;
        //startPosition = snappedPos;

        //Invoke("RemoveButton", 0.3f); //todo why the delay

        //PropertiesPanel.Instance.AddLabel("Draai de uitbouw");
        //PropertiesPanel.Instance.AddActionSlider("", "", 0, 1, 0.5f, (value) => {
        //    uitbouwTransform.rotation = Quaternion.Euler(0, value * 360, 0);
        //}, false, "Draai de uitbouw");

        //PropertiesPanel.Instance.AddLabel("Verplaats de uitbouw");
        //PropertiesPanel.Instance.AddActionSlider("", "", -10, 10, 0, (value) =>
        //{
        //    uitbouwTransform.position = startPosition + (uitbouwTransform.forward * value);
        //}, false, "Draai de uitbouw");
    }

    private void InstantiateUitbouw(Vector3 pos)
    {
        pos.y = terrainFloor + Uitbouw.transform.localScale.y / 2;
        var uitbouw = Instantiate(Uitbouw, pos, Quaternion.identity);
        //startPosition = pos;
        //uitbouwTransform = uitbouw.transform;

        Invoke("RemoveButton", 0.3f); //todo: why with delay?
    }
    /*
    void RemoveButton()
    {
        Destroy(btn.gameObject);
    }

    public IEnumerator HandlePosition(Vector3RD position, string id)
    {
        StartCoroutine(UpdateSidePanelAddress(id));

        yield return new WaitForSeconds(1);

        yield return StartCoroutine(GetAndRenderPerceel(position));

        yield return StartCoroutine(HighlightBuilding(id));

    }

    IEnumerator HighlightBuilding(string id)
    {
        Debug.Log($"HighlightBuilding id: {id}");

        yield return null;

        bool hasRD = false;

        Vector3RD rd = new Vector3RD();
        Transform child = null;

        while (!hasRD)
        {
            child = BuildingsLayer.transform.GetChild(0);
            rd = child.name.GetRDCoordinate();
            if (rd.x != 0) hasRD = true;
            yield return null;
        }


        StartCoroutine(DownloadObjectData(rd, id, child.gameObject));

    }

    
    IEnumerator HighlightBuildingArea(List<Vector2[]> polygons, Vector3 position)
    {
        yield return null;
        var q = from i in polygons
                from p in i
                select CoordConvert.RDtoUnity(p) into v3
                select new Vector2(v3.x, v3.z);

        var polyPoints = q.ToArray();

        foreach (var point in polyPoints)
        {
          //  Debug.Log($"pointx:{point.x} pointy:{point.y}"); //unity absolute 
        }

        foreach (Transform gam in BuildingsLayer.transform)
        {
            var rd = gam.name.GetRDCoordinate();
            var tile = CoordConvert.RDtoUnity(rd);
            //Debug.Log($"tilex: {tile.x} tilez: {tile.z}");

            var filter = gam.GetComponent<MeshFilter>();
            var verts = filter.mesh.vertices;

            var xmin = tile.x + verts.Min(o => o.x) + 500;
            var xmax = tile.x + verts.Max(o => o.x) + 500;
            var zmin = tile.z + verts.Min(o => o.z) + 500;
            var zmax = tile.z + verts.Max(o => o.z) + 500;

            //Debug.Log($"xmin:{xmin} xmax:{xmax} zmin:{zmin} zmax:{zmax}");  //unity absolute ( tile + relative verts pos)

            Color[] colors = new Color[filter.mesh.vertexCount];

            for (int i = 0; i < filter.mesh.vertexCount; i++)
            {
                var vertx = tile.x + verts[i].x + 500;
                var verty = tile.z + verts[i].z + 500;

                //var dist = Vector2.Distance(polyPoints[0], new Vector2(vertx, verty) );
                //distances.Add(dist);
                //check if vertexpoint is in the polygon, if so color it
                if (GeometryCalculator.ContainsPoint(polyPoints, new Vector2(vertx, verty)))   //unity absolute
                //if (dist < 100)
                {
                    //colors[i] = new Color(1, (dist / 100), (dist / 100));
                    colors[i] = Color.red;
                }
                else
                {
                    colors[i] = Color.white;
                }

            }

            filter.mesh.colors = colors;

            //Debug.Log($"building gameobject: {gam.name}"  );
        }
    }

    void RenderPolygonMesh(List<Vector2[]> polygons)
    {
        var perceelGameObject = new GameObject();

        var go = new GameObject();
        go.name = "Perceel mesh";
        go.transform.parent = perceelGameObject.transform;
        ProBuilderMesh m_Mesh = go.gameObject.AddComponent<ProBuilderMesh>();
        go.GetComponent<MeshRenderer>().material = LineMaterial;

        List<Vector3> verts = new List<Vector3>();

        foreach (var points in polygons)
        {
            foreach (var point in points)
            {
                var pos = CoordConvert.RDtoUnity(point);
                verts.Add(pos);
            }
        }

        m_Mesh.CreateShapeFromPolygon(verts.ToArray(), 5, false);  // TODO place op top of maaiveld            


        var go_rev = new GameObject();
        go_rev.transform.parent = perceelGameObject.transform;
        go_rev.name = "Perceel mesh_rev";
        ProBuilderMesh m_Mesh_rev = go_rev.gameObject.AddComponent<ProBuilderMesh>();
        go_rev.GetComponent<MeshRenderer>().material = LineMaterial;
        verts.Reverse();
        m_Mesh_rev.CreateShapeFromPolygon(verts.ToArray(), 5, false);

    }
    */

    IEnumerator RenderPolygons(List<Vector2[]> polygons)
    {

        List<Vector2> vertices = new List<Vector2>();
        List<int> indices = new List<int>();

        int count = 0;
        foreach (var list in polygons)
        {
            for (int i = 0; i < list.Length - 1; i++)
            {
                indices.Add(count + i);
                indices.Add(count + i + 1);
            }
            count += list.Length;
            vertices.AddRange(list);
        }

        GameObject gam = new GameObject();
        gam.name = "Perceel";
        gam.transform.parent = transform;

        MeshFilter filter = gam.AddComponent<MeshFilter>();
        gam.AddComponent<MeshRenderer>().material = LineMaterial;

        var verts = vertices.Select(o => CoordConvert.RDtoUnity(o)).ToArray();

        List<float> terrainFloorPoints = new List<float>();

        while (!terrainFloorPoints.Any())
        {
            //use collider to place the polygon points on the terrain
            for (int i = 0; i < verts.Length; i++)
            {
                var point = verts[i];
                var from = new Vector3(point.x, point.y + 10, point.z);


                Ray ray = new Ray(from, Vector3.down);
                RaycastHit hit;

                yield return null;

                //raycast from the polygon point to hit the terrain so we can place the outline so that it is visible
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    //Debug.Log("we have a hit");
                    verts[i].y = hit.point.y + 0.5f;
                    terrainFloorPoints.Add(hit.point.y);
                }
                else
                {
                    Debug.Log("raycast failed..");
                }
            }
            yield return null;
        }

        terrainFloor = terrainFloorPoints.Average();

        var mesh = new Mesh();
        mesh.vertices = verts;
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
        filter.sharedMesh = mesh;
    }

}
