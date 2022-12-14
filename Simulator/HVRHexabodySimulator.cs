using UnityEngine;
using HexabodyVR.PlayerController;

namespace HurricaneVRExtensions.Simulator
{
    public class HVRHexabodySimulator : HVRBodySimulator
    {
        //Hexabody dependencies
        private HVRHexaBodyInputs _hexabodyInputs;
        private HexaBodyPlayer4 _hexabodyPlayer4;

        protected override void Start()
        {
            if (!ResolveDependencies())
            {
                Debug.Log("Auto resolve dependencies failed: The Rig parameter must be assigned with the Hurricane/Hexabody rig root object.", gameObject);
                enabled = false;
                return;
            }

            _hexabodyInputs.KeyboardDebug = _canMove;
        }

        protected override void FixedUpdate()
        {
            if (_isTurning)
            {
                TurnRig();
            }
        }

        protected override void TurnRig()
        {
            float rotationAngleX = MouseDelta.x * Time.deltaTime * _turnSpeed;
            _hexabodyPlayer4.Pelvis.transform.RotateAround(_hexabodyPlayer4.Pelvis.transform.position, Vector3.up, rotationAngleX);

            float rotationAngleY = MouseDelta.y * Time.deltaTime * _turnSpeed;
            _hexabodyPlayer4.Camera.transform.RotateAround(_hexabodyPlayer4.Camera.transform.position, _hexabodyPlayer4.Camera.transform.right, -rotationAngleY);
        }

        #region Initialization
        protected override bool ResolveDependencies()
        {
            if (!autoResolveDependencies)
                return true;

            _hexabodyInputs = Rig.GetComponentInChildren<HVRHexaBodyInputs>();

            if (_hexabodyInputs == null)
            {
                Debug.Log(DependencyError("HVRHexaBodyInputs"));
                return false;
            }

            _hexabodyPlayer4 = Rig.GetComponentInChildren<HexaBodyPlayer4>();

            if (_hexabodyPlayer4 == null)
            {
                Debug.Log(DependencyError("HexaBodyPlayer4"));
                return false;
            }

            return true;
        }

        private string DependencyError(string component)
        {
            return string.Format("HVRHexabodySimulator AutoResolveDependencies Error: No {0} component found. Make sure Rig parameter is assigned with Hurricane/Hexabody rig root object.", component);
        }
        #endregion

    }
}

