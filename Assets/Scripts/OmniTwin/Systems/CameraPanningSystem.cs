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
        private float3 m_LongitudeLatitudeHeight;

        public void OnCreate(ref SystemState state)
        {
            this.m_Dragging = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            // get globe anchor from singleton
            CesiumGlobeAnchor globeAnchor = OmniWorld.CesiumGlobeAnchor;

            if (Input.GetMouseButtonDown(0))
            {
                this.m_Dragging = true;
                this.m_DragStartMouse = Input.mousePosition;
                this.m_LongitudeLatitudeHeight = (float3)globeAnchor.longitudeLatitudeHeight;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.m_Dragging = false;
            }

            if (this.m_Dragging)
            {
                float3 dragDiff = (float3)Input.mousePosition - this.m_DragStartMouse;
                globeAnchor.longitudeLatitudeHeight = (double3)(this.m_LongitudeLatitudeHeight - dragDiff * OmniWorld.CameraSpeed);
            }
        }

        public void OnDestroy(ref SystemState state)
        {
        }
}
}
