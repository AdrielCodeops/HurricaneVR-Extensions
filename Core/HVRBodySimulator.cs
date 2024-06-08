using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.ControllerInput;

namespace HurricaneVRExtensions.Simulator
{
    public class HVRBodySimulator : MonoBehaviour
    {
        [Header("Required Component")]
        public GameObject Rig;
        public bool autoResolveDependencies = true;

        [Header("Settings")]
        [SerializeField] protected bool canMove = true;

        [Range(0.01f, 2)]
        [SerializeField]
        protected float turnSpeed = 0.1f;

        //Hurricane dependencies
        private HVRPlayerController _hurricanePlayerController;
        private HVRPlayerInputs _hurricanePlayerInputs;

        protected bool isTurning = false;

        #region Input
#if ENABLE_INPUT_SYSTEM
        public bool IsTurningPressed => Mouse.current != null && Mouse.current.rightButton.isPressed;
        public Vector2 MouseDelta => Mouse.current.delta.ReadValue();
#elif ENABLE_LEGACY_INPUT_MANAGER
		public bool IsTurningPressed => Input.GetMouseButton(1);
        public Vector2 MouseDelta => new Vector2(Input.GetAxis("Mouse X") * 10f, Input.GetAxis("Mouse Y") * 10f);
#endif
        #endregion

        protected virtual void Start()
        {
            if (!ResolveDependencies())
            {
                Debug.Log("Auto resolve dependencies failed: The Rig parameter must be assigned with the Hurricane/Hexabody rig root object.", gameObject);
                enabled = false;
                return;
            }

            _hurricanePlayerInputs.UseWASD = true;
        }

        protected void Update()
        {
            isTurning = IsTurningPressed;
            Cursor.visible = !isTurning;
        }

        protected virtual void FixedUpdate()
        {
            if (isTurning)
            {
                TurnRig();
            }
        }

        protected virtual void LateUpdate()
        {
            if (isTurning)
            {
                TurnCamera();
            }
        }

        protected virtual void TurnCamera()
        {
            float rotationAngleY = MouseDelta.y * turnSpeed;
            _hurricanePlayerController.Camera.transform.RotateAround(_hurricanePlayerController.Camera.transform.position, _hurricanePlayerController.Camera.transform.right, -rotationAngleY);
        }

        protected virtual void TurnRig()
        {
            float rotationAngleX = MouseDelta.x * turnSpeed;
            _hurricanePlayerController.transform.RotateAround(_hurricanePlayerController.transform.position, Vector3.up, rotationAngleX);
        }

        #region Initialization
        protected virtual bool ResolveDependencies()
        {
            if (!autoResolveDependencies)
                return true;

            _hurricanePlayerController = Rig.GetComponentInChildren<HVRPlayerController>();
            _hurricanePlayerInputs = Rig.GetComponentInChildren<HVRPlayerInputs>();

            if (_hurricanePlayerController == null)
            {
                Debug.Log(DependencyError("HVRPlayerController"));
                return false;
            }

            if (_hurricanePlayerInputs == null)
            {
                Debug.Log(DependencyError("HVRPlayerInputs"));
                return false;
            }

            return true;
        }

        private string DependencyError(string component)
        {
            return string.Format("HVRBodySimulator AutoResolveDependencies Error: No {0} component found. Make sure Rig parameter is assigned with Hurricane/Hexabody rig root object.", component);
        }

        #endregion
    }
}
