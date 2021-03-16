﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.Configuration
{
    public class ApplicationConfiguration : MonoBehaviour
    {
        [Tooltip("Specific configuration file with custom external data paths and viewer options.\nTo create a config file right click in the assets menu and select ScriptableObjects/ConfigurationFile")]

        [SerializeField]
        private ConfigurationFile configurationFile;

        void Awake()
        {
            Config.configurationFile = configurationFile;
        }
    }
}
