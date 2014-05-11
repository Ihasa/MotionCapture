using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace MotionCapture.BVH
{
    class Root:BVHJoint
    {
        public override string TypeName
        {
            get
            {
                return "ROOT";
            }
        }
        //public override string[] Channels
        //{
        //    get
        //    {
        //        return new string[]{
        //            "Xposition",
        //            "Yposition",
        //            "Zposition",
        //        };
        //    }
        //}
        public Root(string name, Vector3 offset, params BVHJoint[] children) :
            base(name, offset,children)
        {
        }
    }
}
