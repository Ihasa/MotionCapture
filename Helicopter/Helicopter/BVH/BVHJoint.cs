using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace MotionCapture.BVH
{
    class BVHJoint
    {
        string name;
        Vector3 offset;
        float[] transforms;
        BVHJoint[] children=null;

        public virtual string TypeName { get { return "JOINT"; } }
        public virtual string[] Channels
        {
            get
            {
                return new string[]{
                    "Xposition",
                    "Yposition",
                    "Zposition"
                };
            }
        }
        //public virtual string[] Channels
        //{
        //    get
        //    {
        //        return new string[]{
        //            "Xrotation",
        //            "Yrotation",
        //            "Zrotation"
        //        };
        //    }
        //}
        public int TotalChannels
        {
            get
            {
                int res = 0;
                if (Channels != null)
                {
                    res += Channels.Length;
                }
                if(children != null)
                {
                    foreach (BVHJoint j in children)
                    {
                        res += j.TotalChannels;
                    }
                }
                return res;
            }
        }


        public BVHJoint(string name, Vector3 offset,params BVHJoint[] children)
        {
            this.name = name;
            this.offset = offset;
            this.children = children;
            if(Channels != null)
                transforms = new float[Channels.Length];
        }
        public void SetOffset(Vector3 val)
        {
            offset = val;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(TypeName).Append(" ").AppendLine(name);
            builder.AppendLine("{");
            builder.Append("\tOFFSET ").Append(offset.X).Append(" ").Append(offset.Y).Append(" ").Append(offset.Z).AppendLine();
            if (Channels != null)
            {
                builder.Append("\tCHANNELS ").Append(Channels.Length);
                foreach (string str in Channels)
                {
                    builder.Append(" ").Append(str);
                }
                builder.AppendLine();
            }

            if (children != null)
            {
                foreach (BVHJoint j in children)
                {
                    builder.Append('\t').AppendLine(j.ToString());
                }
            }
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
