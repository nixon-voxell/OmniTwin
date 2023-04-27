using UnityEngine;
using CesiumForUnity;
using TMPro;

namespace OmniTwin
{
    public class LocRaycasterMono : MonoBehaviour
    {
        private LocIndicatorMono m_Indicator = null;

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Collider collider = hit.collider;
                if (collider.CompareTag("LocationIndicator"))
                {
                    this.m_Indicator = collider.GetComponent<LocIndicatorMono>();
                    this.m_Indicator.Highlight();
                } else
                {
                    this.m_Indicator = null;
                }
            }

            // check if a location is selected
            if (Input.GetMouseButtonUp(0))
            {
                if (this.m_Indicator != null)
                {
                    CesiumGlobeAnchor anchor = this.m_Indicator.GetComponent<CesiumGlobeAnchor>();
                    TextMeshProUGUI tmproUI = this.m_Indicator.GetComponentInChildren<TextMeshProUGUI>();

                    UIManager.Instance.ShowSideBarWithInfo(
                        tmproUI.text, anchor.longitudeLatitudeHeight.x, anchor.longitudeLatitudeHeight.y
                    );
                } else
                {
                    UIManager.Instance.HideSidebar();
                }
            }
        }
    }
}
