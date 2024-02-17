using Experiment.Strings;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Experiment.Player
{
    [RequireComponent(typeof(PlayerInput)), RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Configs")]
        [SerializeField] private float _standingRunSpeed = 8f;
        [SerializeField] private float _crouchingRunSpeed = 5f;
        [SerializeField] private float _proningRunSpeed = 3f;
        [SerializeField] private float _rotationSpeed = 5f;

        private PlayerInput _playerInput;
        private CharacterController _characterController;
        private InputAction _moveInput;
        private PlayerMovementState _state;
        private bool _isStanding;
        private bool _isCrouching;
        private bool _isProning;
        private PlayerCharacter _character;
        private Camera _camera;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _characterController = GetComponent<CharacterController>();
            _camera = Camera.main;
            _moveInput = _playerInput.actions[InputActionStrings.PlayerAction.Move];
        }

        public void Initialize(PlayerCharacter character)
        {
            _character = character;
        }

        public void ToggleCrouchState(InputAction.CallbackContext context)
        {
            if (IsOwner)
            {
                if (context.started)
                {
                    if (_isCrouching)
                    {
                        _character.SetMovementState(PlayerMovementState.Standing);
                    }
                    else
                    {
                        _character.SetMovementState(PlayerMovementState.Crouching);
                    }
                }
            }
        }

        public void ToggleProneState(InputAction.CallbackContext context)
        {
            if (IsOwner)
            {
                if (context.started)
                {
                    if (_isProning)
                    {
                        _character.SetMovementState(PlayerMovementState.Standing);
                    }
                    else
                    {
                        _character.SetMovementState(PlayerMovementState.Proning);
                    }
                }
            }
        }

        public void SetMovementState(PlayerMovementState state)
        {
            _state = state;

            _isStanding = state == PlayerMovementState.Standing;
            _isCrouching = state == PlayerMovementState.Crouching;
            _isProning = state == PlayerMovementState.Proning;
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            var input = _moveInput.ReadValue<Vector2>();
            var move = new Vector3(input.x, 0, input.y);

            // Take camera direction into action for player movement
            move = move.x * _camera.transform.right + move.z * _camera.transform.forward;
            move.y = 0f;
            var moveSpeed = GetCurrentStateMoveSpeed();
            _character.SetIdleServer(move == Vector3.zero);
            _character.SetMovementVectorServer(input.x, input.y);

            _characterController.Move(move * Time.deltaTime * moveSpeed);
            var targetRotation = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(
                transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private float GetCurrentStateMoveSpeed()
        {
            switch (_state)
            {
                case PlayerMovementState.Standing:
                    return _standingRunSpeed;
                case PlayerMovementState.Crouching:
                    return _crouchingRunSpeed;
                case PlayerMovementState.Proning:
                    return _proningRunSpeed;
            }
            return 0;
        }
    }
}