using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private Vector3 m_Offset;

    private void Update()
    {
        this.transform.position = this.m_Target.position + this.m_Offset;
    }
}
