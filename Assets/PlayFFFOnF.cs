using UnityEngine;

public class PlayFFFOnF : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string animationStateName = "FFF";

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && animator != null)
        {
            animator.Play(animationStateName, 0, 0f);
        }
    }
}
