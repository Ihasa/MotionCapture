using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Microsoft.Kinect;
using MotionCapture.BVH;

namespace MotionCapture
{
    static class Extension
    {
        public static Vector3 ToVector3R(this SkeletonPoint skeletonPoint)
        {
            return new Vector3(-skeletonPoint.X, skeletonPoint.Y, -skeletonPoint.Z);
        }

        public static Vector3 ToVector3L(this SkeletonPoint skeletonPoint)
        {
            return new Vector3(skeletonPoint.X, skeletonPoint.Y, skeletonPoint.Z);
        }
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KinectControler controler;
        Texture2D point;
        Drawer2D drawer;
        Root root;
        Motion motion;
        Hierarchy hierarchy;
        StreamWriter writer;
        string path = @"C:\Users\tamamaken\Desktop\";
        bool capturing = false;
        List<float[]> motionData;

        public static Dictionary<string, string> debugStr = new Dictionary<string, string>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.Title = "MotionCaptureTest";
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;

            motionData = new List<float[]>();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            try
            {
                controler = new KinectControler(0, GraphicsDevice);
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                Exit();
            }
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            point = Content.Load<Texture2D>("jointPoint");
            drawer = new Drawer2D(GraphicsDevice, Content.Load<SpriteFont>("SpriteFont1"));
            //Root root = new Root("hip", Vector3.Zero,
            //    new BVHJoint("thighL", new Vector3(-1, -0.5f, 0),
            //        new BVHJoint("kneeL", new Vector3(0, -1, 0.5f),
            //            new BVHJoint("ankleL", new Vector3(0, -1, -0.5f),
            //                new End("site", new Vector3(0, -0.5f, 1))
            //            )
            //        )
            //    ),
            //    new BVHJoint("thighR", new Vector3(1, -0.5f, 0),
            //        new BVHJoint("kneeR", new Vector3(0, -1, 0.5f),
            //            new BVHJoint("ankleR", new Vector3(0, -1, -0.5f),
            //                new End("site", new Vector3(0, -0.5f, 1))
            //            )
            //        )
            //    )
            //);
            //try
            //{
            //    Motion motion = new Motion(root, 100, 0.016f);
            //    Hierarchy hi = new Hierarchy(root);
            //    writer.WriteLine(new BVHFile(hi, motion).ToString());
            //}
            //catch (Exception e)
            //{
            //    System.Windows.Forms.MessageBox.Show(e.ToString());
            //}
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Dispose(bool disposing)
        {
            controler.Dispose();
            base.Dispose(disposing);
        }

        bool pressedA = false;
        bool pressedB = false;
        int frames = 0;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            if (controler != null)
            {
                controler.Update();
                if (controler.ActiveJointStates > 0)
                {
                    JointCollection joints = controler.JointStates[0];
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
                    {
                        if (!pressedA)
                        {
                            pressedA = true;
                            root = makePose(joints);
                            hierarchy = new Hierarchy(root);
                            motion = new Motion(root, 100, 0.0166666666f);
                            BVHFile bvh = new BVHFile(hierarchy, motion);

                            writeToFile("data.bvh",bvh.ToString());
                            debugStr["PoseCapturing"] = "OK!";
                        }
                    }
                    else
                    {
                        pressedA = false;
                    }

                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.B))
                    {
                        if (!pressedB)
                        {
                            pressedB = true;
                            if (!capturing)
                            {
                                //全てのジョイントがトラッキングされていればキャプチャ開始
                                if (joints.Count == 20)
                                {
                                    capturing = true;
                                    //キャプチャの開始処理
                                    motionData.Clear();
                                    setBindPose();
                                    root = makePose(joints);
                                    hierarchy = new Hierarchy(root);
                                    debugStr["MotionCapture"] = "Start";
                                }
                            }
                            else
                            {
                                try
                                {
                                    //キャプチャ終了
                                    capturing = false;
                                    //キャプチャを終了する処理
                                    //BVHファイルの出力処理
                                    motion = new Motion(root, 0.01666666f, motionData.ToArray());
                                    string debug = "";
                                    foreach (float[] data in motionData)
                                    {
                                        foreach (float num in data)
                                        {
                                            debug += num + " ";
                                        }
                                        debug += "\n";
                                    }
                                    writeToFile("debug.txt", debug);
                                    BVHFile bvh = new BVHFile(hierarchy, motion);
                                    writeToFile("motion.bvh",bvh.ToString());
                                    debugStr["MotionCapture"] = "End";

                                }
                                catch (Exception e)
                                {
                                    System.Windows.Forms.MessageBox.Show(e.ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        pressedB = false;
                    }
                    if (capturing)
                    {
                        //キャプチャ中の処理
                        //そのフレームでのジョイントの位置を保存
                        if (joints.Count == 20)
                        {
                            //生データ
                            Vector3[] positions = new Vector3[]{
                                joints[JointType.HipCenter].Position.ToVector3R(),
                                joints[JointType.Spine].Position.ToVector3R() - joints[JointType.HipCenter].Position.ToVector3R(),
                                joints[JointType.ShoulderCenter].Position.ToVector3R() - joints[JointType.Spine].Position.ToVector3R(),
                                joints[JointType.Head].Position.ToVector3R() - joints[JointType.ShoulderCenter].Position.ToVector3R(),
                                joints[JointType.ShoulderLeft].Position.ToVector3R() - joints[JointType.ShoulderCenter].Position.ToVector3R(),
                                joints[JointType.ElbowLeft].Position.ToVector3R() - joints[JointType.ShoulderLeft].Position.ToVector3R(),
                                joints[JointType.WristLeft].Position.ToVector3R() - joints[JointType.ElbowLeft].Position.ToVector3R(),
                                joints[JointType.HandLeft].Position.ToVector3R() - joints[JointType.WristLeft].Position.ToVector3R(),
                                joints[JointType.ShoulderRight].Position.ToVector3R() - joints[JointType.ShoulderCenter].Position.ToVector3R(),
                                joints[JointType.ElbowRight].Position.ToVector3R() - joints[JointType.ShoulderRight].Position.ToVector3R(),
                                joints[JointType.WristRight].Position.ToVector3R() - joints[JointType.ElbowRight].Position.ToVector3R(),
                                joints[JointType.HandRight].Position.ToVector3R() - joints[JointType.WristRight].Position.ToVector3R(),
                                joints[JointType.HipLeft].Position.ToVector3R() - joints[JointType.HipCenter].Position.ToVector3R(),
                                joints[JointType.KneeLeft].Position.ToVector3R() - joints[JointType.HipLeft].Position.ToVector3R(),
                                joints[JointType.AnkleLeft].Position.ToVector3R() - joints[JointType.KneeLeft].Position.ToVector3R(),
                                joints[JointType.FootLeft].Position.ToVector3R() - joints[JointType.AnkleLeft].Position.ToVector3R(),
                                joints[JointType.HipRight].Position.ToVector3R() - joints[JointType.HipCenter].Position.ToVector3R(),
                                joints[JointType.KneeRight].Position.ToVector3R() - joints[JointType.HipRight].Position.ToVector3R(),
                                joints[JointType.AnkleRight].Position.ToVector3R() - joints[JointType.KneeRight].Position.ToVector3R(),
                                joints[JointType.FootRight].Position.ToVector3R() - joints[JointType.AnkleRight].Position.ToVector3R()
                            };

                            List<float> data = new List<float>();
                            //int i = 0;
                            foreach (Vector3 v in positions)
                            {
                                data.Add(v.X);
                                data.Add(v.Y);
                                data.Add(v.Z);
                                //debugStr["data" + i++] = "" + v.ToString();
                            }
                            motionData.Add(data.ToArray());
                            frames++;
                        }
                    }
                    else frames = 0;
                
                }
                Game1.debugStr["Capturing"] = "" + capturing;
                Game1.debugStr["ValidFrames"] = "" + frames;
            }
            base.Update(gameTime);
        }

        void writeToFile(string fileName,string str)
        {
            FileInfo fileInfo = new FileInfo(path + fileName);
            writer = new StreamWriter(fileInfo.Open(FileMode.OpenOrCreate));
            writer.WriteLine(str);
            writer.Close();
        }
        void setBindPose()
        {
            //1フレーム目にバインドポーズをセット(スキニングしやすいように)

        }
        Root makePose(JointCollection joints)
        {
            root = new Root("HipCenter", Vector3.Zero,
                        new BVHJoint("Spine", joints[JointType.Spine].Position.ToVector3R() - joints[JointType.HipCenter].Position.ToVector3R(),
                            new BVHJoint("ShoulderCenter", joints[JointType.ShoulderCenter].Position.ToVector3R() - joints[JointType.Spine].Position.ToVector3R(),
                                new BVHJoint("Head", joints[JointType.Head].Position.ToVector3R() - joints[JointType.ShoulderCenter].Position.ToVector3R()),
                                new BVHJoint("ShoulderLeft", joints[JointType.ShoulderLeft].Position.ToVector3R() - joints[JointType.ShoulderCenter].Position.ToVector3R(),
                                    new BVHJoint("ElbowLeft", joints[JointType.ElbowLeft].Position.ToVector3R() - joints[JointType.ShoulderLeft].Position.ToVector3R(),
                                        new BVHJoint("WristLeft", joints[JointType.WristLeft].Position.ToVector3R() - joints[JointType.ElbowLeft].Position.ToVector3R(),
                                            new BVHJoint("HandLeft", joints[JointType.HandLeft].Position.ToVector3R() - joints[JointType.WristLeft].Position.ToVector3R())
                                        )
                                    )
                                ),
                                new BVHJoint("ShoulderRight", joints[JointType.ShoulderRight].Position.ToVector3R() - joints[JointType.ShoulderCenter].Position.ToVector3R(),
                                    new BVHJoint("ElbowRight", joints[JointType.ElbowRight].Position.ToVector3R() - joints[JointType.ShoulderRight].Position.ToVector3R(),
                                        new BVHJoint("WristRight", joints[JointType.WristRight].Position.ToVector3R() - joints[JointType.ElbowRight].Position.ToVector3R(),
                                            new BVHJoint("HandRight", joints[JointType.HandRight].Position.ToVector3R() - joints[JointType.WristRight].Position.ToVector3R())
                                        )
                                    )
                                )
                            )
                        ),
                        new BVHJoint("HipLeft", joints[JointType.HipLeft].Position.ToVector3R() - joints[JointType.HipCenter].Position.ToVector3R(),
                            new BVHJoint("KneeLeft", joints[JointType.KneeLeft].Position.ToVector3R() - joints[JointType.HipLeft].Position.ToVector3R(),
                                new BVHJoint("AnkleLeft", joints[JointType.AnkleLeft].Position.ToVector3R() - joints[JointType.KneeLeft].Position.ToVector3R(),
                                    new BVHJoint("FootLeft", joints[JointType.FootLeft].Position.ToVector3R() - joints[JointType.AnkleLeft].Position.ToVector3R())
                                )
                            )
                        ),
                        new BVHJoint("HipRight", joints[JointType.HipRight].Position.ToVector3R() - joints[JointType.HipCenter].Position.ToVector3R(),
                            new BVHJoint("KneeRight", joints[JointType.KneeRight].Position.ToVector3R() - joints[JointType.HipRight].Position.ToVector3R(),
                                new BVHJoint("AnkleRight", joints[JointType.AnkleRight].Position.ToVector3R() - joints[JointType.KneeRight].Position.ToVector3R(),
                                    new BVHJoint("FootRight", joints[JointType.FootRight].Position.ToVector3R() - joints[JointType.AnkleRight].Position.ToVector3R())
                                )
                            )
                        )
                    );
            return root;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.Clear(Color.Black);

            if (controler != null)
            {
                drawer.AddTexture(controler.ColorImage, Vector2.Zero);

                try
                {
                    if (controler.JointPositionColor != null)
                    {
                        foreach (ColorImagePoint p in controler.JointPositionColor.Values)
                        {
                            Vector2 drawPoint = new Vector2(p.X - point.Width / 2, p.Y - point.Height / 2);
                            drawer.AddTexture(point, drawPoint);
                        }
                    }
                }
                catch (Exception e)
                {
                    Window.Title = e.Message;
                }
            }

            int i = 0;
            foreach (KeyValuePair<string,string> pair in debugStr)
            {
                drawer.AddString(pair.Key + "..." + pair.Value, new Vector2(0, i));
                i += 15;
            }
            drawer.Draw();

            base.Draw(gameTime);
        }
    }
}
