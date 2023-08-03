using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;
using Voxell.Util;

namespace OmniTwin.UI
{
    public class FloodVisualizationUI : MonoBehaviour, IVisibility
    {
        [SerializeField] private UIDocument m_Document;
        [SerializeField] private FloodVisualization m_FloodVisualization;

        private VisualElement m_Root;
        private VisualElement m_SimulationImg;
        private VisualElement m_VisualizationImg;

        private Label m_SimFrameLbl;
        private Label m_ViewFrameLbl;

        private Button m_PauseBtn;
        private Button m_PlayBtn;
        private Button m_ResetBtn;
        private Button m_ScreenshotBtn;
        private Button m_PrevBtn;
        private Button m_NextBtn;

        [SerializeField, InspectOnly] private bool m_IsPlaying = false;
        private List<Screenshot> m_Screenshots;
        private int m_ScreenshotIndex;

        public VisualElement SimulationImg => this.m_SimulationImg;

        private void OffsetVisualizationImg(int offset)
        {
            this.SetVisualizationImg(this.m_ScreenshotIndex + offset);
        }

        private void SetVisualizationImg(int index)
        {
            this.m_ScreenshotIndex = math.clamp(
                index,
                0, this.m_Screenshots.Count - 1
            );

            Screenshot screenshot = this.m_Screenshots[this.m_ScreenshotIndex];

            UIElementUtil.SetTexture(
                this.m_VisualizationImg,
                screenshot.tex_Screenshot
            );

            this.m_ViewFrameLbl.text =
            $"Frame: {screenshot.FrameCount} ({this.m_ScreenshotIndex + 1}/{this.m_Screenshots.Count})";
        }

        private void ResetFloodSim()
        {
            foreach (Screenshot screenshot in this.m_Screenshots)
            {
                screenshot.tex_Screenshot.Release();
                Object.Destroy(screenshot.tex_Screenshot);
            }

            this.m_Screenshots.Clear();
            this.m_FloodVisualization.ResetSim();
        }

        private void ScreenshotFloodSim()
        {
            RenderTexture tex_composite = this.m_FloodVisualization.FloodBuffer.tex_Composite;

            RenderTexture tex_screenshot = new RenderTexture(
                tex_composite.width, tex_composite.height,
                tex_composite.graphicsFormat,
                tex_composite.depthStencilFormat,
                tex_composite.mipmapCount
            );

            Graphics.CopyTexture(tex_composite, tex_screenshot);

            // store it in a list
            this.m_Screenshots.Add(new Screenshot
            {
                FrameCount = this.m_FloodVisualization.FrameCount,
                tex_Screenshot = tex_screenshot,
            });

            this.SetVisualizationImg(this.m_Screenshots.Count - 1);
        }

        public void SetVisible(bool active)
        {
            this.m_Root.visible = active;

            if (!active)
            {
                this.m_IsPlaying = false;
            }
        }

        private void Start()
        {
            UXManager.Instance.FloodVisualizationUI = this;

            this.m_Root = this.m_Document.rootVisualElement;
            this.m_SimulationImg = this.m_Root.Q<VisualElement>("simulation-img");
            this.m_VisualizationImg = this.m_Root.Q<VisualElement>("visualization-img");

            this.m_SimFrameLbl = this.m_Root.Q<Label>("simframe-lbl");
            this.m_ViewFrameLbl = this.m_Root.Q<Label>("viewframe-lbl");

            // simulation buttons
            this.m_PauseBtn = this.m_Root.Q<Button>("pause-btn");
            this.m_PlayBtn = this.m_Root.Q<Button>("play-btn");
            this.m_ResetBtn = this.m_Root.Q<Button>("reset-btn");
            this.m_ScreenshotBtn = this.m_Root.Q<Button>("screenshot-btn");

            // visualization buttons
            this.m_PrevBtn = this.m_Root.Q<Button>("prev-btn");
            this.m_NextBtn = this.m_Root.Q<Button>("next-btn");

            this.m_Screenshots = new List<Screenshot>(64);
            this.m_ScreenshotIndex = 0;


            // set visibility to false by default
            this.SetVisible(false);

            this.m_PauseBtn.clicked += () =>
            {
                this.m_IsPlaying = false;
            };

            this.m_PlayBtn.clicked += () =>
            {
                this.m_IsPlaying = true;
            };

            this.m_ResetBtn.clicked += () =>
            {
                this.m_IsPlaying = false;
                this.ResetFloodSim();
            };

            this.m_ScreenshotBtn.clicked += () =>
            {
                this.ScreenshotFloodSim();
            };

            this.m_PrevBtn.clicked += () =>
            {
                this.OffsetVisualizationImg(-1);
            };

            this.m_NextBtn.clicked += () =>
            {
                this.OffsetVisualizationImg(1);
            };
        }

        private void Update()
        {
            // update frame count label
            this.m_SimFrameLbl.text = $"Frame: {this.m_FloodVisualization.FrameCount}";

            if (this.m_IsPlaying)
            {
                this.m_FloodVisualization.UpdateSim();
            }
        }
    }

    public struct Screenshot
    {
        public int FrameCount;
        public RenderTexture tex_Screenshot;
    }
}
