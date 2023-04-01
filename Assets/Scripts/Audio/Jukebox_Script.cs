using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Jukebox_Script : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField]private AudioClip[] BGMClips;
    [SerializeField] TextMeshProUGUI currentlyPlaying;
    void Start()
    {
        //int a=0;
        for (int i = 0; i < this.transform.childCount; ++i)
        {
            if (this.transform.childCount!=BGMClips.Length)
            {
                Debug.Log("please assign same val to arr ln and childcount");
                break;
            }
            JBButton_Script jbButton;
            jbButton = this.transform.GetChild(i).GetComponent<JBButton_Script>();
            /*Debug.Log(jbButton.name);
            Debug.Log(i);
            Debug.Log(BGMClips[i]);*/
            AudioClip temp = BGMClips[i];

            jbButton.button.onClick.AddListener(() => AudioManager_Script.PlayBGM(temp));
            jbButton.button.onClick.AddListener(() =>
            currentlyPlaying.text = $"Now playing: {temp.name:Nothing currently playing}"
            );

            //Debug.Log(i);

            jbButton.labelText.text = $"{BGMClips[i].name}";
        }
        
    }

    void UpdateBGMName()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
