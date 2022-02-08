using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicAdapter : MonoBehaviour
{

    public AudioSource idle;
    public AudioSource chase;
    private float chaseVolume = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        chaseVolume = Mathf.Clamp(chaseVolume + 0.5f * (Time.deltaTime * (AutoMover.InAlertStatus() ? 1 : -1)), 0, 1);

        idle.volume = 1 - chaseVolume;
        chase.volume = chaseVolume;
    }

    public void ChangeMusic(AudioClip _idle, AudioClip _chase)
    {
        idle.Stop();
        chase.Stop();
        idle.clip = _idle;
        chase.clip = _chase;
        idle.Play();
        chase.Play();
    }

}
