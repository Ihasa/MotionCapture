using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using Microsoft.Xna.Framework;
namespace MotionCapture
{
    class HumanMotions
    {
        KinectControler kinectControler;
        public HumanMotions(KinectControler controler)
        {
            kinectControler = controler;
        }

        /// <summary>
        /// 手が肩よりもどれだけ高いか
        /// </summary>
        /// <returns></returns>
        public float Shoulder_Hand_DistanceL()
        {
            if (kinectControler.ActiveJointStates > 0)
            {
                return kinectControler.JointStates[0][JointType.HandLeft].Position.Y - kinectControler.JointStates[0][JointType.ShoulderLeft].Position.Y;
            }
            return 0;
        }

        public Vector3 Shoulder_Hand_DistanceR()
        {
            if (kinectControler.ActiveJointStates > 0)
            {
                SkeletonPoint hand = kinectControler.JointStates[0][JointType.HandRight].Position;
                SkeletonPoint shoulder = kinectControler.JointStates[0][JointType.ShoulderRight].Position;
                return new Vector3(hand.X - shoulder.X, hand.Y - shoulder.Y, hand.Z - shoulder.Z);
            }
            return Vector3.Zero;
        }
    }
}
