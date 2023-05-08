using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;
using CesiumForUnity;
using OmniTwin;

[RequireComponent(typeof(UIDocument), typeof(CesiumGlobeAnchor))]
public class DisasterIndicatorUI : MonoBehaviour
{
    [SerializeField] private UIDocument m_Document;
    [SerializeField] private CesiumGlobeAnchor m_Anchor;
    [SerializeField] private double3 m_CamOffset;
    [SerializeField] private float3 m_CamEuler;

    private VisualElement m_Root;

    private VisualElement m_Indicator;
    private Button m_DisasterBtn;
    private DisasterData m_DisasterData;

    private void OnEnable()
    {
        if (this.m_DisasterData.DetectionImageURL != null)
        {
            this.Init(this.m_DisasterData);
        }
    }

    public void Init(DisasterData disasterData)
    {
        this.m_DisasterData = disasterData;
        this.m_Root = this.m_Document.rootVisualElement;

        this.m_Indicator = this.m_Root.Q<VisualElement>("indicator");
        this.m_DisasterBtn = this.m_Root.Q<Button>("disaster-btn");

        this.m_DisasterBtn.clicked += () =>
        {
            UIManager manager = UIManager.Instance;
            manager.Indicators.SetActive(false);

            // offset geo coordinate
            CesiumGlobeAnchor anchor = OmniWorld.HybridDynCameraMono.CesiumGlobeAnchor;
            double3 targetGeoCoord = anchor.longitudeLatitudeHeight;
            targetGeoCoord.xy += this.m_CamOffset.xy;
            targetGeoCoord.z = this.m_CamOffset.z;

            manager.MainUI.MoveCameraToDisaster(disasterData, targetGeoCoord, this.m_CamEuler);
            manager.MainUI.LoadDetectionImage(disasterData);
        };

        this.m_Anchor.longitudeLatitudeHeight = new double3(
            this.m_DisasterData.Longitude, this.m_DisasterData.Latitude, 0.0d
        );
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
