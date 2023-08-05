using UnityEngine;
using UnityEngine.UIElements;

namespace OmniTwin.UI
{
    public class DetectionImageUI : MonoBehaviour, IVisibility
    {
        [SerializeField] private UIDocument m_Document;

        private VisualElement m_Root;
        private VisualElement m_DetectionImg;

        public VisualElement DetectionImg => this.m_DetectionImg;

        public void SetVisible(bool active)
        {
            this.m_Root.visible = active;
        }

        private void Start()
        {
            UXManager.Instance.DetectionImageUI = this;

            this.m_Root = this.m_Document.rootVisualElement;
            this.m_DetectionImg = this.m_Root.Q<VisualElement>("detection-img");

            // set visibility to false by default
            this.SetVisible(false);
        }
    }
}
