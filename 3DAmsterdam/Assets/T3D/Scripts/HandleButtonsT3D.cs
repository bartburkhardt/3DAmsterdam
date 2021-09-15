using ConvertCoordinates;
using Netherlands3D.LayerSystem;
using Netherlands3D.Sun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HandleButtonsT3D : MonoBehaviour
{
    public GameObject BuildingsLayer;
    public GameObject Zonnepaneel;
    public Texture2D RotateIcon;

    public DropUp MaandenDropup;

    //Sun related
    public Text Dag;
    public Text Maand;
    public Text Tijd;
    private DateTime dateTimeNow;
    private double longitude;
    private double latitude;

    List<string> months = new List<string>()
    {
        "JAN","FEB","MRT","APR","MEI","JUN","JUL","AUG","SEP","OKT","NOV","DEC"
    };


    void Start()
    {
        //Cursor.SetCursor(RotateIcon, Vector2.zero, CursorMode.Auto);
                
        //Sun related
        dateTimeNow = DateTime.Now;        
        var coordinates = CoordConvert.UnitytoWGS84(Vector3.zero);
        longitude = coordinates.lon;
        latitude = coordinates.lat;

        UpdateTijd();

        MaandenDropup.SetItems(months, dateTimeNow.Month-1, SetMonth);
    }

    void SetMonth(int month)
    {
        dateTimeNow = new DateTime(dateTimeNow.Year, month+1, dateTimeNow.Day, dateTimeNow.Hour, dateTimeNow.Minute, 0);
        UpdateSun();
        UpdateTijd();      
    }

    public void ToggleBuildings()
    {
        var buildingsLayer = BuildingsLayer.GetComponent<Layer>();
        buildingsLayer.isEnabled = !buildingsLayer.isEnabled;
    }

    #region Sun related
    public void ToggleZonnepaneel()
    {
        Zonnepaneel.SetActive(!Zonnepaneel.active);
    }

    public void AddHour()
    {
        dateTimeNow = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, dateTimeNow.Hour, 0, 0);
        dateTimeNow =  dateTimeNow.AddHours(1);
        
        UpdateTijd();
        UpdateSun();
        Debug.Log(dateTimeNow);
    }

    public void MinusHour()
    {
        dateTimeNow = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, dateTimeNow.Hour, 0, 0);
        dateTimeNow = dateTimeNow.AddHours(-1);
        UpdateTijd();
        UpdateSun();
        Debug.Log(dateTimeNow);
    }

    private void UpdateTijd()
    {
        Dag.text = $"{dateTimeNow.Day}";
        Maand.text = GetMonthString(dateTimeNow.Month-1);        
        Tijd.text = $"{ dateTimeNow:HH:mm}";
    }

    void UpdateSun()
    {
        var angles = new Vector3();
        double alt;
        double azi;
        SunPosition.CalculateSunPosition(dateTimeNow, (double)latitude, (double)longitude, out azi, out alt);
        angles.x = (float)alt * Mathf.Rad2Deg;
        angles.y = (float)azi * Mathf.Rad2Deg;

        EnviromentSettings.SetSunAngle(angles);
    }

    string GetMonthString(int monthIndex)
    {
        return months[monthIndex];
    }



    #endregion

}
