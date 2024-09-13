using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    private static List<AudioData> s_oneShotAudioSourceClipData = new List<AudioData>();

    public static void PlayOneShotAudio(AudioData audioData, float volumeMultiplier = 1)
    {
        if (audioData.audioSource == null)
            audioData.audioSource = UIManager.src;

        if (audioData.audioSource == null || audioData.audioClipData.audioClip == null)
        {
            Debug.LogError("Audio not found!");
            return;
        }

        if (!s_oneShotAudioSourceClipData.Contains(audioData))
        {
            s_oneShotAudioSourceClipData.Add(audioData);
        }

        // prevent an audio clip from playing if it isn't supposed to:
        if (!IsOneShotAudioReady(audioData))
        {
            return;
        }

        // Log the time for tracking:
        audioData.audioClipData.lastStartTime = Time.time;
        audioData.audioSource.pitch = Random.Range(audioData.audioClipData.pitch - audioData.audioClipData.pitchVariance, audioData.audioClipData.pitch + audioData.audioClipData.pitchVariance);
        audioData.audioSource.PlayOneShot(audioData.audioClipData.audioClip, audioData.audioClipData.volume * volumeMultiplier);
    }

    public static bool IsOneShotAudioReady(AudioData audioData)
    {
        // No key for audioSource so it must be not playing:
        if (!s_oneShotAudioSourceClipData.Contains(audioData))
        {
            s_oneShotAudioSourceClipData.Add(audioData);
        }

        return Time.time - audioData.audioClipData.lastStartTime >= audioData.audioClipData.audioClip.length * audioData.audioClipData.waitUntil;
    }
}

[System.Serializable]
public class AudioData
{
    #region Inspector Assigned Field(s):
    public AudioSource audioSource;
    [SerializeField] private AudioClipData m_audioClipData;
    #endregion

    #region Properties:
    public AudioClipData audioClipData => m_audioClipData;
    #endregion
}

[System.Serializable]
public class AudioClipData
{
    #region Inspector Assigned Field(s):
    [SerializeField] private AudioClip m_audioClip;
    [SerializeField, Range(0, 1f)] private float m_volume;
    [SerializeField, Range(0, 10f)] private float m_waitUntil;
    [SerializeField, Range(-3, 3f)] private float m_pitch = 1;
    [SerializeField, Range(0, 3f)] private float m_pitchVariation = 0;
    [HideInInspector] public float lastStartTime;
    #endregion

    #region Properties:
    public AudioClip audioClip => m_audioClip;
    public float volume => m_volume;
    public float waitUntil => m_waitUntil;
    public float pitch => m_pitch;
    public float pitchVariance => m_pitchVariation;
    #endregion
}