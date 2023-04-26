using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class mainUI : MonoBehaviour
{
    public UIDocument uiDoc;
    private Button modeButton;
    private Button detailsButton;
    private Button alertButton;
    private Button warningButton;
    private Button recommendationButton;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        modeButton = root.Q<Button>("Mode");
        detailsButton = root.Q<Button>("Details");
        alertButton = root.Q<Button>("Alert");
        warningButton = root.Q<Button>("Warning");
        recommendationButton = root.Q<Button>("Recommendation");
        modeButton.clicked += ModeButtonPressed;
        detailsButton.clicked += DetailsButtonPressed;
        alertButton.clicked += AlertButtonPressed;
        warningButton.clicked += WarningButtonPressed;
        recommendationButton.clicked += RecommendationButtonPressed;
    }

    void ModeButtonPressed()
    {
        Debug.Log("Mode Button Pressed");
    }
    void DetailsButtonPressed()
    {
        Debug.Log("Details Button Pressed");
    }
    void AlertButtonPressed()
    {
        Debug.Log("Alert Button Pressed");
    }
    void WarningButtonPressed()
    {
        Debug.Log("Warning Button Pressed");
    }
    void RecommendationButtonPressed()
    {
        Debug.Log("Recommendation Button Pressed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
