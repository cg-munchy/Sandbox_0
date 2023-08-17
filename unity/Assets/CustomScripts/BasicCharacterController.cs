using System;
using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomScripts
{
    public class BasicCharacterController : MonoBehaviour
    {

        [SerializeField] private LayerMaskReference mask;
        [SerializeField] private Animator animator;
        [SerializeField] private float movementSpeed;
        [SerializeField] private bool useGravity = false;
        [SerializeField] private float stepHeight;
        

        private CharacterController controller;
        private float _xDir;
        private float _zDir;
        private float _yDir;
        private float _yVelocity;
        private bool _hasAnimator;
        private bool _isValid = true;
        [FormerlySerializedAs("flags")] public CollisionFlags flagsLastFrame;
        private RaycastHit[] _results = new RaycastHit[3];
        [ShowInInspector][ReadOnly]private float _speed;

        public void SetValid(bool value) => _isValid = !value;


        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            _hasAnimator = animator != null;
        }

        private void Update()
        {
            // get inputs
            _xDir = Input.GetAxis("Horizontal");
            _zDir = Input.GetAxis("Vertical");
            var dir = new Vector3(_xDir, 0, _zDir) + Vector3.up;
            var dotValue = Mathf.Abs(1f - Vector3.Dot(dir.normalized, Vector3.up));
            if (!_hasAnimator) return;
            animator.SetFloat("speed", dotValue);
        }

        private void FixedUpdate()
        {
            if (!_isValid)
            {
                return;
            }
            if ((flagsLastFrame & CollisionFlags.Below) != 0)
                _yVelocity = 0f;
            else
            {
                _yVelocity = CustomPhysicsConfig.Instance.Gravity * Time.fixedDeltaTime;
            }

            var inputDir = new Vector2(_xDir, _zDir);
            if (inputDir.magnitude > 1.0f)
            {
                inputDir.Normalize();
            }

            var xSpeed = inputDir[0] * movementSpeed * Time.fixedDeltaTime;
            var zSpeed = inputDir[1] * movementSpeed * Time.fixedDeltaTime;

            _speed = new Vector2(xSpeed,zSpeed).magnitude;// _speed; //Vector2.ClampMagnitude(new Vector2(xSpeed, zSpeed),1f).magnitude;
            //raycast 
            var ray = new Ray(controller.transform.position+Vector3.up*stepHeight,transform.forward);
            var controllerRadius = controller.radius;
            var count = Physics.RaycastNonAlloc(ray, _results, _speed + controllerRadius, mask.Value);
            var path = Vector3.zero;
            if (count > 0)
            {
                //hitting a slope
                Debug.Log("on a slope");
                //Debug.DrawLine(ray.origin,ray.origin+ray.direction*speed,Color.green);
                path = Vector3.Cross(_results[0].normal, transform.right);
                Debug.DrawLine(ray.origin,ray.origin+ path* (_speed+ controllerRadius),Color.cyan,1.2f);
                // flagsLastFrame = controller.Move(path*_speed);
            }
            else
            {
                //flagsLastFrame = controller.Move(new Vector3(xSpeed, _yVelocity, zSpeed));
            }
            //flagsLastFrame = controller.Move(new Vector3(xSpeed, _yVelocity, zSpeed));
            var displacement = new Vector3(xSpeed, count>0? 0f:_yVelocity, zSpeed);
            //displacement = Vector3.ClampMagnitude(displacement,_speed);
            //var displacement = count > 0 ? path * _speed : new Vector3(xSpeed, _yVelocity, zSpeed);
            flagsLastFrame = controller.Move(displacement);
            transform.LookAt(transform.position + new Vector3(_xDir,0,_zDir));
            Debug.DrawLine(ray.origin,ray.origin+ray.direction*_speed,count>0? Color.red: Color.green);
        }

    }
}