using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionCapture.BVH
{
    class Motion
    {
        Root root;
        int frames;
        float spf;
        float[][] motionData;
        public Motion(Root root,int frames, float spf)
        {
            this.root = root;
            this.frames = frames;
            this.spf = spf;
            motionData = new float[frames][];
            for (int i = 0; i < motionData.Length; i++)
            {
                motionData[i] = new float[root.TotalChannels];
                //とりあえず0ですべて初期化してみる
                for (int j = 0; j < motionData[i].Length; j++)
                {
                    motionData[i][j] = 0;
                }
            }
        }
        public Motion(Root root, float spf, float[][] data)
        {
            this.root = root;
            frames = data.Length;
            this.spf = spf;
            motionData = new float[data.Length][];
            for (int i = 0; i < motionData.Length; i++)
            {
                motionData[i] = new float[data[i].Length];
                for (int j = 0; j < motionData[i].Length; j++)
                {
                    motionData[i][j] = data[i][j];
                }
            }
        }

        public void SetData(float[][] data)
        {
            if (motionData.Length != data.Length)
                throw new ArgumentException();
            for (int i = 0; i < motionData.Length; i++)
            {
                if (motionData[i].Length != data[i].Length)
                    throw new ArgumentException();

                for (int j = 0; j < motionData[i].Length; j++)
                {
                    motionData[i][j] = data[i][j];
                }
            }
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("MOTION");
            builder.Append("Frames: ").Append(frames).AppendLine();
            builder.Append("Frame Time: ").Append(spf).AppendLine();
            foreach (float[] dataPerOneframe in motionData)
            {
                foreach (float data in dataPerOneframe)
                {
                    builder.Append(data).Append(" ");
                }
                builder.AppendLine();                
            }

            return builder.ToString();
        }
    }
}
