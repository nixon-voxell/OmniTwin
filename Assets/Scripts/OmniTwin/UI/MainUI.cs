using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using Unity.Mathematics;
using CesiumForUnity;
using OmniTwin;

public class MainUI : MonoBehaviour
{
    [SerializeField] private UIDocument m_Document;
    [SerializeField] private float m_CamMovementSpeed;
    [SerializeField] private float m_CamRotationSpeed;
    [SerializeField] private float m_RainVFXSpeed;

    private VisualElement m_Root;

    private Button m_BackBtn;
    private Button m_AlertBtn;
    private Button m_WarningBtn;
    private Button m_RecommendationBtn;

    private Button m_LocBtn;
    private Button m_DisasterBtn;

    public VisualElement Root => this.m_Root;
    public Button BackBtn => this.m_BackBtn;

    public void MoveCameraToDisaster(DisasterData disasterData, double3 targetGeoCoord, float3 targetEuler)
    {
        OmniWorld.IsCameraLocked = true;

        this.StartCoroutine(this.MoveCameraToOffset(targetGeoCoord, this.m_CamMovementSpeed));
        this.StartCoroutine(this.RotateCameraToEuler(targetEuler, this.m_CamRotationSpeed));

        if (disasterData.Category == "Flood")
        {
            this.StartCoroutine(this.StartRainFall(this.m_RainVFXSpeed));
        }

        this.BackBtn.visible = true;
    }

    public void LoadDetectionImage(DisasterData disasterData)
    {
        // load detection image
        UIManager.Instance.DetectionImageUI.SetDocumentVisible(true);
        this.StartCoroutine(this.LoadDisasterTexture(disasterData.DetectionImageURL));
    }

    private IEnumerator MoveCameraToOffset(double3 targetGeoCoord, float movementSpeed)
    {
        CesiumGlobeAnchor anchor = OmniWorld.HybridDynCameraMono.CesiumGlobeAnchor;

        while (math.distance(anchor.longitudeLatitudeHeight, targetGeoCoord) > 0.01d)
        {
            anchor.longitudeLatitudeHeight = math.lerp(
                anchor.longitudeLatitudeHeight, targetGeoCoord,
                Time.deltaTime * movementSpeed
            );

            yield return null;
        }
    }

    private IEnumerator RotateCameraToEuler(float3 targeEuler, float rotationSpeed)
    {
        CesiumGlobeAnchor anchor = OmniWorld.HybridDynCameraMono.CesiumGlobeAnchor;
        quaternion targetRotation = quaternion.Euler(math.radians(targeEuler));

        while (math.distance(anchor.rotationEastUpNorth.value, targetRotation.value) > 0.01d)
        {
            anchor.rotationEastUpNorth = math.slerp(
                anchor.rotationEastUpNorth, targetRotation,
                Time.deltaTime * rotationSpeed
            );

            yield return null;
        }
    }

    private IEnumerator StartRainFall(float vfxSpeed)
    {
        float t = OmniWorld.HybridDynCameraMono.RainVFX.GetFloat("Intensity");

        // perform a linear lerp
        while (t < 1.0f)
        {
            t += Time.deltaTime * vfxSpeed;
            OmniWorld.HybridDynCameraMono.RainVFX.SetFloat("Intensity", math.saturate(t));
            yield return null;
        }
    }

    private IEnumerator StopRainFall(float vfxSpeed)
    {
        float t = OmniWorld.HybridDynCameraMono.RainVFX.GetFloat("Intensity");

        // perform a linear lerp
        while (t > 0.0f)
        {
            t -= Time.deltaTime * vfxSpeed;
            OmniWorld.HybridDynCameraMono.RainVFX.SetFloat("Intensity", math.saturate(t));
            yield return null;
        }
    }

    private IEnumerator LoadDisasterTexture(string imageURL)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning(request.error);
        } else
        {
            UIManager.Instance.DetectionImageUI.SetTexture(((DownloadHandlerTexture)(request.downloadHandler)).texture);
        }
    }

    private void Start()
    {
        UIManager.Instance.MainUI = this;

        this.m_Root = this.m_Document.rootVisualElement;

        this.m_BackBtn = this.m_Root.Q<Button>("back-btn");
        this.m_AlertBtn = this.m_Root.Q<Button>("alert-btn");
        this.m_WarningBtn = this.m_Root.Q<Button>("warning-btn");
        this.m_RecommendationBtn = this.m_Root.Q<Button>("recommend-btn");

        this.m_LocBtn = this.m_Root.Q<Button>("loc-btn");
        this.m_DisasterBtn = this.m_Root.Q<Button>("disaster-btn");

        this.BackBtn.clicked += () =>
        {
            UIManager manager = UIManager.Instance;
            // stop all animations
            this.StopAllCoroutines();
            // stop rain
            this.StartCoroutine(this.StopRainFall(this.m_RainVFXSpeed));
            // move camera back to top down view
            OmniWorld.IsCameraLocked = false;
            // reenable indicators
            manager.Indicators.SetActive(true);
            manager.DetectionImageUI.SetDocumentVisible(false);

            this.BackBtn.visible = false;
        };

        this.m_LocBtn.clicked += () =>
        {
            UIManager manager = UIManager.Instance;
            manager.LocIndicators.SetActive(true);
            manager.FloodIndicators.SetActive(false);
        };

        this.m_DisasterBtn.clicked += () =>
        {
            UIManager manager = UIManager.Instance;
            manager.LocIndicators.SetActive(false);
            manager.FloodIndicators.SetActive(true);
            manager.SideBarUI.Close();
        };
    }
}
