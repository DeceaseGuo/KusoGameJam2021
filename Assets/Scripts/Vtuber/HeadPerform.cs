using System;
using UnityEngine;
using DG.Tweening;

public class HeadPerform : MonoBehaviour
{
    public float mJumpHeigght = 0.5f;
    public float mJumpPower = 3;
    public float mjumpTime = 2;
    
    public void ExeCute(Action iFinishAction)
    {
        transform.gameObject.SetActive(true);
        Vector3 endPos = transform.position + (Vector3.up * mJumpHeigght);
        transform.DOJump(endPos, mJumpPower, 1, mjumpTime, false).OnComplete(delegate { iFinishAction?.Invoke(); });
    }
}