using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;
    [SerializeField] Animator playerHandAnimator;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    public void PullPlayerCards()
    {
        playerHandAnimator.SetTrigger("PullCards");
    }
}
