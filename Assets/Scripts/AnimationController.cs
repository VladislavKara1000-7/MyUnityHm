using Assets.Scripts;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private TouchingDirections _touchingDirections;
    private PlayerController _playerController;
    // Animation parameter IDs
    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isGroundedHash;
    private int _jumpHash;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _touchingDirections = GetComponent<TouchingDirections>();
        _playerController = GetComponent<PlayerController>();

        CacheAnimationHashes();
    }

    private void CacheAnimationHashes()
    {
        _isWalkingHash = Animator.StringToHash(PlayerAnimation.IsMoving);
        _isRunningHash = Animator.StringToHash(PlayerAnimation.IsRunning);
        _isGroundedHash = Animator.StringToHash(PlayerAnimation.IsGrounded);
        _jumpHash = Animator.StringToHash(PlayerAnimation.IsJump);
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        _animator.SetBool(_isWalkingHash, _playerController.IsMoving);
        _animator.SetBool(_isRunningHash, _playerController.IsRunning);
        _animator.SetBool(_isGroundedHash, _touchingDirections.IsGround);
        _animator.SetBool(_jumpHash, _playerController.IsJump);
    }

    public void Flip(float x)
    {
        if ((x > 0 && transform.localScale.x < 0) || (x < 0 && transform.localScale.x > 0))
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }
}