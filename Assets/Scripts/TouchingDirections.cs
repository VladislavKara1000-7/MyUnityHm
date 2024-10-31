using Assets.Scripts;
using UnityEngine;


public class TouchingDirections : MonoBehaviour
{
    [SerializeField]
    private float groundDistance = 0.05f;

    [SerializeField]
    private float wallDistance = 0.2f;

    private CapsuleCollider2D touchingCollider;
    private RaycastHit2D[] hits = new RaycastHit2D[5];
    private bool _isGround = true;

    public bool OnRightWall { get; private set; }
    public bool OnLeftWall { get; private set; }
    public bool IsOnWall => OnLeftWall || OnRightWall;

    public bool IsGround
    {
        get => _isGround;
        private set => _isGround = value;
    }

    private void Awake()
    {
        touchingCollider = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
    }

    private void CheckGround()
    {
        IsGround = touchingCollider.Cast(Vector2.down, hits, groundDistance) > 0;
      
    }

    private void CheckWall()
    {
        if (!IsGround)
        {
            OnRightWall = touchingCollider.Cast(Vector2.right, hits, wallDistance) > 0;
            OnLeftWall = touchingCollider.Cast(Vector2.left, hits, wallDistance) > 0;
        }
        else
        {
            OnRightWall = false;
            OnLeftWall = false;
        }
    }
}