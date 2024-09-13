using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static AudioSource src;
    public Image musicToggleImg;
    //public Transform leftBox, rightBox;

    void Awake()
    {
        //leftBox.localScale = Vector3.one;
        //leftBox.DOScaleX(0, 1f).SetEase(Ease.OutSine).SetUpdate(true);
        src = GetComponent<AudioSource>();
    }

    void Start()
    {
        musicToggleImg.color = SoundTrack.track.src.isPlaying ? Color.white : Color.gray;
    }

    public void loadScene(string scene)
    {
        //leftBox.DOScaleX(1, 0.6f).SetEase(Ease.OutSine).SetUpdate(true).onComplete = () =>
        //{
            SceneManager.LoadScene(scene);
        //};
    }

    public void toggleMusic()
    {
        bool isPlaying = !SoundTrack.track.src.isPlaying;
        if (isPlaying)
            SoundTrack.track.src.UnPause();
        else
            SoundTrack.track.src.Pause();
        musicToggleImg.color = isPlaying ? Color.white : Color.gray;
    }
}
