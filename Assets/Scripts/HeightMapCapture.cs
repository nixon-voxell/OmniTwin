using UnityEngine;
using UnityEngine.Rendering;

public class HeightMapCapture : MonoBehaviour
{
    public RenderTexture heightMapTexture;
    public Texture2D heightMapTexture2D;
    public Camera targetCamera; // Reference to the orthographic camera

    public void Start()
    {
        // Initialize the RenderTexture
        int textureWidth = Screen.width;
        int textureHeight = Screen.height;

        heightMapTexture = new RenderTexture(textureWidth, textureHeight, 32, RenderTextureFormat.Depth);
        heightMapTexture.enableRandomWrite = true;
        heightMapTexture.Create();

        // Initialize the Texture2D
        heightMapTexture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.RFloat, false);
    }

    private void Update()
    {
        targetCamera.depthTextureMode = DepthTextureMode.Depth;
        RenderTexture.active = heightMapTexture;  // Set the Render Texture as the active render target
        targetCamera.targetTexture = heightMapTexture;
        targetCamera.Render();                     // Render the scene from the target orthographic camera's perspective
        targetCamera.targetTexture = null;

        CommandBuffer cmd = new CommandBuffer();
        cmd.CopyTexture(heightMapTexture.depthBuffer, this.heightMapTexture2D);

        // ConvertToGrayscale();
    }

    private void ConvertToGrayscale()
    {
        RenderTexture.active = heightMapTexture;  // Set the Render Texture as the active render target
        heightMapTexture2D.ReadPixels(new Rect(0, 0, heightMapTexture.width, heightMapTexture.height), 0, 0);
        heightMapTexture2D.Apply();
        RenderTexture.active = null;              // Reset the active render target

        // Now you can use 'heightMapTexture2D' as a grayscale height map
    }
}
