using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> clipList;
    [SerializeField]
    AudioClip finalClip;
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    float gapBetweenAudio;

    bool playingFinalClip;

    private void Start()
    {
        StartCoroutine(PlayClips());
    }
    IEnumerator PlayClips()
    {
        int i = 0;
        while (clipList.Count > 0)
        {
            if (i >= clipList.Count)
                i = 0;

            audioSource.clip = clipList[i];
            audioSource.Play();
            i++;
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
            Invoke("PlayAgain", finalClip.length + gapBetweenAudio);
        }
    }
    void PlayAgain()
    {
        playingFinalClip = false;
        StartCoroutine(PlayClips());
    }
}
