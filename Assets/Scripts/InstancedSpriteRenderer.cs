using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class InstancedSpriteRenderer : MonoBehaviour
{
    public Mesh quadMesh;

    public Vector4 pivot;
    public Vector4 newUV;

    private static Dictionary<Texture2D, int> textureIndexes = new Dictionary<Texture2D, int>();
    private static Texture2DArray spriteTextures;
    private static int spriteTextureCount = 0;

    private MaterialPropertyBlock props;
    int textureID = 0;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Sprite instancing implementation
        if (spriteRenderer.sprite.texture.width != 512) // Texture2DArray needs same size and format
            return;

        Texture2D tex = spriteRenderer.sprite.texture;
        if (spriteTextures == null)
        {
            spriteTextures = new Texture2DArray(tex.width, tex.height, 128, tex.format, false);
            spriteTextureCount = 0;
        }

        if (props == null)
        {
            props = new MaterialPropertyBlock();
        }

        if (!textureIndexes.ContainsKey(tex))
        {
            Graphics.CopyTexture(tex, 0, 0, spriteTextures, spriteTextureCount, 0);
            textureID = spriteTextureCount;
            textureIndexes[tex] = textureID;
            spriteTextureCount++;
        }
        else
        {
            textureID = textureIndexes[tex];
        }

        spriteRenderer.enabled = false;

        GameObject temp = new GameObject(gameObject.name + "_mesh");
        temp.layer = gameObject.layer;
        temp.transform.SetParent(transform);
        temp.transform.localPosition = Vector3.zero;
        temp.transform.localRotation = Quaternion.identity;
        temp.transform.localScale = Vector3.one;

        MeshFilter meshFilter = temp.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = temp.AddComponent<MeshRenderer>();
        meshRenderer.enabled = true;
        Material mat = Resources.Load("InstancedSprite", typeof(Material)) as Material;
        // spriteRenderer.sharedMaterial = mat;
        meshRenderer.sharedMaterial = mat;
        // meshRenderer.sharedMaterial = spriteRenderer.sharedMaterial;
        // meshRenderer.material.SetTexture("_MainTex", spriteRenderer.sprite.texture);

        meshRenderer.sharedMaterial.SetTexture("_Textures", spriteTextures);

        meshFilter.sharedMesh = quadMesh;

        Sprite sprite = spriteRenderer.sprite;

        // Calculate vertices translate and scale value
        pivot.x = sprite.rect.width / sprite.pixelsPerUnit;
        pivot.y = sprite.rect.height / sprite.pixelsPerUnit;
        pivot.z = ((sprite.rect.width / 2) - sprite.pivot.x) / sprite.pixelsPerUnit;
        pivot.w = ((sprite.rect.height / 2) - sprite.pivot.y) / sprite.pixelsPerUnit;

        // Calculate uv translate and scale value
        newUV.x = sprite.uv[1].x - sprite.uv[0].x;
        newUV.y = sprite.uv[0].y - sprite.uv[2].y;
        newUV.z = sprite.uv[2].x;
        newUV.w = sprite.uv[2].y;

        var positions = new Vector3[1];
        positions[0] = transform.position;
        var lightProbes = new UnityEngine.Rendering.SphericalHarmonicsL2[1];
        var occlusionProbes = new Vector4[1];
        LightProbes.CalculateInterpolatedLightAndOcclusionProbes(positions, lightProbes, occlusionProbes);
        // lightProbes[0][0, 0] = positions[0].x;

        // Set MaterialPropertyBlock
        // meshRenderer.GetPropertyBlock(props);
        props.CopySHCoefficientArraysFrom(lightProbes);
        props.CopyProbeOcclusionArrayFrom(occlusionProbes);
        meshRenderer.lightProbeUsage = LightProbeUsage.CustomProvided;
        meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        // meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
        props.SetFloat("_TextureIndex", textureID);
        props.SetVector("_Pivot", pivot);
        props.SetVector("_NewUV", newUV);
        meshRenderer.SetPropertyBlock(props);
    }
}
