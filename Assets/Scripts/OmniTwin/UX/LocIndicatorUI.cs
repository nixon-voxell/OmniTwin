using UnityEngine;
using UnityEngine.UIElements;
using CesiumForUnity;

namespace OmniTwin.UI
{
    [RequireComponent(typeof(UIDocument), typeof(CesiumGlobeAnchor))]
    public class LocIndicatorUI : MonoBehaviour
    {
        [SerializeField] private UIDocument m_Document;
        [SerializeField] private CesiumGlobeAnchor m_Anchor;
        [SerializeField] private string m_LocName;

        private VisualElement m_Root;

        private VisualElement m_Indicator;
        private Label m_LocLbl;
        private Button m_LocBtn;

        private void OnEnable()
        {
            this.m_Root = this.m_Document.rootVisualElement;

            this.m_Indicator = this.m_Root.Q<VisualElement>("indicator");
            this.m_LocLbl = this.m_Root.Q<Label>("loc-lbl");
            this.m_LocBtn = this.m_Root.Q<Button>("loc-btn");

            this.m_LocLbl.text = this.m_LocName;

            this.m_LocBtn.clicked += () =>
            {
                UXManager.Instance.ShowSideBarWithInfo(
                    this.m_LocName,
                    this.m_Anchor.longitudeLatitudeHeight.x,
                    this.m_Anchor.longitudeLatitudeHeight.y
                );
            };
        }

        private void LateUpdate()
        {
            Vector2 panelPos;
            panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(this.m_Root.panel, this.transform.position, Camera.main);

            float width = this.m_Indicator.contentRect.width;
            float height = this.m_Indicator.contentRect.height;

            this.m_Indicator.style.translate = new StyleTranslate(new Translate(panelPos.x - width * 0.5f, panelPos.y - height));
        }
    }
}
