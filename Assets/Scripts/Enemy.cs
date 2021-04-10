using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FBuff
{
    public EEnemyBuff m_eType;
    public float m_fValue;
    public float m_fEndTime;
}

public enum EEnemyBuff
{
    eSlow,
    eQuick
}

public enum EEnemyState
{
    eNormal,
    eWait,
    eAttacking,
    eReadyToAttack,
    eDie
}

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float m_fMaxHp = 100.0f;

    public float m_fSpeed = 10.0f;

    public float m_fMoveForce = 1000.0f;

    public float m_fSmoothValue = 1.0f;

    [Range(0,1)]
    public float m_fAttackAngleRange = 0.2f;

    public float m_fStopDistance = 5.0f;

    float m_fHP = 100.0f;

    public GameObject m_oPlayer;

    public float m_fAttackDelay = 5.0f;

    Vector2 m_Velocity = Vector2.zero;

    Vector2 m_CurrentVelocity = Vector2.zero;

    Rigidbody2D m_oMoveRB2D;

    public bool m_bActivate = false;
    List<FBuff> m_BuffList = new List<FBuff>();
    EEnemyState m_eState = EEnemyState.eNormal;
    Animator m_Animator;
    SpriteRenderer m_SpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_fHP = m_fMaxHp;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();
        m_oMoveRB2D = GetComponent<Rigidbody2D>();
        ActivateObject(true);
        AddBuff(EEnemyBuff.eSlow, 1.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        ResetBuff();
        m_Animator.SetFloat("Speed", m_oMoveRB2D.velocity.magnitude);
        if(m_eState == EEnemyState.eAttacking || m_eState == EEnemyState.eDie || m_eState == EEnemyState.eReadyToAttack)
        {
            return;
        }
        Vector2 dir = (transform.position - m_oPlayer.transform.position).normalized;
        Vector2 moveVelocity = Vector2.zero;
        if(Vector2.Distance(transform.position, m_oPlayer.transform.position) > m_fStopDistance)
        {
            moveVelocity = new Vector2(Vector2.Dot(transform.right,dir), Vector2.Dot(transform.up,dir)) * -1;
            CancelInvoke();
        }
        else
        {
            m_eState = EEnemyState.eReadyToAttack;
            Invoke("Attack", m_fAttackDelay);
        }
        
        m_oMoveRB2D.AddForce(Vector2.SmoothDamp(m_Velocity, moveVelocity, ref m_CurrentVelocity, m_fSmoothValue, m_fSpeed) * m_fMoveForce);

        if (m_oMoveRB2D.velocity.x != 0)
        {
            if (m_oMoveRB2D.velocity.x < 0.0f)
            {
                m_SpriteRenderer.flipX = true;
            }
            else
            {
                m_SpriteRenderer.flipX = false;
            }
        }

    }

    void Attack()
    {
        m_eState = EEnemyState.eAttacking;
        m_Animator.SetTrigger("Attack");
    }

    void AttackDetect()
    {
        Vector2 dir = (transform.position - m_oPlayer.transform.position).normalized;
        if(Vector2.Dot(transform.right, dir) > 0)
        {
            if (Vector2.Distance(transform.position, m_oPlayer.transform.position) < m_fStopDistance)
            {

            }
        }
    }

    void AttackEnd()
    {
        m_eState = EEnemyState.eNormal;
    }

    public void TakeDamage(float fDamage)
    {
        m_fHP -= fDamage;
        if (m_fHP <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

    public void AddForceToEnemy(Vector2 oForce)
    {
        m_oMoveRB2D.AddForce(oForce);
    }

    public void ActivateObject(bool bActivate)
    {
        m_bActivate = bActivate;
    }

    public void AddBuff(EEnemyBuff eBuffType, float fValue, float fTime)
    {
        switch (eBuffType)
        {
            case EEnemyBuff.eSlow:
                m_fSpeed -= fValue;
                break;
            case EEnemyBuff.eQuick:
                m_fSpeed += fValue;
                break;
            default:
                break;
        }

        FBuff newBuff;
        newBuff.m_eType = eBuffType;
        newBuff.m_fValue = fValue;
        if(fTime > 0.0f)
        {
            newBuff.m_fEndTime = Time.time + fTime;
        }
        else
        {
            newBuff.m_fEndTime = -1.0f;
        }

        m_BuffList.Add(newBuff);
    }

    public void ResetBuff()
    {
        if(m_BuffList.Count == 0)
        {
            return;
        }
        foreach (FBuff buff in m_BuffList)
        {
            if (buff.m_fEndTime < Time.time && buff.m_fEndTime > 0.0f)
            {
                switch (buff.m_eType)
                {
                    case EEnemyBuff.eSlow:
                        m_fSpeed += buff.m_fValue;
                        break;
                    case EEnemyBuff.eQuick:
                        m_fSpeed -= buff.m_fValue;
                        break;
                    default:
                        break;
                }
                m_BuffList.Remove(buff);
                break;
            }
        }
    }

}
