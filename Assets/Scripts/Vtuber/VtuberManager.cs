using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VtuberManager : MonoBehaviour
{
    public static VtuberManager Instance;

    public float VtuberDeadDelay;
    public float SpawnIntervals;
    public float SpawnRange;
    public int SceneVtuberCountLimit;

    public Transform VtuberParent;

    public GameObject VtuberPrefab;
    public VtuberSpriteInfo[] VtuberSpriteInfos;

    public Transform[] SpawnPoints;
    
    private List<VtuberSpriteInfo> _vtuberSpriteList;
    private List<GameObject> _sceneVtubers;
    private System.Random _random;
    private Coroutine _spawnCoroutine;

    private Dictionary<string, AudioClip> _headAudioDic;

    private void Awake()
    {
        Instance = this;

        _sceneVtubers = new List<GameObject>();
        _random = new System.Random();

        _vtuberSpriteList = VtuberSpriteInfos.ToList();
        _headAudioDic = VtuberSpriteInfos.ToDictionary(data => data.Name, data => data.HeadAudio);
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

        vtuberData.Head.name = newVtuberSprite.Name;

        vtuber.SetActive(true);
    }
    #endregion Spawn

    #region Touch
    public void TouchPlayer(VtuberInfo vtuberData)
    {
        vtuberData.Body.SetActive(true);
        vtuberData.FullBody.SetActive(false);

        var boold = vtuberData.Boold;
        var duration = VtuberDeadDelay - 0.3f;

        if (!boold.isPlaying)
        {
            var main = boold.main;
            main.duration = duration > 0 ? duration : 0;
            boold.Play();
        }

        Destroy(vtuberData.Body, VtuberDeadDelay);
    }

    public AudioClip GetHeadAudio(string vtuberName)
    {
        if (!_headAudioDic.TryGetValue(vtuberName, out var audioClip))
        {
            audioClip = null;
            Debug.LogError($"HeadAudio Is Null @Vtuber = {vtuberName}");
        }

        return audioClip;
    }
    #endregion Touch
}
