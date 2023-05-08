using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using CesiumForUnity;

namespace OmniTwin
{
    public partial struct CameraPanningSystem : ISystem, ISystemStartStop
    {
        private bool m_Dragging;
        private float3 m_DragStartMouse;
        private double3 m_DragStartGeoCoord;

        private double3 m_TargetGeoCoord;
        private double3 m_CurrGeoCoord;

        public void OnCreate(ref SystemState state)
        {
            OmniWorld.IsCameraLocked = false;
        }

        public void OnStartRunning(ref SystemState state)
        {
            HybridDynCameraMono hybridDynCam = OmniWorld.HybridDynCameraMono;
            this.m_Dragging = false;
            this.m_CurrGeoCoord = hybridDynCam.CesiumGlobeAnchor.longitudeLatitudeHeight;
            this.m_TargetGeoCoord = this.m_CurrGeoCoord;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (OmniWorld.IsCameraLocked == true) return;
            HybridDynCameraMono hybridDynCam = OmniWorld.HybridDynCameraMono;

            // 200.0f is the default height (magic number per se)
            float height = (float)this.m_TargetGeoCoord.z / 200.0f;

            if (Input.GetMouseButtonDown(0))
            {
                this.m_Dragging = true;
                this.m_DragStartMouse = Input.mousePosition;
                this.m_DragStartGeoCoord = this.m_CurrGeoCoord;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.m_Dragging = false;
            }

            if (this.m_Dragging)
            {
                float3 dragDiff = (float3)Input.mousePosition - this.m_DragStartMouse;
                this.m_TargetGeoCoord = this.m_DragStartGeoCoord - dragDiff * hybridDynCam.CameraSpeed * height;
            }

            this.m_TargetGeoCoord.z -= (Input.mouseScrollDelta.y * hybridDynCam.CameraSpeed.z) * height;
            this.m_TargetGeoCoord = math.clamp(this.m_TargetGeoCoord, hybridDynCam.MinPos, hybridDynCam.MaxPos);

            this.m_CurrGeoCoord = math.lerp(
                this.m_CurrGeoCoord, this.m_TargetGeoCoord,
                SystemAPI.Time.DeltaTime * OmniWorld.HybridDynCameraMono.CamMovementSmoothing
            );

            // get globe anchor from singleton
            CesiumGlobeAnchor anchor = hybridDynCam.CesiumGlobeAnchor;
            anchor.longitudeLatitudeHeight = this.m_CurrGeoCoord;
            anchor.rotationEastUpNorth = math.slerp(
                anchor.rotationEastUpNorth, quaternion.Euler(math.radians(90.0f), 0.0f, 0.0f),
                Time.deltaTime * OmniWorld.HybridDynCameraMono.CamRotationSmoothing
            );
        }

        public void OnStopRunning(ref SystemState state)
        {
            
        }

        public void OnDestroy(ref SystemState state)
        {
        }
    }
}
