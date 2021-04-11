using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public Transform[] EnemySpawnPoint;
    public CinemachineVirtualCamera m_CameraVC;
    public List<Enemy> m_Enemy;
    public Enemy m_SequenceEnemy;
    public Player m_PlayerRole;
    public Canvas m_TutorialUI;
    public AnimationCurve m_EnemySpeedCurve;
    public float m_fUpdateSpeedTime;

    public float m_fAddEnemyTime;

    public GameObject m_EnemyObject;

    // Start is called before the first frame update
    void Start()
    {
        m_TutorialUI.gameObject.SetActive(false);
        m_Enemy.Add(m_SequenceEnemy);
        m_SequenceEnemy.ActivateObject(false);
        EnablePlayerInput(false);
        Invoke("FirstSequence", 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnablePlayerInput(bool bEnable)
    {
        m_PlayerRole.enabled = bEnable;
    }

    public void FirstSequence()
    {
        m_CameraVC.Follow = m_SequenceEnemy.transform;
        Invoke("EndSequence", 2.0f);
    }

    public void EndSequence()
    {
        m_CameraVC.Follow = m_PlayerRole.transform;
        Invoke("PlayToturial", 1.0f);
    }

    public void PlayToturial()
    {
        m_TutorialUI.gameObject.SetActive(true);
    }

    public void EndToturial()
    {
        GameStart();
    }

    public void GameStart()
    {
        m_TutorialUI.gameObject.SetActive(false);
        EnablePlayerInput(true);
        ActivateAllEnemy(true);
        InvokeRepeating("ChangeEnemySpeed", 1.0f, m_fUpdateSpeedTime);
        InvokeRepeating("AddEnemy", 1.0f, m_fAddEnemyTime);
    }

    public void AddEnemy()
    {
        Transform RandomPoint = EnemySpawnPoint[Random.Range(0, EnemySpawnPoint.Length - 1)];
        Enemy newEnemy = Instantiate(m_EnemyObject, RandomPoint).GetComponent<Enemy>();
        m_Enemy.Add(newEnemy);
        newEnemy.ActivateObject(true);
        newEnemy.m_oPlayer = m_PlayerRole.gameObject;
    }

    public void ChangeEnemySpeed()
    {
        foreach (Enemy item in m_Enemy)
        {
            item.AddBuff(EEnemyBuff.eQuick, m_EnemySpeedCurve.Evaluate(Time.time), -1.0f);
        }
    }

    public void ActivateAllEnemy(bool bEnable)
    {
        foreach (Enemy item in m_Enemy)
        {
            item.ActivateObject(bEnable);
        }
    }
}
