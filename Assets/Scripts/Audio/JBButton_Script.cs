using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JBButton_Script : MonoBehaviour
{
    // Start is called before the first frame update
    public Button button;
    public TextMeshProUGUI labelText;
    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        labelText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
