using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public enum Sound { Step, Gun };

    public AudioSource source;
    public List<AudioClip> gunBank;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(Sound sound)
    {
        switch (sound)
        {
            case Sound.Step:
                break;
            case Sound.Gun:
                PlayRandom(gunBank);
                break;
        }
    }

    // Plays a random sound from given soundbank
    private void PlayRandom(List<AudioClip> soundBank)
    {
        source.PlayOneShot(soundBank[Random.Range(0, soundBank.Count)]);
    }

}
