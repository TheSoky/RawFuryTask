using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    //Serialized fields
    [Header("Prefabs & pooling")]

    [SerializeField]
    private List<GameObject> _objectsToSpawn = new List<GameObject>();

    [SerializeField]
    private int _numberOfObjectsToPool = 10;

    [SerializeField] [Range(0, 31)]
    private int _objectDefaultLayer;

    [Header("Spawn info")]

    [SerializeField]
    private float _spawnIntervalInSeconds = 3.0f;

    [SerializeField]
    private float _spawnAreaWidth = 1.0f;

    //Private fields
    private bool _shouldSpawn = true;

    //Properties
    public bool ShouldSpawn { set { _shouldSpawn = value; } }

    private void Start()
    {
        //Preload objects with pooling

        Dictionary<GameObject, int> objectsToPool = new Dictionary<GameObject, int>();

        foreach (GameObject objectToSpawn in _objectsToSpawn)
        {
            objectsToPool.Add(objectToSpawn, _numberOfObjectsToPool);
        }

        PoolManager.Instance.AddNewPool(objectsToPool);

        //Start spawning
        StartCoroutine(SpawnObjectsRoutine());
    }

    private IEnumerator SpawnObjectsRoutine()
    {
        while(_shouldSpawn)
        {
            yield return new WaitForSeconds(_spawnIntervalInSeconds);

            //determine spawn position
            float spawnX = Random.Range(-_spawnAreaWidth, _spawnAreaWidth);
            Vector3 spawnPosition = new Vector3(transform.position.x + spawnX, transform.position.y, transform.position.z);

            //Get object from pool and "spawn" it
            GameObject objectToSpawn = PoolManager.Instance.GetNextAvailableObject(_objectsToSpawn[Random.Range(0, _objectsToSpawn.Count)]);
            objectToSpawn.transform.position = spawnPosition;
            objectToSpawn.layer = _objectDefaultLayer;
            objectToSpawn.SetActive(true);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 lineStart = new Vector3(transform.position.x - _spawnAreaWidth, transform.position.y, transform.position.z);
        Vector3 lineEnd = new Vector3(transform.position.x + _spawnAreaWidth, transform.position.y, transform.position.z);

        Gizmos.DrawLine(lineStart, lineEnd);
    }

}
