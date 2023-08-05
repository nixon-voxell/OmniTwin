using UnityEngine;
using UnityEngine.UIElements;

namespace OmniTwin
{
    public static class UIElementUtil
    {
        public static void SetTexture(VisualElement visualElement, Texture2D texture)
        {
            visualElement.style.backgroundImage = new StyleBackground
            {
                value = new Background
                {
                    texture = texture
                }
            };
        }

        public static void SetTexture(VisualElement visualElement, RenderTexture texture)
        {
            visualElement.style.backgroundImage = new StyleBackground
            {
                value = new Background
                {
                    renderTexture = texture
                }
            };
        }
    }
}
