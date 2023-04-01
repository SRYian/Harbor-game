using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
public class LevelSelect : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]private GameObject loadingscreen;
    [SerializeField]private Slider loadingbar;
    [SerializeField] private TextMeshProUGUI[] levelscores;
    /*private PlayerData playerData;

    private void Start()
    {
        //if no save is found
        if (SaveSystem.LoadPlayer() == null)
        {
            Debug.Log("yodayo");
            SaveSystem.SavePlayer(1, 0f);
        }
        else
        {
            playerData = SaveSystem.LoadPlayer();
            for (int i = 0; i < levelscores.Length; i++)
            {
                levelscores[i].text = $"{playerData.Scores[i]:00}";
            }
        }
        
    }
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(1, 0f);

    }
    
    public void ResetGame()
    {
        SaveSystem.SavePlayer(1, 0);
    }*/
    public void LoadLevel(int id)
    {
        StartCoroutine(loadasync(id));

    }
    IEnumerator loadasync(int sceneid)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneid);
        loadingscreen.SetActive(true);
        while (!loading.isDone)
        {
            float loadvalue = Mathf.Clamp01(loading.progress / 0.9f);
            loadingbar.value = loadvalue;
            //Debug.Log("Sudah load " + loadvalue);
            yield return null;
        }
    }
}
