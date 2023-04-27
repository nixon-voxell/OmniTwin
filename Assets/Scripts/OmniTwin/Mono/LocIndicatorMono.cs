using UnityEngine;

namespace OmniTwin
{
    [RequireComponent(typeof(MeshRenderer))]
    public class LocIndicatorMono : MonoBehaviour
    {
        [SerializeField, ColorUsage(true, true)] private Color m_NormalColor;
        [SerializeField, ColorUsage(true, true)] private Color m_HighlightColor;

        private Material m_mat_Indicator;
        private bool m_Highlight;

        public void Highlight()
        {
            this.m_Highlight = true;
        }

        private void Start()
        {
            MeshRenderer renderer = this.GetComponent<MeshRenderer>();
            this.m_mat_Indicator = renderer.material;
            this.m_Highlight = false;
        }

        private void Update()
        {
            if (this.m_Highlight)
            {
                this.m_mat_Indicator.SetColor("_Color", this.m_NormalColor);
            } else
            {
                this.m_mat_Indicator.SetColor("_Color", this.m_HighlightColor);
            }

            this.m_Highlight = false;
        }
    }
}
