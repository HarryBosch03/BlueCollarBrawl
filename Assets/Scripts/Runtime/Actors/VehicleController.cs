using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Runtime.Actors
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : ActorBehaviour
    {
        public float maxForwardSpeed;
        public float maxReverseSpeed;
        public float forwardAcceleration;
        public float reverseAcceleration;
        public float brakePower;
        public float turnSpeed;

        [Space]
        public Vector3 wheelOffset;
        public Vector2 wheelSpacing;
        public float wheelRadius;

        private Rigidbody body;

        protected override void Awake()
        {
            base.Awake();
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            GetWheelDisplacement(wheelSpacing.x, wheelSpacing.y);
            GetWheelDisplacement(-wheelSpacing.x, wheelSpacing.y);
            GetWheelDisplacement(wheelSpacing.x, -wheelSpacing.y);
            GetWheelDisplacement(-wheelSpacing.x, -wheelSpacing.y);
        }

        private void GetWheelDisplacement(float wheelX, float wheelZ)
        {
            var position = transform.TransformPoint(wheelOffset + new Vector3(wheelX, 0f, wheelZ));
            var groundPoint = position - transform.up * wheelRadius;
            var ray = new Ray(position + transform.up * wheelRadius, -transform.up);
            if (Physics.Raycast(ray, out var hit, wheelRadius * 2f))
            {
                var velocity = Vector3.Dot(hit.normal, body.GetPointVelocity(hit.point));
                var force = hit.normal * Mathf.Max(-velocity, 0f);

                body.linearVelocity += Vector3.Project(force, (hit.point - body.worldCenterOfMass).normalized);
                body.angularVelocity += Vector3.Cross(hit.point - body.worldCenterOfMass, force);
                transform.position += Vector3.Project(hit.point - groundPoint, hit.normal);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var body = GetComponent<Rigidbody>();
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(body.worldCenterOfMass, 0.2f);

            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(wheelOffset + new Vector3(wheelSpacing.x, 0f, wheelSpacing.y), wheelRadius);
            Gizmos.DrawWireSphere(wheelOffset + new Vector3(-wheelSpacing.x, 0f, wheelSpacing.y), wheelRadius);
            Gizmos.DrawWireSphere(wheelOffset + new Vector3(wheelSpacing.x, 0f, -wheelSpacing.y), wheelRadius);
            Gizmos.DrawWireSphere(wheelOffset + new Vector3(-wheelSpacing.x, 0f, -wheelSpacing.y), wheelRadius);
        }
    }
}