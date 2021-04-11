using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public Transform[] m_HeadStage;

    public int m_CurrentPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
