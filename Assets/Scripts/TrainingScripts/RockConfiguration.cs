using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockConfiguration : MonoBehaviour
{
    List<GameObject> configs = new List<GameObject>();
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform config in transform)
        {
            configs.Add(config.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEpisodeBegin()
    {
        DeactivateRocks();
        ActivateRandomConfig();
    }
    public void DeactivateRocks()
    {
        foreach(var config in configs)
        {
            config.SetActive(false);
        }
    }

    public void ActivateRandomConfig()
    {
        int configIndex = UnityEngine.Random.Range(0, configs.Count);
        configs[configIndex].SetActive(true);
    }
}
