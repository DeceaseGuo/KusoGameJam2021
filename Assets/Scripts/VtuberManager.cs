using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VtuberManager : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public GameObject[] VtubersPrefab;
    public Transform VtuberParent;
    public float SpawnIntervals;
    public float SpawnRange;
    public int SceneVtuberCountLimit;

    private List<GameObject> _vtuberList;

    private List<GameObject> _sceneVtubers;

    private System.Random _random;

    private Coroutine _spawnCoroutine;

    private void Awake()
    {
        _vtuberList = VtubersPrefab.ToList();
        _sceneVtubers = new List<GameObject>();
        _random = new System.Random();
    }

    private void Start()
    {
        _spawnCoroutine = StartCoroutine(SpawnTimer(SpawnIntervals));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (SpawnPoints.Length == 0)
            {
                SpawnPoints = new Transform[] { transform };
            }
            var testSpawnPoint = GetSpawnPoint();

            Debug.Log($"SpawnPoint = {testSpawnPoint}");
        }
    }

    private IEnumerator SpawnTimer(float spawnIntervals)
    {
        yield return new WaitForSeconds(spawnIntervals);

        if (_sceneVtubers.Count < SceneVtuberCountLimit)
        {
            var newVtuber = GetNewVtuber();
            var spawnPoint = GetSpawnPoint();
            Spawn(newVtuber, spawnPoint);
        }
    }

    private GameObject GetNewVtuber()
    {
        var index = _random.Next(_vtuberList.Count);

        var vtuber = _vtuberList[index];
        _sceneVtubers.Add(vtuber);

        var lastIndex = _vtuberList.Count - 1;
        _vtuberList[index] = _vtuberList[lastIndex];
        _vtuberList.RemoveAt(lastIndex);

        if (_vtuberList.Count <= 0)
        {
            StopCoroutine(_spawnCoroutine);
        }

        return vtuber;
    }

    private Vector3 GetSpawnPoint()
    {
        var index = _random.Next(SpawnPoints.Length);

        var spawnPoint = SpawnPoints[index].position;

        var xOffset = (float)_random.NextDouble() * SpawnRange;
        var zOffset = (float)_random.NextDouble() * SpawnRange;
        spawnPoint.x += xOffset;
        spawnPoint.z += zOffset;

        return spawnPoint;
    }

    private void Spawn(GameObject newVtuber, Vector3 spawnPos)
    {
        var obj = Instantiate(newVtuber, VtuberParent);
        obj.transform.position = spawnPos;
    }
}
