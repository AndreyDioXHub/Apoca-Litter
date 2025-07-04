using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindChestTextView : MonoBehaviour
{
    [SerializeField]
    private PlayerNetworkResolver _resolver;
    [SerializeField]
    private Animator _failAnimator;
    [SerializeField]
    private Animator _successAnimator;
    [SerializeField]
    private Animator _rewardAnimator;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(PlayerNetworkResolver resolver)
    {
        _resolver = resolver;
        _resolver.OnGetTask.AddListener(HideFindChestText);
        _resolver.OnGetTask.AddListener(ShowReward);

        _resolver.OnFailTask.AddListener(ShowFindChestText);
        _resolver.OnPushTask.AddListener(ShowFindChestText);

        _resolver.OnFailTask.AddListener(ShowFail);
        _resolver.OnPushTask.AddListener(ShowSuccess);
    }

    public void ShowFail()
    {
        _failAnimator.SetTrigger("Trigger");
    }

    public void ShowSuccess()
    {
        _successAnimator.SetTrigger("Trigger");
    }

    public void ShowFindChestText()
    {
        gameObject.SetActive(true);
    }

    public void ShowReward()
    {
        _rewardAnimator.SetTrigger("Trigger");
    }

    public void HideFindChestText()
    {
        gameObject.SetActive(false);
    }
}
