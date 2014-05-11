using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;


namespace MotionCapture
{
    class KinectControler:IDisposable
    {
        #region フィールド
        KinectSensor kinect;
        //☆Kinectからどんな情報を取得するか

        /// <summary>
        /// 何フレーム分の情報を取得するか
        /// </summary>
        int getFrames;

        GraphicsDevice graphicsDevice;
        byte[] colorImageData;
        #endregion
        
        #region プロパティ
        /// <summary>
        /// ジョイントの情報。0が最新、過去のgetFrames - 1までの情報を取得
        /// </summary>
        public JointCollection[] JointStates { get; private set; }
        public int ActiveJointStates
        {
            get
            {
                for (int i = 0; i < getFrames; i++)
                {
                    if (JointStates[i] == null)
                    {
                        return i;
                    }
                }
                return getFrames;
            }
        }
        protected SkeletonPoint[] DepthData { get; private set; }
        public Texture2D ColorImage
        {
            get
            {
                //Kinectから直接受け取ったデータは、RGBの順番が逆らしい
                byte[] textureData = new byte[colorImageData.Length];
                for (int i = 0; i + 3 < textureData.Length; i+=4)
                {
                    textureData[i + 2] = colorImageData[i];//B
                    textureData[i + 1] = colorImageData[i + 1];//G
                    textureData[i] = colorImageData[i + 2];//R
                    textureData[i + 3] = 255;//A...の情報はKinectのデータには入ってない??
                }

                Texture2D texture = new Texture2D(graphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight);
                texture.SetData<byte>(textureData);
                return texture;
            }
        }
        public Dictionary<JointType, ColorImagePoint> JointPositionColor
        {
            get
            {
                if (ActiveJointStates < 1)
                    return null;
                Dictionary<JointType, ColorImagePoint> res = new Dictionary<JointType, ColorImagePoint>();

                CoordinateMapper mapper = new CoordinateMapper(kinect);
                foreach (Joint j in JointStates[0])
                {
                    res[j.JointType] = mapper.MapSkeletonPointToColorPoint(j.Position, kinect.ColorStream.Format);
                }       
                return res;
            }
        }
        #endregion
        
        #region 開始・終了処理(コンストラクタとか)
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="kinectNum">使用するKinectの番号。一台しかつないでない場合は0でよい。</param>
        public KinectControler(int kinectNum,GraphicsDevice graphicsDevice)
        {
            if (kinectNum >= KinectSensor.KinectSensors.Count)
            {
                throw new ArgumentException("Kinectが接続されていません");
            }
            if (KinectSensor.KinectSensors[kinectNum].Status != KinectStatus.Connected)
            {
                throw new Exception("Kinectが接続されていません");
            }
            //Kinectの開始処理
            kinect = KinectSensor.KinectSensors[kinectNum];
            StartKinect();
            initObjects(graphicsDevice);
            Game1.debugStr["Kinect Connection"] = "" + kinect.Status;
        }
        void initObjects(GraphicsDevice graphicsDevice)
        {
            getFrames = 60;
            JointStates = new JointCollection[getFrames];
            this.graphicsDevice = graphicsDevice;
            //ColorImage = new Texture2D(graphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight);
        }
        /// <summary>
        /// Kinectの開始処理
        /// </summary>
        /// <param name="kinect"></param>
        private void StartKinect()
        {
            //kinectのRGBカメラを有効にする
            kinect.ColorStream.Enable();
            //フレーム更新時のイベントを登録
            //kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(getColorFrameInfo);
            //↑型を明示

            //☆距離カメラの追加
            //同じく、有効にしてフレーム更新イベントを登録
            kinect.DepthStream.Enable();
            //kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(getDepthFrameInfo);

            //☆プレイヤー情報の取得のための追加
            //スケルトンを有効にする
            kinect.SkeletonStream.Enable();
            //フレーム更新イベントを登録
            //kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(getSkeletonFrameInfo);

            //開始
            kinect.Start();
        }
        /// <summary>
        /// デストラクタのようなもの
        /// </summary>
        public void Dispose()
        {
            StopKinect();
        }
        //Kinectの終了処理
        //<param name = "kinect"></param>
        private void StopKinect()
        {
            if (kinect != null)
            {
                if (kinect.IsRunning)
                {
                    ////フレーム更新イベントを消す
                    //kinect.ColorFrameReady -= getColorFrameInfo;

                    //kinect.DepthFrameReady -= getDepthFrameInfo;

                    //kinect.SkeletonFrameReady -= getSkeletonFrameInfo;

                    //kinect停止
                    kinect.Stop();
                    //インスタンス破棄
                    kinect.Dispose();
                }
            }
        }
        #endregion

        #region Kinectから情報を取得

        public void Update()
        {
            getColorFrameInfo();
            getDepthFrameInfo();
            getSkeletonFrameInfo();
        }
        /// <summary>
        /// RGBデータを取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getColorFrameInfo()
        {
            using (ColorImageFrame cFrame = kinect.ColorStream.OpenNextFrame(1))
            {
                if (cFrame != null)
                {
                    //Texture2Dに変換
                    byte[] imageData = new byte[cFrame.PixelDataLength];
                    cFrame.CopyPixelDataTo(imageData);
                    colorImageData = imageData;
                    //ColorImage.SetData(imageData);
                }
            }
        }
        /// <summary>
        /// 深度情報を取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getDepthFrameInfo()
        {
            using (DepthImageFrame dFrame = kinect.DepthStream.OpenNextFrame(1))
            {
                if (dFrame != null)
                {
                    CoordinateMapper mapper = new CoordinateMapper(kinect);
                    DepthData = new SkeletonPoint[dFrame.PixelDataLength];
                    DepthImagePixel[] depthData = new DepthImagePixel[dFrame.PixelDataLength];
                    dFrame.CopyDepthImagePixelDataTo(depthData);
                    mapper.MapDepthFrameToSkeletonFrame(DepthImageFormat.Resolution640x480Fps30, depthData, DepthData);
                }
            }
        }

        /// <summary>
        /// ジョイントの情報を取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getSkeletonFrameInfo()
        {
            using (SkeletonFrame sFrame = kinect.SkeletonStream.OpenNextFrame(1))
            {
                if (sFrame != null)
                {
                    Skeleton[] skeletons = new Skeleton[sFrame.SkeletonArrayLength];
                    sFrame.CopySkeletonDataTo(skeletons);
                    //最初に検出したスケルトンを使用
                    for (int i = 0; i < skeletons.Length; i++)
                    {
                        if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                        {
                            updateJointStates(skeletons[i].Joints);
                            Game1.debugStr["Kinect is"] = "Tracking";
                            break;
                        }else 
                            Game1.debugStr["Kinect is"] = "Not Tracking";
                    }
                }
            }
        }
        void updateJointStates(JointCollection current)
        {
            for (int i = getFrames - 1; i > 0; i--)
            {
                JointStates[i] = JointStates[i - 1];
            }
            JointStates[0] = current;
        }
        #endregion

    }
}
