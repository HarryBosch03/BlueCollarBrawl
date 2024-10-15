using System;
using UnityEngine;

namespace Runtime.Actors
{
    public abstract class ActorBehaviour : MonoBehaviour
    {
        public IActorInputProvider actor { get; private set; }

        protected virtual void Awake()
        {
            actor = GetComponent<IActorInputProvider>();
        }
        
        public float ReadAxis(string name) => actor.ReadAxis(name);
        public bool IsPressed(string name) => actor.IsPressed(name);
        public bool WasPressedThisFrame(string name) => actor.WasPressedThisFrame(name);
        public bool WasReleasedThisFrame(string name) => actor.WasReleasedThisFrame(name);
        public void Subscribe(string name, Action callback) => actor.Subscribe(name, callback);
        public void Unsubscribe(string name, Action callback) => actor.Unsubscribe(name, callback);
    }
}