using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Experiment.Strings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Experiment.Player
{
    [RequireComponent(typeof(PlayerMovement)), RequireComponent(typeof(PlayerCharacter))]
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [Header("Camera Positions")]
        [SerializeField] private float _cameraMoveSpeed = 10f;
        [SerializeField] private Transform _standingCameraPoint;
        [SerializeField] private Transform _crouchingCameraPoint;
        [SerializeField] private Transform _proningCameraPoint;

        private PlayerMovement _movement;
        private PlayerCharacter _character;
        private Camera _camera;
        private bool _isOwner;
        private bool _isStanding;
        private bool _isCrouching;
        private bool _isProning;
        private TweenerCore<Vector3, Vector3, VectorOptions> _cameraMoveTween;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _camera = Camera.main;
            Initialize(true);
        }

        public void Initialize(bool isOwner)
        {
            _isOwner = isOwner;
            _isStanding = true;
            _isCrouching = false;
            _isProning = false;

            _movement.Initialize(_isOwner, this);
            SetMovementState(PlayerMovementState.Standing);
            _movement.SetMovementState(PlayerMovementState.Standing);
            AssignCameraToPlayer();
        }

        private void AssignCameraToPlayer()
        {
            if (_isOwner)
            {
                _camera.transform.parent = transform;
            }
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
            _isStanding = state == PlayerMovementState.Standing;
            _isCrouching = state == PlayerMovementState.Crouching;
            _isProning = state == PlayerMovementState.Proning;

            _animator.SetBool(AnimationStrings.VBot.Standing, _isStanding);
            _animator.SetBool(AnimationStrings.VBot.Crouching, _isCrouching);
            _animator.SetBool(AnimationStrings.VBot.Proning, _isProning);

            MoveCameraToStatePosition(state);
            _movement.SetMovementState(state);
        }

        private void MoveCameraToStatePosition(PlayerMovementState state)
        {
            if (_isOwner)
            {
                // Clear previous tween, if it exists or currently processing
                if (_cameraMoveTween != null)
                {
                    _cameraMoveTween.Kill();
                    _cameraMoveTween = null;
                }

                switch (state)
                {
                    case PlayerMovementState.Standing:
                        _cameraMoveTween = _camera.transform.DOMove(
                            _standingCameraPoint.position, _cameraMoveSpeed).SetSpeedBased();
                        break;
                    case PlayerMovementState.Crouching:
                        _cameraMoveTween = _camera.transform.DOMove(
                            _crouchingCameraPoint.position, _cameraMoveSpeed).SetSpeedBased();
                        break;
                    case PlayerMovementState.Proning:
                        _cameraMoveTween = _camera.transform.DOMove(
                            _proningCameraPoint.position, _cameraMoveSpeed).SetSpeedBased();
                        break;
                }
            }
        }

        public void ToggleCrouchState(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (_isCrouching)
                {
                    SetMovementState(PlayerMovementState.Standing);
                }
                else
                {
                    SetMovementState(PlayerMovementState.Crouching);
                }
            }
        }

        public void ToggleProneState(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (_isProning)
                {
                    SetMovementState(PlayerMovementState.Standing);
                }
                else
                {
                    SetMovementState(PlayerMovementState.Proning);
                }
            }
        }
    }
}