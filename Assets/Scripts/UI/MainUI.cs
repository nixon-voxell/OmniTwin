using UnityEngine;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    [SerializeField] private UIDocument m_Document;
    private Button m_ModeBtn;
    private Button m_AlertBtn;
    private Button m_WarningBtn;
    private Button m_RecommendationBtn;

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

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        m_ModeBtn = root.Q<Button>("mode-btn");
        m_ModeBtn.clicked += ModeBtnPressed;

        m_AlertBtn = root.Q<Button>("alert-btn");
        m_AlertBtn.clicked += AlertBtnPressed;

        m_WarningBtn = root.Q<Button>("warning-btn");
        m_WarningBtn.clicked += WarningBtnPressed;

        m_RecommendationBtn = root.Q<Button>("recommend-btn");
        m_RecommendationBtn.clicked += RecommendationBtnPressed;
    }
}
