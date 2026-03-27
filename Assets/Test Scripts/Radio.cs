using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    public AudioSource radioAudio;
    public AudioClip[] radioSongs;
    private AudioClip nextRadioSong;
    public int counter = 0;
    public bool isPaused;
    public bool wasPaused;

    //why are some of my variables public and other protected and others private?

    private void Start()
    {
        radioAudio.clip = radioSongs[0];
        radioAudio.Play();
        nextRadioSong = radioSongs[counter];
    }

    private void Update()
    {
        if(!isPaused)
        {
            radioAudio.UnPause();

            if (radioAudio.isPlaying && radioAudio.clip == nextRadioSong)
            {
                if (counter == radioSongs.Length - 1)
                {
                    counter = 0;
                    nextRadioSong = radioSongs[counter];
                }
                else
                {
                    counter++;
                    nextRadioSong = radioSongs[counter];
                }
            }
            else if (!radioAudio.isPlaying && radioAudio.clip != nextRadioSong)
            {
                radioAudio.clip = nextRadioSong;
                radioAudio.Play();
            }
        }
        else
        {
            radioAudio.Pause();
        }
    }
}
