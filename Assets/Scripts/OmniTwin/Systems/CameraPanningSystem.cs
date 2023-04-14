using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using CesiumForUnity;

namespace OmniTwin
{
    public partial struct CameraPanningSystem : ISystem
    {
        private bool m_Dragging;
        private float3 m_DragStartMouse;
        private double3 m_DragStartGeoCoord;

        public void OnCreate(ref SystemState state)
        {
            this.m_Dragging = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            // get globe anchor from singleton
            CesiumGlobeAnchor globeAnchor = OmniWorld.CesiumGlobeAnchor;
            // 200.0f is the default height (magic number per se)
            float height = (float)globeAnchor.longitudeLatitudeHeight.z / 200.0f;

            if (Input.GetMouseButtonDown(0))
            {
                this.m_Dragging = true;
                this.m_DragStartMouse = Input.mousePosition;
                this.m_DragStartGeoCoord = globeAnchor.longitudeLatitudeHeight;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.m_Dragging = false;
            }

            if (this.m_Dragging)
            {
                float3 dragDiff = (float3)Input.mousePosition - this.m_DragStartMouse;
                globeAnchor.longitudeLatitudeHeight = this.m_DragStartGeoCoord - dragDiff * OmniWorld.CameraSpeed * height;
            }

            double3 geoCoord = globeAnchor.longitudeLatitudeHeight;
            geoCoord.z -= (Input.mouseScrollDelta.y * OmniWorld.CameraSpeed.z) * height;
            globeAnchor.longitudeLatitudeHeight = geoCoord;
        }

        public void OnDestroy(ref SystemState state)
        {
        }
}
}
