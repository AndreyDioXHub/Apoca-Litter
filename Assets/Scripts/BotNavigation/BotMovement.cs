using UnityEngine;
using UnityEngine.Events;

public class BotMovement : MonoBehaviour
{
    public UnityEvent OnAfterFall = new UnityEvent();
    public UnityEvent<float> OnVelocity = new UnityEvent<float>();

    [SerializeField]
    private float _acceleration = 9.0f;
    [SerializeField]
    private float _speed = 1.0f;
    [SerializeField]
    private float _accelerationInAir = 3.0f;
    [SerializeField]
    private float _deceleration = 11.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float _airControl = 0.8f;

    [SerializeField]
    private float _gravity = 35;
    [SerializeField]
    private float _stickToGroundForce = 0.03f;

    [SerializeField]
    private float _rigidbodyPushForce = 1.0f;
    [SerializeField]
    private BotFindMoveDirection _navigator;

    private CharacterController _controller;
    /*
    [SerializeField]
    private Character _character;*/



    private float standingHeight;

    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private float velocityFloat;

    [SerializeField]
    private bool isGrounded;

    private bool wasGrounded;


    protected void Awake()
    {
    }

    protected void Start()
    {

        _controller = GetComponent<CharacterController>();

        standingHeight = _controller.height;
    }

    protected void Update()
    {
        isGrounded = IsGrounded();
        MoveCharacter();
        wasGrounded = isGrounded;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.moveDirection.y > 0.0f && velocity.y > 0.0f)
        {
            velocity.y = 0.0f;
        }

        Rigidbody hitRigidbody = hit.rigidbody;

        if (hitRigidbody == null)
        {
            return;
        }
        Vector3 force = (hit.moveDirection + Vector3.up * 0.35f) * velocity.magnitude * _rigidbodyPushForce;
        hitRigidbody.AddForceAtPosition(force, hit.point);
    }

    private void MoveCharacter()
    {
        if (BotsCanMove.Instance == null)
        {

        }
        else
        {
            if (!BotsCanMove.Instance.CanMove)
            {
                return;
            }
        }

        //Vector2 frameInput = _navigator.HaveDirection ? new Vector2(1, 0) : Vector2.zero;// ClampMagnitude(_botNavigation.BestDirection, 1.0f);
        var desiredDirection = _navigator.HaveDirection ? transform.forward : Vector3.zero; //new Vector3(frameInput.x, 0.0f, frameInput.y);
        /*
        if (_character.IsRunning())
        {
            desiredDirection *= speedRunning;
        }
        else
        {
            if (_character.IsAiming())
            {
                desiredDirection *= speedAiming;
            }
            else
            {
                desiredDirection *= speedWalking;
                desiredDirection.x *= walkingMultiplierSideways;
                desiredDirection.z *= (frameInput.y > 0 ? walkingMultiplierForward : walkingMultiplierBackwards);
            }
        }*/

        //desiredDirection = transform.TransformDirection(desiredDirection);
        desiredDirection *= _speed;

        if (isGrounded == false)
        {
            velocity += desiredDirection * _accelerationInAir * _airControl * Time.deltaTime;
            velocity.y -= (velocity.y >= 0 ? _gravity : _gravity) * Time.deltaTime;
        }
        else
        {
            if (velocity.y < -0.7f)
            {
                OnAfterFall?.Invoke();
            }

            velocity.y = -0.7f;
            velocity = Vector3.Lerp(velocity, new Vector3(desiredDirection.x, velocity.y, desiredDirection.z), Time.deltaTime * (desiredDirection.sqrMagnitude > 0.0f ? _acceleration : _deceleration));
        
        }

        velocityFloat = velocity.magnitude;

        OnVelocity?.Invoke(velocityFloat);

        Vector3 applied = velocity * Time.deltaTime;
        if (_controller.isGrounded)
            applied.y = -_stickToGroundForce;
        _controller.Move(applied);


        if (transform.position.y < -100)
        {
            _controller.enabled = false;
            transform.position = new Vector3(transform.position.x, 100, transform.position.z);
            _controller.enabled = true;
        }
    }

    public bool IsGrounded()
    {
        if (_controller == null)
        {
            //Debug.Log("IsGrounded controller is null");
            return true;
        }

        return _controller.isGrounded;
    }
}
