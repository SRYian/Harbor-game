using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Mainmenu : MonoBehaviour
{
    public GameObject loadingscreen;
    public Slider loadingbar;
   
    public void QuitGame()
    {
        Debug.Log("You have quit the game\n");
        Application.Quit();
    }
 
    

}
