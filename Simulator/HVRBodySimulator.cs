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

        [Header("Parameters")]
        [SerializeField] protected bool _canMove = true;
        [SerializeField] protected float _turnSpeed = 4f;

        //Hurricane dependencies
        private HVRPlayerController _hurricanePlayerController;
        private HVRPlayerInputs _hurricanePlayerInputs;

        protected bool _isTurning = false;

        #region Input
#if ENABLE_INPUT_SYSTEM
        public bool HasTurningStarted => Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
        public bool HasTurningEnded => Mouse.current.rightButton.wasReleasedThisFrame;
        public Vector2 MouseDelta => Mouse.current.delta.ReadValue();
#elif ENABLE_LEGACY_INPUT_MANAGER
        public bool HasTurningStarted => Input.GetMouseButtonDown(1);
        public bool HasTurningEnded => Input.GetMouseButtonUp(1);
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
            _hurricanePlayerInputs.UseWASD = _canMove;
        }
        protected void Update()
        {
            if (HasTurningStarted)
                _isTurning = true;
            if (HasTurningEnded)
                _isTurning = false;

            Cursor.visible = !_isTurning;
        }

        protected virtual void FixedUpdate()
        {
            if (_isTurning)
            {
                TurnRig();
            }
        }

        protected virtual void TurnRig()
        {
            float rotationAngleX = MouseDelta.x * Time.deltaTime * _turnSpeed;
            _hurricanePlayerController.transform.RotateAround(_hurricanePlayerController.transform.position, Vector3.up, rotationAngleX);

            float rotationAngleY = MouseDelta.y * Time.deltaTime * _turnSpeed;
            _hurricanePlayerController.Camera.transform.RotateAround(_hurricanePlayerController.Camera.transform.position, _hurricanePlayerController.Camera.transform.right, -rotationAngleY);
        }

        #region Initialization
        protected virtual bool ResolveDependencies()
        {
            if (!autoResolveDependencies)
                return true;

            _hurricanePlayerController = Rig.GetComponentInChildren<HVRPlayerController>();
            if (_hurricanePlayerController == null)
            {
                Debug.Log(DependencyError("HVRPlayerController"));
                return false;
            }

            _hurricanePlayerInputs = Rig.GetComponentInChildren<HVRPlayerInputs>();
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

