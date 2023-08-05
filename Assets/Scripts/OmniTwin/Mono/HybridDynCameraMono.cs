using UnityEngine;
using UnityEngine.VFX;
using Unity.Mathematics;
using CesiumForUnity;

namespace OmniTwin
{
    public class HybridDynCameraMono : MonoBehaviour
    {
        [SerializeField] private CesiumGlobeAnchor m_CesiumGlobeAnchor;
        [SerializeField] private float3 m_CameraSpeed;
        [SerializeField] private float m_CamMovementSmoothing;
        [SerializeField] private float m_CamRotationSmooting;
        [SerializeField] private double3 m_MinPos;
        [SerializeField] private double3 m_MaxPos;

        [SerializeField] private VisualEffect m_RainVFX;
        [SerializeField] private AudioSource m_RainSource;

        public CesiumGlobeAnchor CesiumGlobeAnchor => this.m_CesiumGlobeAnchor;
        public double3 CameraSpeed => this.m_CameraSpeed;
        public float CamMovementSmoothing => this.m_CamMovementSmoothing;
        public float CamRotationSmoothing => this.m_CamRotationSmooting;
        public double3 MinPos => this.m_MinPos;
        public double3 MaxPos => this.m_MaxPos;
        public VisualEffect RainVFX => this.m_RainVFX;
        public AudioSource RainSource => this.m_RainSource;

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
