using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject loadingscreen;
    [SerializeField] private Slider loadingbar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowWinScreen()
    {

    }
    public void ShowLoseScreen()
    {

    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public void LoadLevel(int id)
    {
        StartCoroutine(loadasync(id));

    }

    public void ReturnToMenu()
    {
        LoadLevel(0);
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
