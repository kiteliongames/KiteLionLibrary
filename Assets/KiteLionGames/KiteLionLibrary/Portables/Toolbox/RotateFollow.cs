using KiteLionGames.Utilities.Editor;
using UnityEngine;


namespace KiteLionGames
{
    namespace Utilities.Camera
    {
        /// <summary>
        /// Given a target, this script will rotate the object to follow the target like a CCTV Camera.
        /// Set a limit on how far the object can rotate on each axis. And also how far the target can go before the object stops following.
        /// </summary>
        public class RotateFollow : MonoBehaviour
        {
            /// <summary>
            /// You can assign target's X position to affect Pitch/Yaw/Roll, for example.
            /// </summary>
            public enum FollowAxis
            {
                Pitch,
                Yaw,
                Roll
            }

            public GameObject FollowTarget;

            [SerializeField]
            private float _FollowSpeed = 1.0f;

            /// <summary>
            /// Disable any axis you don't want to follow.
            /// </summary>
            [Header("Disable any axis you don't want to follow.")]
            public bool FollowX = true;
            public bool FollowY = true;
            public bool FollowZ = true;

            /// <summary>
            /// Set which axis you want to affect when the target moves on the X axis.
            /// </summary>
            [Header("Change the axis the target position impacts.")]
            public FollowAxis XAffects;
            /// <summary>
            /// Set which axis you want to affect when the target moves on the Y axis.
            /// </summary>
            public FollowAxis YAffects;
            /// <summary>
            /// Set which axis you want to affect when the target moves on the Z axis.
            /// </summary>
            public FollowAxis ZAffects;

            [MinMaxSlider(-180, 180)]
            public Vector2 RangePitch;
            [MinMaxSlider(-180, 180)]
            public Vector2 RangeYaw;
            [MinMaxSlider(-180, 180)]
            public Vector2 RangeRoll;

            /// <summary>
            /// Range limits of -100 and 100 are abitrary. You can set them to whatever you want for your designer.
            /// </summary>
            [MinMaxSlider(-100, 100)]
            public Vector2 RangeX;
            [MinMaxSlider(-100, 100)]
            public Vector2 RangeY;
            [MinMaxSlider(-100, 100)]
            public Vector2 RangeZ;

            private float MinX => RangeX.x;
            private float MaxX => RangeX.y;
            private float MinY => RangeY.x;
            private float MaxY => RangeY.y;
            private float MinZ => RangeZ.x;
            private float MaxZ => RangeZ.y;
            private float percentX;
            private float percentY;
            private float percentZ;
            private float targetPitch; // on the x axis
            private float targetYaw; // on the y axis
            private float targetRoll; // on the z axis
            private Quaternion targetRotation;

            void Start()
            {
                targetPitch = transform.rotation.eulerAngles.x;
                targetYaw = transform.rotation.eulerAngles.y;
                targetRoll = transform.rotation.eulerAngles.z;
            }

            void Update()
            {
                if (FollowTarget == null)
                    return;

                if (FollowX)
                {
                    if (FollowTarget.transform.position.x >= MinX && FollowTarget.transform.position.x <= MaxX)
                    {
                        // This math is to get a percentage of how far the target is between the min and max range.
                        // This percentage is then used to lerp between the min and max rotation values.
                        percentX = MathHelper(FollowTarget.transform.position.x, MinX, MaxX);
                        switch (XAffects)
                        {
                            case FollowAxis.Pitch:
                                targetPitch = Mathf.Lerp(RangePitch.x, RangePitch.y, percentX);
                                break;
                            case FollowAxis.Yaw:
                                targetYaw = Mathf.Lerp(RangeYaw.x, RangeYaw.y, percentX);
                                break;
                            case FollowAxis.Roll:
                                targetRoll = Mathf.Lerp(RangeRoll.x, RangeRoll.y, percentX);
                                break;
                        }
                    }
                }
                if (FollowY)
                {
                    if (FollowTarget.transform.position.y >= MinY && FollowTarget.transform.position.y <= MaxY)
                    {
                        percentY = MathHelper(FollowTarget.transform.position.y, MinY, MaxY);
                        switch (YAffects)
                        {
                            case FollowAxis.Pitch:
                                targetPitch = Mathf.Lerp(RangePitch.x, RangePitch.y, percentY);
                                break;
                            case FollowAxis.Yaw:
                                targetYaw = Mathf.Lerp(RangeYaw.x, RangeYaw.y, percentY);
                                break;
                            case FollowAxis.Roll:
                                targetRoll = Mathf.Lerp(RangeRoll.x, RangeRoll.y, percentY);
                                break;
                        }
                    }
                }
                if (FollowZ)
                {
                    if (FollowTarget.transform.position.z >= MinZ && FollowTarget.transform.position.z <= MaxZ)
                    {
                        percentZ = MathHelper(FollowTarget.transform.position.z, MinZ, MaxZ);
                        switch (ZAffects)
                        {
                            case FollowAxis.Pitch:
                                targetPitch = Mathf.Lerp(RangePitch.x, RangePitch.y, percentZ);
                                break;
                            case FollowAxis.Yaw:
                                targetYaw = Mathf.Lerp(RangeYaw.x, RangeYaw.y, percentZ);
                                break;
                            case FollowAxis.Roll:
                                targetRoll = Mathf.Lerp(RangeRoll.x, RangeRoll.y, percentZ);
                                break;
                        }
                    }
                }

                targetPitch *= -1; // invert the pitch fix

                targetRotation = Quaternion.Euler(targetPitch, targetYaw, targetRoll);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _FollowSpeed);
            }
            private float MathHelper(float position, float min, float max)
            {
                return Mathf.Abs(position - min) / Mathf.Abs(max - min);
            }
        }

    }
}