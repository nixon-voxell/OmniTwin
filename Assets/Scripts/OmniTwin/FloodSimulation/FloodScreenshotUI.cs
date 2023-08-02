using UnityEngine;
using UnityEngine.UIElements;

namespace OmniTwin.UI
{
    public class FloodScreenshotUI : MonoBehaviour, IVisibility
    {
        [SerializeField] private UIDocument m_Document;

        private VisualElement m_Root;
        private VisualElement m_VisualizationImg;

        private Label m_FrameLbl;

        private Button m_PrevBtn;
        private Button m_NextBtn;

        public VisualElement VisualizationImg => this.m_VisualizationImg;

        public void SetFrameData(int index, int count)
        {
            this.m_FrameLbl.text = $"Frame: {index}/{count}";
            // TODO: set frame texture as well
        }

        public void SetVisible(bool active)
        {
            this.m_Root.visible = active;
        }

        private void Start()
        {
            UXManager.Instance.FloodScreenshotUI = this;

            this.m_Root = this.m_Document.rootVisualElement;
            this.m_VisualizationImg = this.m_Root.Q<VisualElement>("visualization-img");

            this.m_FrameLbl = this.m_Root.Q<Label>("frame-lbl");

            this.m_PrevBtn = this.m_Root.Q<Button>("prev-btn");
            this.m_NextBtn = this.m_Root.Q<Button>("next-btn");

            // set visibility to false by default
            this.SetVisible(false);
        }
    }
}
