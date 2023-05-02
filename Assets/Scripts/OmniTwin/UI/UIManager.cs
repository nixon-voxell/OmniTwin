using UnityEngine;
using Voxell.Util;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [InspectOnly] public MainUI MainUI;
    [InspectOnly] public SideBarUI SideBarUI;

    public void ShowSideBarWithInfo(string location, double lon, double lat)
    {
        this.SideBarUI.SetDocumentVisible(true);
        this.SideBarUI.UpateDetails(location, lon, lat);
    }

    public void HideSidebar()
    {
        this.SideBarUI.SetDocumentVisible(false);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Debug.Log("There is more than one UIManager in the scene.");
        }
    }
}
