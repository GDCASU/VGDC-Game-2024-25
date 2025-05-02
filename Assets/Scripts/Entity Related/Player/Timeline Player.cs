
using UnityEngine;

public class TimelinePlayer : MonoBehaviour
{
    PlayerController controller;
    PlayerAmmoManager ammoManager;
    Animator animator;
    RuntimeAnimatorController defaultPlayerController;
    [SerializeField] RuntimeAnimatorController timelineController;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        ammoManager = GetComponent<PlayerAmmoManager>();
        animator = GetComponent<Animator>();
        defaultPlayerController = animator.runtimeAnimatorController;
    }
    public void StartTimelinePlayer()
    {
        controller.enabled = false;
        ammoManager.enabled = false;
        animator.runtimeAnimatorController = timelineController;
        animator.applyRootMotion = false;
    }

    public void EndTimelinePlayer()
    {
        controller.enabled = true;
        ammoManager.enabled = true;
        animator.runtimeAnimatorController = defaultPlayerController;
        animator.applyRootMotion = true;
    }
}
