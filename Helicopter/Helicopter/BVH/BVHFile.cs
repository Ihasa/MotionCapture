using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionCapture.BVH
{
    class BVHFile
    {
        Hierarchy hierarchy;
        Motion motion;

        public BVHFile(Hierarchy hierarchy, Motion motion)
        {
            this.hierarchy = hierarchy;
            this.motion = motion;
        }
        public override string ToString()
        {
            return hierarchy.ToString() + motion.ToString();
        }

    }
}
