using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture
{
    public static class CoordinateFilterEditor
    {
        public static bool Editor(CoordinateFilter filter)
        {
            bool updated = false;
            var changedType = (CoordinateFilter.Type)EditorGUILayout.EnumPopup(filter.type);

            if (changedType != filter.type) updated = true;
            filter.type = changedType;

            switch (filter.type)
            {
                case CoordinateFilter.Type.Circle:
                    break;
            }

            return updated;
        }
    }
}
