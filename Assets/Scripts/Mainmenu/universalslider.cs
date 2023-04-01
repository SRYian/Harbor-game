using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class universalslider : MonoBehaviour
{
    public AudioMixer mixer;
    [SerializeField] private GameObject tosetvol;
    public static float globalvol=100;
    private Slider myslider;
    private float currentvol;
    private void Start()
    {
        //globalvol = currentvol;
      myslider = GetComponent<Slider>();
        myslider.value = globalvol;
    }

    public void SetVolume(float sliderval)
    {
        myslider.value = globalvol;
        globalvol = sliderval;
        float mixerval=
        Mathf.Log10(sliderval) * 20 ;

        if (mixerval==float.NegativeInfinity||mixerval<-60f)
        {
            mixer.SetFloat(tosetvol.name, -50f);
            return;

        }
        mixer.SetFloat(tosetvol.name, mixerval);

        //Debug.Log(Mathf.Log10(sliderval) * 20);
    }
    
}
