using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWMSProjector : MonoBehaviour
{
    public Renderer TerrainRenderer;
    public GameObject WMSProjectorGameobject;

    Renderer ren;

    Color terrainColor;

    void Start()
    {
        ren = WMSProjectorGameobject.GetComponent<Renderer>();
        terrainColor = TerrainRenderer.material.color;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ren.enabled = !ren.enabled;
            TerrainRenderer.material.color = ren.enabled ? Color.white : terrainColor;

        }
    }
}
