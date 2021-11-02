using UnityEngine;

namespace AnttiStarterKit.Displaces
{
    public class DisplaceCamera : MonoBehaviour
    {
        public Material worldDisplaceMaterial;
        
        private Camera cam;
        private RenderTexture texture;
        private static readonly int DisplaceTex = Shader.PropertyToID("_DisplaceTex");
        private static readonly int Flip = Shader.PropertyToID("_Flip");

        private void Awake()
        {
            cam = GetComponent<Camera>();
            texture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
            cam.targetTexture = texture;
            worldDisplaceMaterial.SetTexture(DisplaceTex, texture);
            worldDisplaceMaterial.SetFloat(Flip, Application.platform == RuntimePlatform.WebGLPlayer ? 0 : 1);
        }
    }
}
