using UnityEngine;
using UnityEngine.UIElements;

namespace OmniTwin.UI
{
    public class FloodVisualizationUI : MonoBehaviour
    {
        [SerializeField] private UIDocument m_Document;

        private VisualElement m_Root;
        private VisualElement m_VisualizationImg;

        public VisualElement VisualizationImg => this.m_VisualizationImg;

        public void SetDocumentVisible(bool active)
        {
            this.m_Root.visible = active;
        }

        private void Start()
        {
            UIManager.Instance.FloodVisualizationUI = this;

            this.m_Root = this.m_Document.rootVisualElement;
            this.m_VisualizationImg = this.m_Root.Q<VisualElement>("visualization-img");

            // set visibility to false by default
            this.SetDocumentVisible(false);
        }
    }
}
