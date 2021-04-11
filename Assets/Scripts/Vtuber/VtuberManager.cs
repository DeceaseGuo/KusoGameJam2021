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
    private int _sceneVtubersCount;
    private System.Random _random;
    private Coroutine _spawnCoroutine;

    private Dictionary<string, AudioClip> _headAudioDic;
    private Dictionary<Transform, GameObject> _spawnVtuberDic;
    private AudioSource _audioSource;

    private void Awake()
    {
        Instance = this;
        _random = new System.Random();

        _audioSource = GetComponent<AudioSource>();

        _vtuberSpriteList = VtuberSpriteInfos.ToList();
        _headAudioDic = VtuberSpriteInfos.ToDictionary(data => data.Name, data => data.HeadAudio);

        _spawnVtuberDic = new Dictionary<Transform, GameObject>();
        for (int i = 0; i < SpawnPoints.Length; i++)
        {
            var spawnPoint = SpawnPoints[i];
            if (!_spawnVtuberDic.ContainsKey(spawnPoint))
            {
                _spawnVtuberDic.Add(spawnPoint, null);
            }
        }
    }

    public void StartSpawn()
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
            TryGetSpawnPoint(out var testSpawnPoint);

            Debug.Log($"SpawnPoint = {testSpawnPoint}");
        }
    }

    #region Spawn
    private IEnumerator SpawnTimer(float spawnIntervals)
    {
        while (_vtuberSpriteList.Count > 0)
        {
            yield return new WaitForSeconds(spawnIntervals);
            Debug.Log($"Scene = {_sceneVtubersCount}, Sprite = {_vtuberSpriteList.Count}");

            if (_sceneVtubersCount < SceneVtuberCountLimit && TryGetSpawnPoint(out var spawnPoint))
            {
                var newVtuberSprite = GetNewVtuberSprite();
                var spawnPos = GetOffsetSpawnPos(spawnPoint);
                var newVtuber = SpawnVtuber(newVtuberSprite, spawnPos);

                _audioSource.clip = newVtuberSprite.Audio;
                if (_audioSource.isPlaying)
                {
                    _audioSource.Stop();
                }
                _audioSource.Play();

                _spawnVtuberDic[spawnPoint] = newVtuber;
                _sceneVtubersCount++;
            }
        }

        Debug.Log("StopCoroutine");
        StopCoroutine(_spawnCoroutine);
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

    private bool TryGetSpawnPoint(out Transform spawnPoint)
    {
        spawnPoint = null;

        var spawnPoints = _spawnVtuberDic
            .Where(pair => pair.Value == null)
            .Select(pair => pair.Key)
            .ToArray();

        if (spawnPoints.Length == 0)
        {
            return false;
        }

        var index = _random.Next(spawnPoints.Length);
        spawnPoint = spawnPoints[index];

        return true;
    }

    private Vector3 GetOffsetSpawnPos(Transform spawnPoint)
    {
        var spawnPos = spawnPoint.position;
        var xOffset = (float)_random.NextDouble() * SpawnRange;
        var yOffset = (float)_random.NextDouble() * SpawnRange;
        spawnPos.x += xOffset;
        spawnPos.y += yOffset;

        return spawnPos;
    }

    private GameObject SpawnVtuber(VtuberSpriteInfo newVtuberSprite, Vector3 spawnPos)
    {
        var vtuber = Instantiate(VtuberPrefab, VtuberParent);
        vtuber.transform.position = spawnPos;

        var vtuberData = vtuber.GetComponent<VtuberInfo>();
        vtuberData.FullBody.GetComponent<SpriteRenderer>().sprite = newVtuberSprite.FullBody;
        vtuberData.Head.GetComponent<SpriteRenderer>().sprite = newVtuberSprite.Head;
        vtuberData.Body.GetComponent<SpriteRenderer>().sprite = newVtuberSprite.Body;

        vtuberData.Head.name = newVtuberSprite.Name;

        vtuber.SetActive(true);

        return vtuber;
    }
    #endregion Spawn

    #region Touch
    public void TouchPlayer(VtuberInfo vtuberData)
    {
        if (vtuberData.IsDead)
        {
            return;
        }

        SetVtuberActive(vtuberData);

        PlayBoold(vtuberData.Boold, VtuberDeadDelay - 0.05f);

        Destroy(vtuberData.gameObject, VtuberDeadDelay);
        _sceneVtubersCount--;
    }

    private void SetVtuberActive(VtuberInfo vtuberData)
    {
        vtuberData.IsDead = true;
        vtuberData.Body.SetActive(true);
        vtuberData.FullBody.SetActive(false);
    }

    private void PlayBoold(ParticleSystem boold, float duration)
    {
        if (!boold.isPlaying)
        {
            var main = boold.main;
            main.duration = duration > 0 ? duration : 0;
            boold.Play();
        }
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
