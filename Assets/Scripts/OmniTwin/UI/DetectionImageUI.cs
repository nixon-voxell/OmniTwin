using UnityEngine;
using UnityEngine.UIElements;

public class DetectionImageUI : MonoBehaviour
{
    [SerializeField] private UIDocument m_Document;
    [SerializeField] private Texture2D m_DetectionTexture;

    private VisualElement m_Root;
    private VisualElement m_DetectionImg;

    public void SetDocumentVisible(bool active)
    {
        this.m_Root.visible = active;
    }

    public void SetTexture(Texture2D texture)
    {
        this.m_DetectionTexture = texture;
        this.m_DetectionImg.style.backgroundImage = new StyleBackground
        {
            value = new Background
            {
                texture = texture
            }
        };
    }

    private void Start()
    {
        UIManager.Instance.DetectionImageUI = this;

        this.m_Root = this.m_Document.rootVisualElement;
        this.m_DetectionImg = this.m_Root.Q<VisualElement>("detection-img");

        // set visibility to false by default
        this.SetDocumentVisible(false);
    }
}
