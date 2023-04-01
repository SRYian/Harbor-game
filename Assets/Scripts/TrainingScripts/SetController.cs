using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetController : MonoBehaviour
{
    RockConfiguration rockConfiguration;
    Ship ship;
    Monster monster;
    // Start is called before the first frame update
    private void Awake()
    {
        rockConfiguration = GetComponentInChildren<RockConfiguration>();
        ship = GetComponentInChildren<Ship>();
        monster = GetComponentInChildren<Monster>();
    }
    void Start()
    {
        
    }

    public void OnEpisodeBegin()
    {
        rockConfiguration.OnEpisodeBegin();
        ship.randomizePosition();
        monster.randomizePosition();
    }

    public void EndShip()
    {
        ship.EndEpisode();
    }

    public void EndMonster(float reward)
    {
        //print(monster.GetCumulativeReward());
        monster.AddReward(reward);
        monster.EndEpisode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
