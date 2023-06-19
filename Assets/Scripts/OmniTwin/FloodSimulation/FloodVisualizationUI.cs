using UnityEngine;
using UnityEngine.UIElements;

namespace OmniTwin.UI
{
    public class FloodVisualizationUI : MonoBehaviour, IVisibility
    {
        [SerializeField] private UIDocument m_Document;

        private VisualElement m_Root;
        private VisualElement m_VisualizationImg;

        private Label m_FrameLbl;

        private Button m_PauseBtn;
        private Button m_PlayBtn;
        private Button m_ResetBtn;
        private Button m_ScreenshotBtn;

        public VisualElement VisualizationImg => this.m_VisualizationImg;

        public void SetVisible(bool active)
        {
            this.m_Root.visible = active;
        }

        private void Start()
        {
            UIManager.Instance.FloodVisualizationUI = this;

            this.m_Root = this.m_Document.rootVisualElement;
            this.m_VisualizationImg = this.m_Root.Q<VisualElement>("visualization-img");

            this.m_FrameLbl = this.m_Root.Q<Label>("frame-lbl");

            this.m_PauseBtn = this.m_Root.Q<Button>("pause-btn");
            this.m_PlayBtn = this.m_Root.Q<Button>("play-btn");
            this.m_ResetBtn = this.m_Root.Q<Button>("reset-btn");
            this.m_ScreenshotBtn = this.m_Root.Q<Button>("screenshot-btn");

            // set visibility to false by default
            this.SetVisible(false);
        }
    }
}
