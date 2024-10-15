using System;

namespace Runtime.Actors
{
    public interface IActorInputProvider
    {
        public float ReadAxis(string name);
        public bool IsPressed(string name);
        public bool WasPressedThisFrame(string name);
        public bool WasReleasedThisFrame(string name);
        public void Subscribe(string name, Action callback);
        public void Unsubscribe(string name, Action callback);
    }
}