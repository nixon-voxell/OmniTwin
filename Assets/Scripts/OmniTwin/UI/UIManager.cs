using UnityEngine;
using Voxell.Util;

namespace OmniTwin.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [InspectOnly] public MainUI MainUI;
        [InspectOnly] public SideBarUI SideBarUI;
        [InspectOnly] public DetectionImageUI DetectionImageUI;
        [InspectOnly] public FloodVisualizationUI FloodVisualizationUI;

        [SerializeField] private GameObject m_Indicators;
        [SerializeField] private GameObject m_LocIndicators;
        [SerializeField] private GameObject m_FloodIndicators;

        public GameObject Indicators => this.m_Indicators;
        public GameObject LocIndicators => this.m_LocIndicators;
        public GameObject FloodIndicators => this.m_FloodIndicators;

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
}
