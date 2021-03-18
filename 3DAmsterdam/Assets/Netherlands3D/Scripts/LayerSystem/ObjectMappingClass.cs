﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.LayerSystem
{
    public class ObjectMappingClass : ScriptableObject
    {
        public List<string> ids;
        public List<int> triangleCount;
        public List<Vector2> mappedUVs;
        public Vector2[] uvs;
        public List<int> vectorMap;
    }
}