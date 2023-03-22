using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockConfiguration : MonoBehaviour
{
    List<GameObject> configs = new List<GameObject>();
    public bool isDark;
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform config in transform)
        {
            configs.Add(config.gameObject);
            foreach (Transform rock in config)
            {
                rock.gameObject.AddComponent<Rock>();
            }
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

        if (isDark)
        {
            DarkenRocks();
        }
        else
        {
            LightenRocks();
        }
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

    public void DarkenRocks()
    {
        foreach(GameObject config in configs)
        {
            foreach(Rock rock in config.GetComponentsInChildren<Rock>())
            {
                rock.HideRock();
            }
        }
    }

    public void LightenRocks()
    {
        foreach (GameObject config in configs)
        {
            foreach (Rock rock in config.GetComponentsInChildren<Rock>())
            {
                rock.ShowRock();
            }
        }
    }
}
