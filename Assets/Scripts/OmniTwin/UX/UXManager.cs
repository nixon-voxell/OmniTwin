using System.Collections.Generic;
using UnityEngine;
using Voxell.Util;

namespace OmniTwin.UI
{
    public class UXManager : MonoBehaviour
    {
        public static UXManager Instance;

        [InspectOnly] public MainUI MainUI;
        [InspectOnly] public SideBarUI SideBarUI;
        [InspectOnly] public DetectionImageUI DetectionImageUI;
        [InspectOnly] public FloodVisualizationUI FloodVisualizationUI;

        [SerializeField] private GameObject m_Indicators;
        [SerializeField] private GameObject m_LocIndicators;
        [SerializeField] private GameObject m_FloodIndicators;
        private HashSet<IVisibility> m_ActiveUIs;

        public GameObject Indicators => this.m_Indicators;
        public GameObject LocIndicators => this.m_LocIndicators;
        public GameObject FloodIndicators => this.m_FloodIndicators;

        public void SetUIActive(IVisibility visibilityUI)
        {
            visibilityUI.SetVisible(true);
            this.m_ActiveUIs.Add(visibilityUI);
        }

        public void SetUIInactive(IVisibility visibilityUI)
        {
            visibilityUI.SetVisible(false);
            this.m_ActiveUIs.Remove(visibilityUI);
        }

        public void DisableAllActiveUI()
        {
            foreach (IVisibility visibilityUI in this.m_ActiveUIs)
            {
                visibilityUI.SetVisible(false);
            }

            this.m_ActiveUIs.Clear();
        }

        public bool IsUIActive(IVisibility visibilityUI)
        {
            return this.m_ActiveUIs.Contains(visibilityUI);
        }

        public void ShowSideBarWithInfo(string location, double lon, double lat)
        {
            this.SetUIActive(this.SideBarUI);
            this.SideBarUI.UpateDetails(location, lon, lat);
        }

        public void HideSidebar()
        {
            this.SetUIInactive(this.SideBarUI);
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

            this.m_ActiveUIs = new HashSet<IVisibility>(16);
        }
    }
}
