using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionCapture.BVH
{
    class Hierarchy
    {
        Root rootBone;
        public Hierarchy(Root root)
        {
            rootBone = root;
        }
        public override string ToString()
        {
            return "HIERARCHY\n" + rootBone.ToString();
        }
    }
}
