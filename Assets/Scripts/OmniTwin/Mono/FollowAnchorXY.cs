using UnityEngine;
using Unity.Mathematics;
using CesiumForUnity;

public class FollowAnchorXY : MonoBehaviour
{
    public CesiumGlobeAnchor m_TargetAnchor;
    public CesiumGlobeAnchor m_SelfAnchor;

    private void LateUpdate()
    {
        double3 llh = this.m_TargetAnchor.longitudeLatitudeHeight;
        llh.z = this.m_SelfAnchor.longitudeLatitudeHeight.z;

        this.m_SelfAnchor.longitudeLatitudeHeight = llh;
    }
}
