using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager_Script : MonoBehaviour
{
    static AudioSource [] Source;
    private void Awake()
    {
        Source = GetComponentsInChildren<AudioSource>();

    }
    // Start is called before the first frame update
    
   
    // Update is called once per frame
    public static void PlayBGM(AudioClip sound)
    {
      
        Source[0].Stop();
        Source[0].clip = sound;
        Source[0].Play();
    }
    public static void StopBGM()
    {
        Source[0].Stop();
        
    }

    public static void PlaySFX(AudioClip sound)
    {

        Source[1].Stop();
        Source[1].clip = sound;
        Source[1].Play();
    }
    public static void StopSFX()
    {
        Source[1].Stop();

    }

}
