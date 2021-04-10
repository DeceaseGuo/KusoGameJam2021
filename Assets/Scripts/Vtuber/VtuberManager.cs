using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VtuberManager : MonoBehaviour
{
    public static VtuberManager Instance;

    public float SpawnIntervals;
    public float SpawnRange;
    public int SceneVtuberCountLimit;

    public Transform VtuberParent;
    public Transform HeadParent;

    public GameObject VtuberPrefab;
    public VtuberSpriteInfo[] VtuberSpriteInfos;

    public Transform[] SpawnPoints;
    
    private List<VtuberSpriteInfo> _vtuberSpriteList;
    private List<GameObject> _sceneVtubers;
    private System.Random _random;
    private Coroutine _spawnCoroutine;

    private void Awake()
    {
        Instance = this;
        _vtuberSpriteList = VtuberSpriteInfos.ToList();
        _sceneVtubers = new List<GameObject>();
        _random = new System.Random();

        if (HeadParent == null)
        {
            HeadParent = transform;
        }
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

    #region Spawn
    private IEnumerator SpawnTimer(float spawnIntervals)
    {
        yield return new WaitForSeconds(spawnIntervals);

        if (_sceneVtubers.Count < SceneVtuberCountLimit && _vtuberSpriteList.Count > 0)
        {
            var newVtuberSprite = GetNewVtuberSprite();
            var spawnPoint = GetSpawnPoint();
            Spawn(newVtuberSprite, spawnPoint);
        }
    }

    private VtuberSpriteInfo GetNewVtuberSprite()
    {
        var index = _random.Next(_vtuberSpriteList.Count);

        var vtuberSprite = _vtuberSpriteList[index];

        var lastIndex = _vtuberSpriteList.Count - 1;
        _vtuberSpriteList[index] = _vtuberSpriteList[lastIndex];
        _vtuberSpriteList.RemoveAt(lastIndex);

        return vtuberSprite;
    }

    private Vector3 GetSpawnPoint()
    {
        var index = _random.Next(SpawnPoints.Length);

        var spawnPoint = SpawnPoints[index].position;

        var xOffset = (float)_random.NextDouble() * SpawnRange;
        var yOffset = (float)_random.NextDouble() * SpawnRange;
        spawnPoint.x += xOffset;
        spawnPoint.y += yOffset;

        return spawnPoint;
    }

    private void Spawn(VtuberSpriteInfo newVtuberSprite, Vector3 spawnPos)
    {
        var vtuber = Instantiate(VtuberPrefab, VtuberParent);
        vtuber.transform.position = spawnPos;

        var vtuberData = vtuber.GetComponent<VtuberInfo>();
        vtuberData.FullBody.GetComponent<SpriteRenderer>().sprite = newVtuberSprite.FullBody;
        vtuberData.Head.GetComponent<SpriteRenderer>().sprite = newVtuberSprite.Head;
        vtuberData.Body.GetComponent<SpriteRenderer>().sprite = newVtuberSprite.Body;
        vtuberData.GetComponent<AudioSource>().clip = newVtuberSprite.Audio;

        vtuber.SetActive(true);
    }
    #endregion Spawn

    #region Touch
    public void TouchPlayer(VtuberInfo vtuberData)
    {
        var vtuberHeadTrans = vtuberData.Head.transform;
        vtuberHeadTrans.SetParent(VtuberManager.Instance.HeadParent);
        vtuberHeadTrans.transform.localPosition = Vector3.zero;

        vtuberData.Head.SetActive(true);
        vtuberData.Body.SetActive(true);
        vtuberData.FullBody.SetActive(false);

        if(!vtuberData.Boold.isPlaying)
        {
            vtuberData.Boold.Play();
        }
    }
    #endregion Touch
}
