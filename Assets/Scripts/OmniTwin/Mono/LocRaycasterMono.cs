using UnityEngine;

namespace OmniTwin
{
    public class LocRaycasterMono : MonoBehaviour
    {
        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Collider collider = hit.collider;
                if (collider.CompareTag("LocationIndicator"))
                {
                    LocIndicatorMono indicator = collider.GetComponent<LocIndicatorMono>();
                    indicator.Highlight();
                }
            }
        }
    }
}
