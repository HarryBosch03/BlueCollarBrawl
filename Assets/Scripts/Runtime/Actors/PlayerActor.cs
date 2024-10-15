using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Actors
{
    public class PlayerActor : MonoBehaviour, IActorInputProvider
    {
        private Dictionary<string, Action> listeners = new();
        
        public InputActionAsset actions { get; private set; }

        public float ReadAxis(string name) => actions.FindAction(name).ReadValue<float>();
        public bool IsPressed(string name) => actions.FindAction(name).IsPressed();
        public bool WasPressedThisFrame(string name) => actions.FindAction(name).WasPressedThisFrame();
        public bool WasReleasedThisFrame(string name) => actions.FindAction(name).WasReleasedThisFrame();
        public void Subscribe(string name, Action callback) => listeners[name] += callback;
        public void Unsubscribe(string name, Action callback) => listeners[name] += callback;

        public void SetDevices(InputDevice[] devices) => actions.devices = devices;

        private void Awake()
        {
            actions = Instantiate(InputSystem.actions);
            
            foreach (var map in actions.actionMaps)
            {
                foreach (var action in map)
                {
                    action.performed += OnActionPerformed;
                }
            }
        }

        private void OnEnable()
        {
            actions.Enable();
        }

        private void OnDisable()
        {
            actions.Disable();
        }

        private void OnDestroy()
        {
            DestroyImmediate(actions);
        }

        private void OnActionPerformed(InputAction.CallbackContext ctx)
        {
            var name = ctx.action.name;
            if (listeners.ContainsKey(name)) listeners[name].Invoke();
        }
    }
}