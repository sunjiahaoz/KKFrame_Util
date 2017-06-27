using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KK.Frame.Util
{

    /// <summary>
    /// 参考Thinksquirrel CameraShake
    /// </summary>
    public class CameraShake : MonoBehaviour
    {

        /// <summary>
        /// 参与shake的摄像头
        /// </summary>        
        public List<Camera> _lstCameras = new List<Camera>();

        /// <summary>
        /// 执行震动的次数
        /// </summary>
        public int _nNumOfShakes = 2;

        /// <summary>
        /// 每个方向上的震动数值
        /// </summary>
        public Vector3 _vec3ShakeAmount = Vector3.zero;

        /// <summary>
        /// 每个方向的旋转数值
        /// </summary>
        public Vector3 _vec3RotationAmount = Vector3.zero;

        /// <summary>
        /// 第一次震动的初始化距离
        /// </summary>
        public float _fDistance = 0.10f;

        /// <summary>
        /// 震动速度
        /// </summary>
        public float _fSpeed = 50f;

        /// <summary>
        /// 衰减速度（0~1），值越高就会越快停止震动
        /// </summary>
        public float _fDecay = 0.20f;

        /// <summary>
        /// 如果设为true,则最终的震动速度会基于TimeScale
        /// </summary>
        public bool _bMultiplyByTimeScale = true;

        // 状态
        private bool _bShaking = false;
        private bool _bCancelling = false;

        internal class ShakeState
        {
            internal readonly Vector3 startPosition;
            internal readonly Quaternion startRotation;
            internal Vector3 shakePosition;
            internal Quaternion shakeRotation;

            internal ShakeState(Vector3 position, Quaternion rotation)
            {
                startPosition = position;
                startRotation = rotation;
                shakePosition = position;
                shakeRotation = rotation;
            }
        }

        private Dictionary<Camera, List<ShakeState>> states = new Dictionary<Camera, List<ShakeState>>();
        private Dictionary<Camera, int> shakeCount = new Dictionary<Camera, int>();

        // 震动用到的最小默认值
        private const bool checkForMinimumValues = true;
        private const float minShakeValue = 0.001f;
        private const float minRotationValue = 0.001f;

        #region _单例_
        public static CameraShake instance;
        CameraShake()
        {
            instance = this;
        }
        #endregion

        #region _静态属性_
        /// <summary>
        /// 是否正在震动
        /// </summary>
        public static bool isShaking
        {
            get
            {
                return instance.IsShaking();
            }
        }
        /// <summary>
        /// 是否正在取消
        /// </summary>
        public static bool isCancelling
        {
            get
            {
                return instance.IsCancelling();
            }
        }
        #endregion

        #region _静态方法_
        public static void Shake()
        {
            instance.DoShake();
        }
        public static void Shake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, bool multiplyByTimeScale)
        {
            instance.DoShake(numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, multiplyByTimeScale);
        }
        public static void Shake(System.Action callback)
        {
            instance.DoShake(callback);
        }
        public static void Shake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, bool multiplyByTimeScale, System.Action callback)
        {
            instance.DoShake(numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, multiplyByTimeScale, callback);
        }
        public static void CancelShake()
        {
            instance.DoCancelShake();
        }
        public static void CancelShake(float time)
        {
            instance.DoCancelShake(time);
        }
        #endregion

        #region _Events_
        /// <summary>
        /// 当一个camera开始震动的时候调用
        /// </summary>
        public event System.Action cameraShakeStarted;
        /// <summary>
        /// 当一个camera完成震动并回到初始原点的时候调用
        /// </summary>
        public event System.Action allCameraShakesCompleted;
        #endregion

        #region _公用方法_
        public bool IsShaking()
        {
            return _bShaking;
        }
        public bool IsCancelling()
        {
            return _bCancelling;
        }
        /// <summary>
        /// 执行属性中配置的数据进行震动
        /// </summary>
        public void DoShake()
        {
            Vector3 seed = Random.insideUnitSphere;

            foreach (Camera cam in _lstCameras)
            {
                StartCoroutine(DoShake_Internal(cam, seed, this._nNumOfShakes, this._vec3ShakeAmount, this._vec3RotationAmount, this._fDistance, this._fSpeed, this._fDecay, this._bMultiplyByTimeScale, null));
            }
        }

        public void DoShake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, bool multiplyByTimeScale)
        {
            Vector3 seed = Random.insideUnitSphere;

            foreach (Camera cam in _lstCameras)
            {
                StartCoroutine(DoShake_Internal(cam, seed, numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, multiplyByTimeScale, null));
            }
        }
        public void DoShake(System.Action callback)
        {
            Vector3 seed = Random.insideUnitSphere;

            foreach (Camera cam in _lstCameras)
            {
                StartCoroutine(DoShake_Internal(cam, seed, this._nNumOfShakes, this._vec3ShakeAmount, this._vec3RotationAmount, this._fDistance, this._fSpeed, this._fDecay, this._bMultiplyByTimeScale, callback));
            }
        }

        public void DoShake(int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, bool multiplyByTimeScale, System.Action callback)
        {
            Vector3 seed = Random.insideUnitSphere;

            foreach (Camera cam in _lstCameras)
            {
                StartCoroutine(DoShake_Internal(cam, seed, numberOfShakes, shakeAmount, rotationAmount, distance, speed, decay, multiplyByTimeScale, callback));
            }
        }

        public void DoCancelShake()
        {
            if (_bShaking && !_bCancelling)
            {
                _bShaking = false;
                this.StopAllCoroutines();
                foreach (Camera cam in _lstCameras)
                {
                    if (shakeCount.ContainsKey(cam))
                    {
                        shakeCount[cam] = 0;
                    }
                    ResetState(cam.transform, cam);
                }
            }
        }

        public void DoCancelShake(float time)
        {
            if (_bShaking && !_bCancelling)
            {
                this.StopAllCoroutines();
                this.StartCoroutine(DoResetState(_lstCameras, shakeCount, time));
            }
        }
        #endregion

        #region _Private方法_
        private void OnDrawGizmosSelected()
        {
            foreach (Camera cam in _lstCameras)
            {
                if (!cam)
                    continue;

                if (IsShaking())
                {
                    Vector3 offset = cam.worldToCameraMatrix.GetColumn(3);
                    offset.z *= -1;
                    offset = cam.transform.position + cam.transform.TransformPoint(offset);
                    Quaternion rot = QuaternionFromMatrix(cam.worldToCameraMatrix.inverse * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1)));
                    Matrix4x4 matrix = Matrix4x4.TRS(offset, rot, cam.transform.lossyScale);
                    Gizmos.matrix = matrix;
                }
                else
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(cam.transform.position, cam.transform.rotation, cam.transform.lossyScale);
                    Gizmos.matrix = matrix;
                }

                Gizmos.DrawWireCube(Vector3.zero, _vec3ShakeAmount);

                Gizmos.color = Color.cyan;

                if (cam.orthographic)
                {
                    Vector3 pos = new Vector3(0, 0, (cam.nearClipPlane + cam.farClipPlane) / 2f);
                    Vector3 size = new Vector3(cam.orthographicSize / cam.aspect, cam.orthographicSize * 2f, cam.farClipPlane - cam.nearClipPlane);
                    Gizmos.DrawWireCube(pos, size);
                }
                else
                {
                    Gizmos.DrawFrustum(Vector3.zero, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, (.7f / cam.aspect));
                }
            }
        }

        private IEnumerator DoShake_Internal(Camera cam, Vector3 seed, int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, bool multiplyByTimeScale, System.Action callback)
        {
            // 等待异步取消操作完成
            if (_bCancelling)
                yield return null;

            // 设置随机值
            var mod1 = seed.x > .5f ? 1 : -1;
            var mod2 = seed.y > .5f ? 1 : -1;
            var mod3 = seed.z > .5f ? 1 : -1;

            // 第一次震动
            if (!_bShaking)
            {
                _bShaking = true;

                if (cameraShakeStarted != null)
                    cameraShakeStarted();
            }

            if (shakeCount.ContainsKey(cam))
                shakeCount[cam]++;
            else
                shakeCount.Add(cam, 1);

            // 基于第一个camera获取像素宽度
            // float pixelWidth = GetPixelWidth(_lstCameras[0].transform, _lstCameras[0]);

            // 
            Transform cachedTransform = cam.transform;
            Vector3 camOffset = Vector3.zero;
            Quaternion camRot = Quaternion.identity;

            int currentShakes = numberOfShakes;
            float shakeDistance = distance;
            float rotationStrength = 1;

            float startTime = Time.realtimeSinceStartup;
            float scale = multiplyByTimeScale ? Time.timeScale : 1;
            //  float pixelScale = pixelWidth * scale;
            Vector3 start1 = Vector2.zero;
            Quaternion startR = Quaternion.identity;
            // Vector2 start2 = Vector2.zero;

            ShakeState state = new ShakeState(cachedTransform.position, cachedTransform.rotation);
            List<ShakeState> stateList;

            if (states.TryGetValue(cam, out stateList))
            {
                stateList.Add(state);
            }
            else
            {
                stateList = new List<ShakeState>();
                stateList.Add(state);
                states.Add(cam, stateList);
            }

            // 主循环
            while (currentShakes > 0)
            {
                if (checkForMinimumValues)
                {
                    // 如果旋转小于要求的最小值，braek
                    if (rotationAmount.sqrMagnitude != 0 && rotationStrength <= minRotationValue)
                        break;

                    // 如果距离小于要求的最小值,break;
                    if (shakeAmount.sqrMagnitude != 0 && distance != 0 && shakeDistance <= minShakeValue)
                        break;
                }

                var timer = (Time.realtimeSinceStartup - startTime) * speed;

                state.shakePosition = start1 + new Vector3(
                    mod1 * Mathf.Sin(timer) * (shakeAmount.x * shakeDistance * scale),
                    mod2 * Mathf.Cos(timer) * (shakeAmount.y * shakeDistance * scale),
                    mod3 * Mathf.Sin(timer) * (shakeAmount.z * shakeDistance * scale));

                state.shakeRotation = startR * Quaternion.Euler(
                    mod1 * Mathf.Cos(timer) * (rotationAmount.x * rotationStrength * scale),
                    mod2 * Mathf.Sin(timer) * (rotationAmount.y * rotationStrength * scale),
                    mod3 * Mathf.Cos(timer) * (rotationAmount.z * rotationStrength * scale));

                camOffset = GetGeometricAvg(stateList);
                camRot = GetAvgRotation(stateList);
                NormalizeQuaternion(ref camRot);

                Matrix4x4 m = Matrix4x4.TRS(camOffset, camRot, new Vector3(1, 1, -1));

                cam.worldToCameraMatrix = m * cachedTransform.worldToLocalMatrix;

                if (timer > Mathf.PI * 2)
                {
                    startTime = Time.realtimeSinceStartup;
                    shakeDistance *= (1 - Mathf.Clamp01(decay));
                    rotationStrength *= (1 - Mathf.Clamp01(decay));
                    currentShakes--;
                }
                yield return null;
            }

            // 结束判断

            shakeCount[cam]--;

            // 最后一次震动
            if (shakeCount[cam] == 0)
            {
                _bShaking = false;
                ResetState(cam.transform, cam);

                if (allCameraShakesCompleted != null)
                {
                    allCameraShakesCompleted();
                }
            }
            else
            {
                stateList.Remove(state);
            }

            if (callback != null)
                callback();
        }

        private Vector3 GetGeometricAvg(List<ShakeState> states)
        {
            float x = 0, y = 0, z = 0, l = states.Count;

            foreach (ShakeState state in states)
            {
                x -= state.shakePosition.x;
                y -= state.shakePosition.y;
                z -= state.shakePosition.z;
            }

            return new Vector3(x / l, y / l, z / l);
        }
        private Quaternion GetAvgRotation(List<ShakeState> states)
        {
            Quaternion avg = new Quaternion(0, 0, 0, 0);

            foreach (ShakeState state in states)
            {
                if (Quaternion.Dot(state.shakeRotation, avg) > 0)
                {
                    avg.x += state.shakeRotation.x;
                    avg.y += state.shakeRotation.y;
                    avg.z += state.shakeRotation.z;
                    avg.w += state.shakeRotation.w;
                }
                else
                {
                    avg.x += -state.shakeRotation.x;
                    avg.y += -state.shakeRotation.y;
                    avg.z += -state.shakeRotation.z;
                    avg.w += -state.shakeRotation.w;
                }
            }

            var mag = Mathf.Sqrt(avg.x * avg.x + avg.y * avg.y + avg.z * avg.z + avg.w * avg.w);

            if (mag > 0.0001f)
            {
                avg.x /= mag;
                avg.y /= mag;
                avg.z /= mag;
                avg.w /= mag;
            }
            else
            {
                avg = states[0].shakeRotation;
            }

            return avg;
        }
        private float GetPixelWidth(Transform cachedTransform, Camera cachedCamera)
        {
            var position = cachedTransform.position;
            var screenPos = cachedCamera.WorldToScreenPoint(position - cachedTransform.forward * .01f);
            var offset = Vector3.zero;

            if (screenPos.x > 0)
                offset = screenPos - Vector3.right;
            else
                offset = screenPos + Vector3.right;

            if (screenPos.y > 0)
                offset = screenPos - Vector3.up;
            else
                offset = screenPos + Vector3.up;

            offset = cachedCamera.ScreenToWorldPoint(offset);

            return 1f / (cachedTransform.InverseTransformPoint(position) - cachedTransform.InverseTransformPoint(offset)).magnitude;
        }

        private void ResetState(Transform cachedTransform, Camera cam)
        {
            cam.ResetWorldToCameraMatrix();
            states[cam].Clear();
        }
        private List<Vector3> offsetCache = new List<Vector3>(10);
        private List<Quaternion> rotationCache = new List<Quaternion>(10);

        private IEnumerator DoResetState(List<Camera> cameras, Dictionary<Camera, int> shakeCount, float time)
        {
            offsetCache.Clear();
            rotationCache.Clear();

            foreach (Camera cam in cameras)
            {
                offsetCache.Add((Vector3)((cam.worldToCameraMatrix * cam.transform.worldToLocalMatrix.inverse).GetColumn(3)));
                rotationCache.Add(QuaternionFromMatrix((cam.worldToCameraMatrix * cam.transform.worldToLocalMatrix.inverse).inverse * Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1))));

                if (shakeCount.ContainsKey(cam))
                {
                    shakeCount[cam] = 0;
                }
                states[cam].Clear();
            }

            float t = 0;
            _bCancelling = true;
            while (t < time)
            {
                int i = 0;
                foreach (Camera cam in cameras)
                {
                    Transform cachedTransform = cam.transform;
                    Vector3 pos = Vector3.Lerp(offsetCache[i], Vector3.zero, t / time);
                    Quaternion rot = Quaternion.Slerp(rotationCache[i], cachedTransform.rotation, t / time);
                    Matrix4x4 m = Matrix4x4.TRS(pos, rot, new Vector3(1, 1, -1));

                    cam.worldToCameraMatrix = m * cachedTransform.worldToLocalMatrix;
                    i++;
                }
                t += Time.deltaTime;
                yield return null;
            }

            foreach (Camera cam in cameras)
            {
                cam.ResetWorldToCameraMatrix();
            }
            this._bShaking = false;
            this._bCancelling = false;
        }
        #endregion

        #region Quaternion helpers
        private static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }
        private static void NormalizeQuaternion(ref Quaternion q)
        {
            float sum = 0;

            for (int i = 0; i < 4; ++i)
                sum += q[i] * q[i];

            float magnitudeInverse = 1 / Mathf.Sqrt(sum);

            for (int i = 0; i < 4; ++i)
                q[i] *= magnitudeInverse;
        }
        #endregion
    }
}
