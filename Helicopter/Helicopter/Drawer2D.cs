using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MotionCapture
{
    class Drawer2D
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        List<Tuple<Texture2D, Rectangle>> textureList;
        List<Tuple<string, Vector2>> stringList;
        GraphicsDevice graphicsDevice;
        public Drawer2D(GraphicsDevice graphicsDevice,SpriteFont spriteFont)
        {
            this.graphicsDevice = graphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            this.spriteFont = spriteFont;
            textureList = new List<Tuple<Texture2D, Rectangle>>();
            stringList = new List<Tuple<string, Vector2>>();
        }
        public Drawer2D(GraphicsDevice graphicsDevice)
            : this(graphicsDevice, null){}


        public void AddTexture(Texture2D texture, Vector2 position, float scaleX, float scaleY)
        {
            textureList.Add(new Tuple<Texture2D, Rectangle>(texture, new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width * scaleX), (int)(texture.Height * scaleY))));
        }
        public void AddTexture(Texture2D texture, Vector2 position)
        {
            AddTexture(texture, position, 1,1);
        }
        public void AddTexture(Texture2D texture, Vector2 position, float scale)
        {
            AddTexture(texture, position, scale, scale);
        }


        public void AddString(string str, Vector2 position)
        {
            stringList.Add(new Tuple<string,Vector2>(str,position));
        }
        public void Draw()
        {
            spriteBatch.Begin();
            foreach (Tuple<Texture2D, Rectangle> pair in textureList)
            {
                spriteBatch.Draw(pair.Item1, pair.Item2, Color.White);
            }
            foreach (Tuple<string, Vector2> pair in stringList)
            {
                if (spriteFont == null)
                    throw new Exception("文字列を描画する場合は、コンストラクタでフォントを指定してください");
                spriteBatch.DrawString(spriteFont, pair.Item1, pair.Item2, Color.Red);
            }
            spriteBatch.End();

            //// Zバッファを有効にする?
            graphicsDevice.DepthStencilState = DepthStencilState.Default;


            textureList.Clear();
            stringList.Clear();
        }
    }
}
