using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public Transform EnemySpawnPoint;
    public CinemachineVirtualCamera m_CameraVC;
    public List<Enemy> m_Enemy;
    public Enemy m_SequenceEnemy;
    public Player m_PlayerRole;
    public Canvas m_TutorialUI;

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
        m_TutorialUI.gameObject.SetActive(false);
        EnablePlayerInput(true);
        ActivateAllEnemy(true);

    }

    public void ActivateAllEnemy(bool bEnable)
    {
        foreach (Enemy item in m_Enemy)
        {
            item.ActivateObject(bEnable);
        }
    }
}
