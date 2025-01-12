using RobbieWagnerGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement2D : MonoBehaviour
{
    [SerializeField] private UnitAnimator unitAnimator;
    public bool canMove = true;
    [HideInInspector] public bool moving = false;
    private UnitAnimationState currentAnimationState;
    private Vector2 movementVector = Vector2.zero;
    [SerializeField] private float currentWalkSpeed = 3;
    public Rigidbody2D rb;
    public LayerMask collisionLayers;

    [SerializeField] private AudioSource footstepAudioSource;

    private Vector3 lastFramePos = Vector3.zero;

    //[SerializeField][Range(-1, 0)] private float wallPushback = -.08f;
    private HashSet<Collider2D> colliders;

    public static CharacterMovement2D Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        colliders = new HashSet<Collider2D>();

        InputManager.Instance.gameControls.PLAYER.Move.performed += OnMove;
        InputManager.Instance.gameControls.PLAYER.Move.canceled += StopPlayer;
    }

    private void FixedUpdate() 
    {
        if (canMove && movementVector.y != 0)
        {
            Vector2 normalizedMovement = movementVector.normalized;
            Vector2 desiredVelocity = normalizedMovement * currentWalkSpeed;

            Debug.Log(normalizedMovement);

            Vector2 verticalMove = new Vector2(0, desiredVelocity.y);
            RaycastHit2D hitVertical = Physics2D.Raycast(rb.position + verticalMove.normalized * .301f, verticalMove, Mathf.Abs(verticalMove.normalized.y * .001f), collisionLayers);

            float finalY = hitVertical.collider == null ? verticalMove.y : 0;

            rb.velocity = new Vector2(0, finalY);
        }
        else
        {
            // No movement input
            rb.velocity = Vector2.zero;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if(canMove)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if(movementVector.x != input.x && input.x != 0f)
            {
                //if(input.x > 0) unitAnimator.ChangeAnimationState(UnitAnimationState.WalkRight);
                //else unitAnimator.ChangeAnimationState(UnitAnimationState.WalkLeft);
                moving = true;
            }
            else if(input.x == 0 && movementVector.y != input.y && input.y != 0f)
            {
                //if(input.y > 0) unitAnimator.ChangeAnimationState(UnitAnimationState.WalkForward);
                //else unitAnimator.ChangeAnimationState(UnitAnimationState.WalkBack);
                moving = true;
            }
            else if(input.x == 0 && input.y == 0)
            {
                //if(movementVector.x > 0)unitAnimator.ChangeAnimationState(UnitAnimationState.IdleRight);
                //else if(movementVector.x < 0)unitAnimator.ChangeAnimationState(UnitAnimationState.IdleLeft);
                //else if(movementVector.y > 0) unitAnimator.ChangeAnimationState(UnitAnimationState.IdleForward);
                //else unitAnimator.ChangeAnimationState(UnitAnimationState.Idle);
                moving = false;
                //if(footstepAudioSource != null) StopMovementSounds();
            }

            movementVector = input;
        }
    }

    private void OnDisable()
    {
        StopPlayer();
    }

    public void StopPlayer(InputAction.CallbackContext context)
    {
        StopPlayer();
    }

    public void StopPlayer()
    {
        //if(movementVector.x > 0) unitAnimator.ChangeAnimationState(UnitAnimationState.IdleRight);
        //else if(movementVector.x < 0) unitAnimator.ChangeAnimationState(UnitAnimationState.IdleLeft);
        //else if(movementVector.y > 0) unitAnimator.ChangeAnimationState(UnitAnimationState.IdleForward);
        //else if(movementVector != Vector2.zero) unitAnimator.ChangeAnimationState(UnitAnimationState.Idle);

        movementVector = Vector3.zero;
        moving = false;
        //if(footstepAudioSource != null) StopMovementSounds();
    }

    public void CeasePlayerMovement()
    {
        canMove = false;
        StopPlayer();
    }

    //public void PlayMovementSounds()
    //{
    //    footstepAudioSource.Play();
    //}

    //public void StopMovementSounds()
    //{
    //    footstepAudioSource.Stop();
    //}

    private void OnCollisionEnter2D(Collision2D other)
    {
        colliders.Add(other.collider);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        colliders.Remove(other.collider);
    }
}
