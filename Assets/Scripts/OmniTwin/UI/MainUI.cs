using UnityEngine;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    [SerializeField] private UIDocument m_Document;

    private VisualElement m_Root;
    private Button m_ModeBtn;
    private Button m_AlertBtn;
    private Button m_WarningBtn;
    private Button m_RecommendationBtn;

    public VisualElement Root => this.m_Root;

    private void ModeBtnPressed()
    {
        Debug.Log("Mode Button Pressed");
    }

    private void AlertBtnPressed()
    {
        Debug.Log("Alert Button Pressed");
    }

    private void WarningBtnPressed()
    {
        Debug.Log("Warning Button Pressed");
    }

    private void RecommendationBtnPressed()
    {
        Debug.Log("Recommendation Button Pressed");
    }

    private void OnEnable()
    {
        UIManager.Instance.MainUI = this;

        this.m_Root = this.m_Document.rootVisualElement;

        m_ModeBtn = this.m_Root.Q<Button>("mode-btn");
        m_ModeBtn.clicked += ModeBtnPressed;

        m_AlertBtn = this.m_Root.Q<Button>("alert-btn");
        m_AlertBtn.clicked += AlertBtnPressed;

        m_WarningBtn = this.m_Root.Q<Button>("warning-btn");
        m_WarningBtn.clicked += WarningBtnPressed;

        m_RecommendationBtn = this.m_Root.Q<Button>("recommend-btn");
        m_RecommendationBtn.clicked += RecommendationBtnPressed;
    }
}
