using System;
using System.Numerics;
using Runtime.Units;
using UnityEngine;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Runtime.Actors
{
    [RequireComponent(typeof(Rigidbody))]
    public class VehicleController : ActorBehaviour
    {
        [SpeedUnit(SpeedUnitAttribute.Unit.KmpH)]
        public float maxForwardSpeed;
        [SpeedUnit(SpeedUnitAttribute.Unit.KmpH)]
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
                var normalDv = hit.normal * Mathf.Max(-velocity, 0f) * 0.25f;

                ChangeVelocityAtPoint(normalDv, hit.point);
                transform.position += Vector3.Project(hit.point - groundPoint, hit.normal);
                
                var tangentForcePosition = groundPoint;
                tangentForcePosition += Vector3.Project(body.worldCenterOfMass - tangentForcePosition, hit.normal);
                velocity = Vector3.Dot(transform.right, body.GetPointVelocity(tangentForcePosition));
                var tangentDv = transform.right * -velocity;
                ChangeVelocityAtPoint(tangentDv, tangentForcePosition);
            }
        }

        private void ChangeVelocityAtPoint(Vector3 dv, Vector3 point)
        {
            body.linearVelocity += dv;
            body.angularVelocity += Vector3.Cross(point - body.worldCenterOfMass, dv);
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