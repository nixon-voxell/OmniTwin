using UnityEngine;
using Unity.Mathematics;
using CesiumForUnity;

namespace OmniTwin
{
    public class HybridDynCameraMono : MonoBehaviour
    {
        [SerializeField] private CesiumGlobeAnchor m_CesiumGlobeAnchor;
        [SerializeField] private float3 m_CameraSpeed;
        [SerializeField] private float m_CamSmoothing;

        public CesiumGlobeAnchor CesiumGlobeAnchor => this.m_CesiumGlobeAnchor;
        public double3 CameraSpeed => this.m_CameraSpeed;
        public float CamSmoothing => this.m_CamSmoothing;

        private void Awake()
        {
            if (OmniWorld.HybridDynCameraMono == null)
            {
                OmniWorld.HybridDynCameraMono = this;
            } else
            {
                Debug.LogError("There are more than 1 HybridDynCameraMono in the scene.");
            }
        }
    }
}
