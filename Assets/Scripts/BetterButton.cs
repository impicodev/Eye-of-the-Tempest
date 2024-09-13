using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BetterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public AudioData clickSound, hoverSound;
    public Transform otherObj;

    private Button button;
    private Vector3 defaultScale;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { AudioManager.PlayOneShotAudio(clickSound); });
        defaultScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.PlayOneShotAudio(hoverSound);
        transform.DOScale(defaultScale * hoverScale, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true); ;
        if (otherObj)
            otherObj.DOScale(defaultScale * hoverScale, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true); ;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(defaultScale, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true); ;
        if (otherObj)
            otherObj.DOScale(defaultScale, 0.2f).SetEase(Ease.OutCubic).SetUpdate(true);
    }
}
