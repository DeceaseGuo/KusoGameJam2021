using UnityEngine;
using UnityEngine.Video;

public class Stage : MonoBehaviour
{
    public VideoPlayer mWinVideo = null;
    public Transform[] m_HeadStage;
    public int m_CurrentPoint;

    public SpriteRenderer mSpriteRender = null;
    public AudioSource mAudio = null;
    private System.Action mVideoFinishAction = null;

    public void PlayWinVideo(System.Action iFinishAction)
    {
        if (mWinVideo)
        {
            mWinVideo.Stop();
            mWinVideo.targetTexture.Release();
            Destroy(mWinVideo.gameObject);
        }
        mSpriteRender.gameObject.SetActive(true);

        mWinVideo = new GameObject().AddComponent<VideoPlayer>();
        mWinVideo.audioOutputMode = VideoAudioOutputMode.AudioSource;
        mWinVideo.SetTargetAudioSource(0, mAudio);
        mWinVideo.renderMode = VideoRenderMode.MaterialOverride;
        mWinVideo.targetMaterialRenderer = mSpriteRender;

        string streamingMediaPath = System.IO.Path.Combine(Application.streamingAssetsPath, "winVideo.mp4");
        mWinVideo.url = streamingMediaPath;
        mWinVideo.Prepare();
        mWinVideo.Play();

        mVideoFinishAction = iFinishAction;
        mWinVideo.loopPointReached += WinVideoFinish;
    }

    private void WinVideoFinish(VideoPlayer iPlayer)
    {
        mVideoFinishAction?.Invoke();
    }

    public Transform HeadAttach()
    {
        m_CurrentPoint++;

        if(m_CurrentPoint == 10)
        {
            GameFlow.m_Instance.GameWin();
            PlayWinVideo(null);
        }

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
