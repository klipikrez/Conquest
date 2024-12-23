using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioClip[] send;
    public AudioClip[] battle;
    public AudioClip[] tower;
    public GameObject battleSoundPrefab;
    public AudioMixerGroup DD;
    // Start is called before the first frame update
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayAudioClip(6);
        }
    }
    public void PlayAudioClip(int index)
    {

        StartCoroutine(Play(audioClips[index]));

    }
    public void PlaySendSound()
    {
        StartCoroutine(Play(send[Random.Range(0, send.Length - 1)]));
    }
    public void PlayBattleSound(Vector3 position)
    {
        vfxManager.Instance.Play(position, 0);
        StartCoroutine(PlayDDD(battle[Random.Range(0, battle.Length - 1)], position));

    }

    public void PlayTowerSound(Vector3 position)
    {
        StartCoroutine(PlayDDD(tower[Random.Range(0, tower.Length - 1)], position));
    }

    public void PlayDeniedSound()
    {
        PlayAudioClip(Random.Range(8, 10));
    }

    IEnumerator PlayDDD(AudioClip audio, Vector3 position)
    {
        GameObject gameobj = Instantiate(battleSoundPrefab, position, Quaternion.identity, gameObject.transform);
        AudioSource audioSource = gameobj.GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length); //cekaj
        audioSource.Stop();
        Destroy(gameobj);
    }
    IEnumerator Play(AudioClip audio)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = DD;
        audioSource.clip = audio;
        audioSource.Play();
        yield return new WaitForSeconds(audio.length); //cekaj
        audioSource.Stop();
        Destroy(audioSource);
    }

}
