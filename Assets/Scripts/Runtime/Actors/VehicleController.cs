using Runtime.Units;
using UnityEngine;
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
        public float stationaryTurnAngle = 60f;
        public float maxSpeedTurnAngle = 5f;

        [Space]
        public Vector3 wheelOffset;
        public Vector2 wheelSpacing;
        public float wheelRadius;

        [Space]
        public float counterRollTorque;

        [Space]
        public WheelCollider wheelFrontLeft;
        public WheelCollider wheelFrontRight;
        public WheelCollider wheelRearLeft;
        public WheelCollider wheelRearRight;

        private float inputThrottle;
        private float inputSteering;
        
        private float motorTorque;
        private float brakeTorque;
        private float steerAngle;
        
        private float currentSpeed;

        private int wheelsOnGround = 0;
        
        private Rigidbody body;

        private void OnValidate()
        {
            ValidateWheel(ref wheelFrontLeft, "Wheel.FL", -wheelSpacing.x, wheelSpacing.y);
            ValidateWheel(ref wheelFrontRight, "Wheel.FR", wheelSpacing.x, wheelSpacing.y);
            ValidateWheel(ref wheelRearLeft, "Wheel.BL", -wheelSpacing.x, -wheelSpacing.y);
            ValidateWheel(ref wheelRearRight, "Wheel.BR", wheelSpacing.x, -wheelSpacing.y);
        }
        
        private void ValidateWheel(ref WheelCollider wheel, string name, float spacingX, float spacingZ)
        {
            if (wheel == null) wheel = transform.Find(name).GetComponent<WheelCollider>();
            if (wheel != null)
            {
                wheel.radius = wheelRadius;
                wheel.transform.position = transform.TransformPoint(wheelOffset + new Vector3(spacingX, wheelRadius, spacingZ));
            }
        }

        protected override void Awake()
        {
            base.Awake();
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            inputThrottle = ReadAxis("Throttle");
            inputSteering = ReadAxis("Steering");
            
            currentSpeed = Vector3.Dot(transform.forward, body.linearVelocity);
                
            if (inputThrottle > 0.1f)
            {
                var targetSpeed = inputThrottle > 0f ? maxForwardSpeed : maxReverseSpeed;
                var acceleration = inputThrottle > 0f ? forwardAcceleration : reverseAcceleration;
                motorTorque = acceleration * inputThrottle * (1f - currentSpeed / targetSpeed);
                brakeTorque = 0f;
            }
            else
            {
                motorTorque = 0f;
                brakeTorque = brakePower;
            }
            
            steerAngle = stationaryTurnAngle / ((stationaryTurnAngle / maxSpeedTurnAngle - 1f) / maxForwardSpeed * Mathf.Abs(currentSpeed) + 1f) * inputSteering;
            
            UpdateWheel(wheelFrontLeft, true);
            UpdateWheel(wheelFrontRight, true);
            UpdateWheel(wheelRearLeft, false);
            UpdateWheel(wheelRearRight, false);
        }

        private void UpdateWheel(WheelCollider wheel, bool isDriveWheel)
        {
            wheel.motorTorque = isDriveWheel ? motorTorque : 0f;
            wheel.brakeTorque = isDriveWheel ? brakeTorque : 0f;
            wheel.steerAngle = isDriveWheel ? steerAngle : 0f;
            
            if (wheel.isGrounded) 
        }

        private void OnDrawGizmosSelected()
        {
            var body = GetComponent<Rigidbody>();
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(body.worldCenterOfMass, 0.2f);
        }
    }
}