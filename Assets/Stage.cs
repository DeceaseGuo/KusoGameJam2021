using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public Transform[] m_HeadStage;

    public int m_CurrentPoint;

    public Transform HeadAttach()
    {
        m_CurrentPoint++;
        if(m_CurrentPoint < m_HeadStage.Length)
        {
            return m_HeadStage[m_CurrentPoint];
        }
        else
        {
            return transform;
        }
        
    }
}
