using Cinemachine;
using Experiment.Strings;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Experiment.Player
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerCharacter : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;

        [Header("Camera")]
        [SerializeField] private Transform _standingCameraPoint;
        [SerializeField] private Transform _crouchingCameraPoint;
        [SerializeField] private Transform _proningCameraPoint;

        private CinemachineVirtualCamera _virtualCamera;
        private PlayerMovement _movement;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            Initialize();
        }

        public void Initialize()
        {
            _movement.Initialize(this);
            SetMovementState(PlayerMovementState.Standing);
            _movement.SetMovementState(PlayerMovementState.Standing);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (base.IsOwner)
            {
                if (_virtualCamera != null)
                {
                    _virtualCamera.m_Follow = _standingCameraPoint;
                }
            }
        }

        [ObserversRpc]
        private void SetIdle(bool isIdle)
        {
            _animator.SetBool(AnimationStrings.VBot.Idle, isIdle);
        }

        [ServerRpc]
        public void SetIdleServer(bool isIdle)
        {
            SetIdle(isIdle);
        }

        [ObserversRpc]
        private void SetMovementVector(float x, float z)
        {
            _animator.SetFloat(AnimationStrings.VBot.X, x);
            _animator.SetFloat(AnimationStrings.VBot.Z, z);
        }

        [ServerRpc]
        public void SetMovementVectorServer(float x, float z)
        {
            SetMovementVector(x, z);
        }

        public void SetMovementState(PlayerMovementState state)
        {
            SetAnimationStateServer(state);
            MoveCameraToStatePosition(state);
            _movement.SetMovementState(state);
        }

        [ObserversRpc]
        private void SetAnimationState(PlayerMovementState state)
        {
            _animator.SetBool(AnimationStrings.VBot.Standing, state == PlayerMovementState.Standing);
            _animator.SetBool(AnimationStrings.VBot.Crouching, state == PlayerMovementState.Crouching);
            _animator.SetBool(AnimationStrings.VBot.Proning, state == PlayerMovementState.Proning);
        }

        [ServerRpc]
        private void SetAnimationStateServer(PlayerMovementState state)
        {
            SetAnimationState(state);
        }

        private void MoveCameraToStatePosition(PlayerMovementState state)
        {
            if (IsOwner)
            {
                if (_virtualCamera == null)
                {
                    return;
                }

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