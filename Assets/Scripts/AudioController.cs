using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> clipList;
    [SerializeField]
    private AudioClip finalClip;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private float gapBetweenAudio;

    private bool playingFinalClip;

    private void Start()
    {
        StartCoroutine(PlayClips());
    }

    IEnumerator PlayClips()
    {
        for(int i = 0; clipList.Count > 0; i ++)
        {
            if (i >= clipList.Count)
                i = 0;

            audioSource.clip = clipList[i];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length + gapBetweenAudio);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playingFinalClip)
        {
            playingFinalClip = true;
            StopAllCoroutines();
            audioSource.clip = finalClip;
            audioSource.Play();
            Invoke("ResumeLoopPlayback", finalClip.length + gapBetweenAudio);
        }
    }

    private void ResumeLoopPlayback()
    {
        playingFinalClip = false;
        StartCoroutine(PlayClips());
    }
}
