using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
public enum EnemyState
{
    IDLE, 
    ALERT,
    PATROL,
    FURY,
    FOLLOW,
    DEAD
}

public enum GameState
{
    GAMEPLAY,
    GAMEOVER
}
public class GameManager : MonoBehaviour
{
    public GameState gameState = GameState.GAMEPLAY;

    [Header("Slime IA")]
    public Transform[] smileWaypoints;    
    public  float slimeIdleWaitTime = 5f;   
    public float slimeDistanceToAttack = 2.9f;
    public float slimeAlertWaitTime = 1f;
    

    public float slimeAttackDelay = 1f;
    public float slimeLookAtSpeed = 1f;
    public Player player;

    [Header("Rain manager")]
    public PostProcessVolume rainPostProcessingVolume;
    public ParticleSystem rainParticle;
    public int rainRateOverTime;
    public int rainIncrement;
    public float rainIncrementDelay;

    [Header("info Player")]
    public int gems = 0;

    [Header("Interface")]
    public Text gemText;

    [Header("Drop Igem")]
    public GameObject gemPrefab;
    [Range(0, 100)]
    public int dropPercent = 25;

    private ParticleSystem.EmissionModule rainModule;
    void Start()
    {
        player = FindObjectOfType<Player>();
        rainModule = rainParticle.emission;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnOffRain(bool isRain)
    {
        StopCoroutine("RainManager");
        StopCoroutine("RainPostProcessing");
        StartCoroutine(RainManager(isRain));
        StartCoroutine(RainPostProcessing(isRain));
    }

    public void ChangeGameState(GameState newGmeState)
    {
        gameState = newGmeState;
    }
    IEnumerator RainManager(bool isRain)
    {
        switch(isRain)
        {
            case true:
                for(float r =rainModule.rateOverTime.constant; r <rainRateOverTime ; r+= rainIncrement)
                {
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = rainRateOverTime;
                break;
            case false:
                for (float r = rainModule.rateOverTime.constant; r > 0 ; r -= rainIncrement)
                {
                    rainModule.rateOverTime = r;
                    yield return new WaitForSeconds(rainIncrementDelay);
                }
                rainModule.rateOverTime = 0;
                break;
        }
    }

    IEnumerator RainPostProcessing(bool isRain)
    {
        switch (isRain)
        {
            case true:
                for (float w = rainPostProcessingVolume.weight;  w < 1; w += Time.deltaTime)
                {
                    rainPostProcessingVolume.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                rainPostProcessingVolume.weight = 1;
                break;
            case false:
                for (float w = rainPostProcessingVolume.weight; w > 0; w -= Time.deltaTime)
                {
                    rainPostProcessingVolume.weight = w;
                    yield return new WaitForEndOfFrame();
                }
                rainPostProcessingVolume.weight = 0;
                break;
        }
    }

    public void AddGems(int gemsToAdd)
    {
        gems += gemsToAdd;
        gemText.text = gems.ToString();
    }

    public void SpawGem(Vector3 location)
    {
        int spawChange = UnityEngine.Random.Range(0, 100);
        if(spawChange <= dropPercent)
        {
            GameObject newGem = Instantiate(gemPrefab);
            newGem.transform.position = location;
        }
    }
}
