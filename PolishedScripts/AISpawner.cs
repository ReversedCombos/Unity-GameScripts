using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is used in tandom with the EnemyAIController - When the AI is destroyed then it will respawn the AI

public class AISpawner : MonoBehaviour
{
    [SerializeField] private GameObject AIPrefab;
    [SerializeField] private List<GameObject> SpawnPoints;
    private GameObject AIGameObject;

    private void Awake()
    {
        
    }
    private void Update()
    {
        if(AIGameObject == null)
        {
            AIGameObject = Instantiate(AIPrefab, SpawnPoints[Random.Range(0, SpawnPoints.Count)].transform);
            AIGameObject.transform.parent = null;
        }
    }
}
