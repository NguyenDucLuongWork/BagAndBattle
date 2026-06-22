using JetBrains.Annotations;
using LgTyLib.Modules.SpriteLocalCoords;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemVisual : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;

    private Coroutine scaleRoutine;
    private Vector3 baseScale = Vector3.one;

    private Image uiImage;
    private Material materialInstance;
    private SpriteLocalCoords spriteLocalCoords;

    private static readonly int SpriteUVsProperty = Shader.PropertyToID("_SpriteUVs");
    private static readonly int ProgressProperty = Shader.PropertyToID("_Progress");

    public void Init(ItemData itemData)
    {
        this.itemData = itemData;
        uiImage = GetComponent<Image>();
        spriteLocalCoords = GetComponent<SpriteLocalCoords>();

        materialInstance = uiImage.materialForRendering;

        UpdateVisual();
    }


    [ContextMenu("UpdateVisual")]
    public void UpdateVisual()
    {
        uiImage.sprite = itemData.sprite;
        spriteLocalCoords.RecalculateSpriteLocalUV();
    }

    [ContextMenu("UsingEffect")]
    public void UsingEffect()
    {
        ScaleUpEffect();
    }

    public void ScaleUpEffect(float newScale = 1.2f, float time = 1f)
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }

        scaleRoutine = StartCoroutine(ScaleRoutine(newScale, time));
    }

    private IEnumerator ScaleRoutine(float newScale, float time)
    {
        float halfTime = time * 0.5f;
        Vector3 targetScale = baseScale * newScale;

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            float t = halfTime > 0f ? elapsed / halfTime : 1f;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        transform.localScale = targetScale;

        elapsed = 0f;
        startScale = transform.localScale;
        while (elapsed < halfTime)
        {
            elapsed += Time.deltaTime;
            float t = halfTime > 0f ? elapsed / halfTime : 1f;
            transform.localScale = Vector3.Lerp(startScale, baseScale, t);
            yield return null;
        }
        transform.localScale = baseScale;

        scaleRoutine = null;
    }

    public void UpdateShaderUVs()
    {
        if (uiImage == null || uiImage.sprite == null || materialInstance == null) return;

        Vector4 uvRect = UnityEngine.Sprites.DataUtility.GetOuterUV(uiImage.sprite);

        float width = uvRect.z - uvRect.x;
        float height = uvRect.w - uvRect.y;

        Vector4 shaderVector = new Vector4(width, height, uvRect.x, uvRect.y);

        uiImage.material.SetVector(SpriteUVsProperty, shaderVector);
    }

}