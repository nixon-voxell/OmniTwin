using UnityEngine;
using Unity.Mathematics;
using CesiumForUnity;

namespace OmniTwin
{
    public class HybridDynCameraMono : MonoBehaviour
    {
        [SerializeField] private CesiumGlobeAnchor m_CesiumGlobeAnchor;
        [SerializeField] private float3 m_CameraSpeed;

        private void Awake()
        {
            OmniWorld.CesiumGlobeAnchor = this.m_CesiumGlobeAnchor;
            OmniWorld.CameraSpeed = this.m_CameraSpeed;
        }
    }
}
