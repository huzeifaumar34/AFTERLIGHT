using UnityEngine;

namespace Afterlight
{
    public static class MaterialFactory
    {
        public static Material Create(string name, Color color, float metallic = 0f, float smoothness = 0.25f)
        {
            var shader = Shader.Find("Standard");
            var material = new Material(shader) { name = name, color = color };
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);
            return material;
        }

        public static Material Emissive(string name, Color color, float intensity = 1.2f)
        {
            var material = Create(name, color, 0f, 0.5f);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * intensity);
            return material;
        }
    }
}
