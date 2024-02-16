using Cinemachine;
using Experiment.Strings;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Experiment.Player
{
    [RequireComponent(typeof(PlayerMovement)), RequireComponent(typeof(PlayerInput))]
    public class PlayerCharacter : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;

        [Header("Camera")]
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private float _cameraMoveSpeed = 10f;
        [SerializeField] private Transform _standingCameraPoint;
        [SerializeField] private Transform _crouchingCameraPoint;
        [SerializeField] private Transform _proningCameraPoint;

        private PlayerMovement _movement;
        private PlayerInput _playerInput;
        private bool _isOwner;
        private InputAction _lookInput;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _playerInput = GetComponent<PlayerInput>();
            Initialize(true);
        }

        public void Initialize(bool isOwner)
        {
            _isOwner = isOwner;
            _lookInput = _playerInput.actions[InputActionStrings.PlayerAction.Look];

            _movement.Initialize(_isOwner, this);
            SetMovementState(PlayerMovementState.Standing);
            _movement.SetMovementState(PlayerMovementState.Standing);
            _virtualCamera.m_Follow = _standingCameraPoint;
        }

        public void SetIdle(bool isIdle)
        {
            _animator.SetBool(AnimationStrings.VBot.Idle, isIdle);
        }

        public void SetMovementVector(float x, float z)
        {
            _animator.SetFloat(AnimationStrings.VBot.X, x);
            _animator.SetFloat(AnimationStrings.VBot.Z, z);
        }

        public void SetTurnValue(float value)
        {
            _animator.SetBool(AnimationStrings.VBot.Turning, value != 0);
            _animator.SetFloat(AnimationStrings.VBot.Turn, value);
        }

        public void SetMovementState(PlayerMovementState state)
        {
            _animator.SetBool(AnimationStrings.VBot.Standing, state == PlayerMovementState.Standing);
            _animator.SetBool(AnimationStrings.VBot.Crouching, state == PlayerMovementState.Crouching);
            _animator.SetBool(AnimationStrings.VBot.Proning, state == PlayerMovementState.Proning);

            MoveCameraToStatePosition(state);
            _movement.SetMovementState(state);
        }

        private void MoveCameraToStatePosition(PlayerMovementState state)
        {
            if (_isOwner)
            {
                switch (state)
                {
                    case PlayerMovementState.Standing:
                        _virtualCamera.m_Follow = _standingCameraPoint;
                        break;
                    case PlayerMovementState.Crouching:
                        _virtualCamera.m_Follow = _crouchingCameraPoint;
                        break;
                    case PlayerMovementState.Proning:
                        _virtualCamera.m_Follow = _proningCameraPoint;
                        break;
                }
            }
        }
    }
}