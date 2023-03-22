using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetController : MonoBehaviour
{
    RockConfiguration rockConfiguration;
    // Start is called before the first frame update
    private void Awake()
    {
        rockConfiguration = GetComponentInChildren<RockConfiguration>();
    }
    void Start()
    {
        
    }

    public void OnEpisodeBegin()
    {
        rockConfiguration.OnEpisodeBegin();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
