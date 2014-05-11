using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace MotionCapture.BVH
{
    class End:BVHJoint
    {
        public End(string name, Vector3 offset):
            base(name,offset)
        {

        }
        public override string TypeName
        {
            get
            {
                return "End";
            }
        }
        public override string[] Channels
        {
            get
            {
                return null;
            }
        }
    }
}
