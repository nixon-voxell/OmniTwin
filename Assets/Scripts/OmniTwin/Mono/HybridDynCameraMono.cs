using UnityEngine;
using CesiumForUnity;

public class HybridDynCameraMono : MonoBehaviour
{
    public static HybridDynCameraMono Instance;
    private CesiumGlobeAnchor m_CesiumGlobeAnchor;

    public CesiumGlobeAnchor CesiumGlobeAnchor => this.m_CesiumGlobeAnchor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Object.Destroy(this);
        }
    }
}
