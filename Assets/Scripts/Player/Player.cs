using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private enum State
    {
        IDLE,
        RUN,
        ATTACK,
        USEITEM,
        DIE
    }
    private State mCurState = State.IDLE;

    public AudioSource mAudio = null;
    public AudioSource mHeadAudio = null;
    public float mMoveSpeed = 0;
    [Header("使用道具僵直")]
    public float mUseItemStiff = 0;
    public GameObject mOilSpout = null;
    public GameObject mOilTrap = null;
    [Header("位移")]
    public float mDashStiff = 0;
    public float mDashSpeed = 0;
    [Header("頭")]
    public Transform mHeadPos = null;

    private bool mIsRight = false;
    private string mHaedName = null;
    private Transform mCurHeadTransform = null;
    private float mCurDelayStiff = 0;
    private byte mItemCount = 0;
    private Animator mAnim = null;
    private Vector2 mMoveDirection = Vector2.zero;
    private Vector2 mAtkDirection = Vector2.zero;
    private Rigidbody2D mRig = null;

    public AudioClip mAtkAudio = null;

    private void Awake()
    {
        mAnim = GetComponent<Animator>();
        mRig = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        StateAction();
    }

    #region Step
    private void StateAction()
    {
        switch (mCurState)
        {
            case State.IDLE:
            case State.RUN:
                StepMove();
                StepAttack();
                StepUseItem();
                break;
            case State.ATTACK:
                mRig.velocity = mAtkDirection * mDashSpeed;
                break;
            case State.USEITEM:
                break;
            case State.DIE:
                break;
            default:
                break;
        }
    }

    private void ChangeStep(State iState)
    {
        mCurState = iState;
    }

    private void StepMove()
    {
        if (mCurDelayStiff <= 0)
        {
            mMoveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            mRig.velocity = new Vector2(mMoveDirection.x * mMoveSpeed, mMoveDirection.y * mMoveSpeed);
            if (mMoveDirection.x != 0 || mMoveDirection.y != 0)
            {
                if ((!mIsRight && mMoveDirection.x > 0) || (mIsRight && mMoveDirection.x < 0))
                {
                    Flip();
                }
                mAnim.SetFloat(State.RUN.ToString(), 1);
            }
            else
                mAnim.SetFloat(State.RUN.ToString(), -1);
        }
        else
        {
            mCurDelayStiff -= Time.deltaTime;
        }
    }

    private void StepAttack()
    {
        if (mCurDelayStiff <= 0 && mCurState != State.ATTACK && Input.GetMouseButton(0))
        {
            mAudio.PlayOneShot(mAtkAudio);
            mRig.velocity = Vector2.zero;
            mAnim.SetTrigger(State.ATTACK.ToString());
            mCurDelayStiff = mDashStiff;
            SetAtkDirection();
            ChangeStep(State.ATTACK);
        }
    }

    private void StepUseItem()
    {
        if (mItemCount > 0 && mCurDelayStiff <= 0 && mCurState != State.USEITEM && Input.GetMouseButton(1))
        {
            mItemCount--;
            mRig.velocity = Vector2.zero;
            mCurDelayStiff = mUseItemStiff;
            mAtkDirection = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            SetAtkDirection();
            mAnim.SetTrigger(State.USEITEM.ToString());
            mOilSpout.transform.forward = mAtkDirection;
            mOilSpout.SetActive(true);
            Vector3 aTrapPos = mOilTrap.transform.position;
            GameObject aTrap = Instantiate(mOilTrap);
            aTrap.transform.rotation = Quaternion.identity;
            aTrap.transform.position = aTrapPos;
            aTrap.SetActive(true);
            ChangeStep(State.USEITEM);
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D iOther)
    {
        if (mCurState == State.ATTACK)
        {
            if (iOther.gameObject.CompareTag("Vtuber"))
            {
                TriggerVtuber(iOther.gameObject);
            }
        }

        if (iOther.gameObject.CompareTag("VtuberHead"))
        {
            TriggerVtuberHead(iOther.gameObject);
        }
        else if (iOther.gameObject.CompareTag("Altar"))
        {
            PutDownHead(iOther.gameObject);
        }
    }

    #region TriggerEnterObj
    private void TriggerVtuber(GameObject iObj)
    {
        VtuberInfo aVtuberInfo = iObj.GetComponent<VtuberInfo>();
        if (aVtuberInfo != null && !aVtuberInfo.IsDead)
        {
            if (!string.IsNullOrEmpty(mHaedName))//有頭在手上情況
            {
                ThrowHead();
            }

            HeadPerform aHeadPerform = aVtuberInfo.Head.GetComponent<HeadPerform>();
            mHaedName = aVtuberInfo.Head.name;
            VtuberManager.Instance.TouchPlayer(aVtuberInfo);
            aHeadPerform.ExeCute(() =>
            {
                SetHead(aVtuberInfo.Head.transform);
            });
            mItemCount++;
        }
    }
    private void TriggerVtuberHead(GameObject iObj)
    {
        if (mCurHeadTransform == null)//手上沒頭
        {
            mHaedName = iObj.name;
            SetHead(iObj.transform);
        }
        else//手上有頭
        {
            if (mHaedName != iObj.name)//頭不一樣交換
            {
                ThrowHead();
                mHaedName = iObj.name;
                SetHead(iObj.transform);
            }
        }
    }
    #endregion

    #region Head
    private void SetHead(Transform iHeadT)
    {
        if (mCurHeadTransform != null)
        {
            ThrowHead();
        }
        mCurHeadTransform = iHeadT;
        mCurHeadTransform.position = mHeadPos.position;
        mCurHeadTransform.SetParent(mHeadPos);
        mCurHeadTransform.localPosition = Vector3.zero;
        mCurHeadTransform.GetComponent<CircleCollider2D>().enabled = false;
        PlayHeadAudio();
    }
    private void ThrowHead()
    {
        if (mCurHeadTransform != null)
        {
            Vector3 aCurPos = mCurHeadTransform.position;
            mCurHeadTransform.SetParent(null);
            mCurHeadTransform.position = aCurPos;
            StopHeadAudio();
            StartCoroutine(DelayOpenHeadTrigger(mCurHeadTransform.GetComponent<CircleCollider2D>()));
            mCurHeadTransform = null;
        }
    }
    private IEnumerator DelayOpenHeadTrigger(CircleCollider2D iCollider)
    {
        yield return new WaitForSeconds(2f);
        iCollider.enabled = true;
    }
    private void PutDownHead(GameObject iObj)
    {
        if (mCurHeadTransform != null)
        {
            Stage aAltar = iObj.GetComponent<Stage>();
            Transform aCacheTransform = aAltar.HeadAttach();
            mCurHeadTransform.SetParent(null);
            CircleCollider2D aCollider = mCurHeadTransform.GetComponent<CircleCollider2D>();
            Destroy(aCollider);
            mCurHeadTransform.SetParent(aCacheTransform);
            mCurHeadTransform.position = aCacheTransform.position;
            mCurHeadTransform.rotation = Quaternion.identity;
            StopHeadAudio();
            mCurHeadTransform = null;
            mHaedName = null;
        }
    }
    private void PlayHeadAudio()
    {
        if (!string.IsNullOrEmpty(mHaedName))
        {
            mHeadAudio.clip = VtuberManager.Instance.GetHeadAudio(mHaedName);
            mHeadAudio.Play();
        }
    }
    private void StopHeadAudio()
    {
        mHeadAudio.Stop();
    }
    #endregion
    private void SetAtkDirection()
    {
        mAtkDirection = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;
        if ((!mIsRight && mAtkDirection.x > 0) || (mIsRight && mAtkDirection.x < 0))
        {
            Flip();
        }
    }
    private void Flip()
    {
        mIsRight = !mIsRight;
        Vector3 aLocalScale = transform.localScale;
        aLocalScale.x *= -1;
        transform.localScale = aLocalScale;

        var oilScale = mOilSpout.transform.localScale;
        oilScale.x *= -1;
        mOilSpout.transform.localScale = oilScale;
    }

    public void ReturnIDLE()
    {
        if (mCurState == State.USEITEM)
        {
            mOilSpout.SetActive(false);
        }
        mRig.velocity = Vector2.zero;
        mAnim.SetFloat(State.RUN.ToString(), -1);
        ChangeStep(State.IDLE);
    }

    public void GoDie()
    {
        ThrowHead();
        ReturnIDLE();
        ChangeStep(State.DIE);
    }
}