using UnityEngine;
using static Unity.Mathematics.math;

public class DisasterAnim : MonoBehaviour
{
    [SerializeField] private float m_GrowSpeed;
    private Vector3 m_TargetScale;

    private void OnEnable()
    {
        this.m_TargetScale = this.transform.localScale;
        this.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        Vector3 scale = this.transform.localScale;

        scale = Vector3.Lerp(
            scale, this.m_TargetScale,
            Time.deltaTime * this.m_GrowSpeed
        );

        if (distancesq(scale, this.m_TargetScale) < 1.0f)
        {
            scale = Vector3.zero;
        }

        this.transform.localScale = scale;
    }

    private void OnDisable()
    {
        this.transform.localScale = this.m_TargetScale;
    }
}
