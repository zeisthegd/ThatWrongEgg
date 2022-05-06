using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;

using Penwyn.Game;

namespace Penwyn.Tools
{
    [CreateAssetMenu(menuName = "Util/Input Reader")]
    public class InputReader : SingletonScriptableObject<InputReader>, PlayerInput.IGameplayActions
    {
        #region Gameplay Input Events

        //Movement
        public event UnityAction<Vector2> Move;

        //Skills Using
        public event UnityAction KickPressed;
        public event UnityAction KickReleased;

        public event UnityAction ItemPressed;
        public event UnityAction ItemReleased;

        public event UnityAction JumpPressed;
        public event UnityAction JumpReleased;

        public event UnityAction ChangeMouseVisibilityPressed;
        public event UnityAction ChangeMouseVisibilityReleased;

        public event UnityAction GameplayInputEnabled;
        public event UnityAction GameplayInputDisabled;

        #endregion

        #region Logic Variables

        public bool IsHoldingKick { get; set; }
        public bool IsHoldingItem { get; set; }
        public bool IsHoldingJump { get; set; }
        public bool IsHoldingGlide { get; set; }
        public bool IsHoldinghangeMouseVisibility { get; set; }

        #endregion

        private PlayerInput playerinput;

        void OnEnable()
        {
            if (playerinput == null)
            {
                playerinput = new PlayerInput();
                playerinput.Gameplay.SetCallbacks(this);
            }
        }

        public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MoveInput = context.ReadValue<Vector2>();
                Move?.Invoke(MoveInput);
            }
            else
                MoveInput = Vector2.zero;
        }

        public void OnKick(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsHoldingKick = true;
                KickPressed?.Invoke();
            }
            else if (context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            {
                IsHoldingKick = false;
                KickReleased?.Invoke();
            }

        }

        public void OnItem(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsHoldingItem = true;
                ItemPressed?.Invoke();
            }
            else if (context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            {
                IsHoldingItem = false;
                ItemReleased?.Invoke();
            }
        }


        public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {

            if (context.started)
            {
                JumpPressed?.Invoke();
                IsHoldingJump = true;
            }
            else if (context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            {
                JumpReleased?.Invoke();
                IsHoldingJump = false;
            }
        }


        public void OnChangeCursorVisibility(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ChangeMouseVisibilityPressed?.Invoke();
                IsHoldinghangeMouseVisibility = true;
            }
            else if (context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            {
                ChangeMouseVisibilityReleased?.Invoke();
                IsHoldinghangeMouseVisibility = false;
            }
        }

        public void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {

        }

        public void EnableGameplayInput()
        {
            playerinput.Gameplay.Enable();
            GameplayInputEnabled?.Invoke();
        }

        public void DisableGameplayInput()
        {
            playerinput.Gameplay.Disable();
            GameplayInputDisabled?.Invoke();
        }

        void OnDisable()
        {
            DisableGameplayInput();
        }

        public Vector2 MoveInput { get; set; }
    }
}
