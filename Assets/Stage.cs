using UnityEngine;
using UnityEngine.Video;

public class Stage : MonoBehaviour
{
    public VideoPlayer mWinVideo = null;
    public Transform[] m_HeadStage;
    public int m_CurrentPoint;

    private System.Action mVideoFinishAction = null;

    void Update()
    {
        if (gameObject.GetComponentsInChildren<HeadPerform>().Length == 10)
        {
            GameFlow.m_Instance.GameWin();
            PlayWinVideo(null);
        }
    }

    public void PlayWinVideo(System.Action iFinishAction)
    {
        if (!mWinVideo.isPlaying)
        {
            mVideoFinishAction = iFinishAction;
            mWinVideo.gameObject.SetActive(true);
            mWinVideo.Play();
            mWinVideo.loopPointReached += WinVideoFinish;
        }
    }

    private void WinVideoFinish(VideoPlayer iPlayer)
    {
        mVideoFinishAction?.Invoke();
    }

    public Transform HeadAttach()
    {
        m_CurrentPoint = gameObject.GetComponentsInChildren<HeadPerform>().Length;


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
