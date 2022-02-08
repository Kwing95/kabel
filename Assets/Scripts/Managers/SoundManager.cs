using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;
    public enum Sound { Step, Gun, Explosion, Knife, Glass, Flare };

    private AudioSource source;

    public List<AudioClip> gunBank;
    public List<AudioClip> explosionBank;
    public List<AudioClip> knifeBank;
    public List<AudioClip> stepBank;
    public List<AudioClip> glassBank;
    public List<AudioClip> flareBank;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(Sound sound, int seed=-1, float volume=1)
    {
        switch (sound)
        {
            case Sound.Step:
                PlayRandom(stepBank, -1, volume);
                break;
            case Sound.Gun:
                PlayRandom(gunBank, seed);
                break;
            case Sound.Explosion:
                PlayRandom(explosionBank);
                break;
            case Sound.Knife:
                PlayRandom(knifeBank);
                break;
            case Sound.Glass:
                PlayRandom(glassBank);
                break;
            case Sound.Flare:
                PlayRandom(flareBank);
                break;
        }
    }

    // Plays a random sound from given soundbank
    private void PlayRandom(List<AudioClip> soundBank, int seed=-1, float volume=1)
    {
        source.volume = volume;
        if(seed == -1)
        {
            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(soundBank[Random.Range(0, soundBank.Count)]);
        }
        else
        {
            source.pitch = PRNG.SeededRange(0.9f, 1.1f, seed);
            source.PlayOneShot(soundBank[PRNG.SeededRange(0, soundBank.Count, seed)]);
        }
        
    }

}
