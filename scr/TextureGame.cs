
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Forms.Services;


namespace Editor1.scr
{
    public class TextureGame
    {
        private Texture2D _Texture2D;
        private Rectangle recTexture;
        private Rectangle frameSize;
        private readonly MonoGameService gameService;
        private string textureName;
        private int redB_Width;
        private int redB_Height;

        public TypeTexturaEnum TypeTextura = TypeTexturaEnum.none;
        public Color ColorTexture = Color.White;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effect = SpriteEffects.None;
        public float Layer;
        public bool Visible = true;

        public string TextureName
        {
            get => textureName;
            set
            {
                if (_Texture2D == null & gameService == null) throw new System.ArgumentException("There is no object reference. MonoGameService. (xml С# serialization makes links empty)");
                _Texture2D = gameService.Content.Load<Texture2D>(value);
                textureName = value;
            }
        }
        public Vector2 Locate
        {
            get { return new Vector2(recTexture.X, recTexture.Y); }
            set { recTexture = new Rectangle((int)value.X, (int)value.Y, Width, Height); }
        }
        public int Width
        {
            get => recTexture.Width;
            set => recTexture = new Rectangle(recTexture.X, recTexture.Y, value, recTexture.Height);
        }
        public int Height
        {
            get => recTexture.Height;
            set => recTexture = new Rectangle(recTexture.X, recTexture.Y, recTexture.Width, value);
        }
        /// <summary>
        /// Optional
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Since the texture can be larger, it can reduce the chances of collisions
        /// </summary>
        public Rectangle RedBlock
        {
            get
            {
                int x = recTexture.X - (recTexture.X - redB_Width);
                int y = recTexture.Y - (recTexture.Y - redB_Height);
                return new Rectangle(recTexture.X + x, recTexture.Y + y, frameSize.Width - redB_Width, frameSize.Height - redB_Height);
            }
        }
        /// <summary>
        /// Frame size on texture
        /// </summary>
        public Rectangle FrameSize
        {
            get => frameSize;
            set => frameSize = value;
        }
        public int X { get => recTexture.X; set => recTexture = new Rectangle(value, recTexture.Y, recTexture.Width, recTexture.Height); }
        public int Y { get => recTexture.Y; set => recTexture = new Rectangle(recTexture.X, value, recTexture.Width, recTexture.Height); }
        public Rectangle TxRectangle { get => recTexture; }

        public TextureGame(string texturaName, MonoGameService ms)
        {
            gameService = ms;
            TextureName = texturaName;
            recTexture = new Rectangle(0, 0, _Texture2D.Width, _Texture2D.Height);
            frameSize = new Rectangle(0, 0, Width, Height);
            Layer = 0;
            Rotation = 0;
            Origin = Vector2.Zero;
            Scale = Vector2.Zero;
        }
        public TextureGame(string texturaName, MonoGameService ms, int X, int Y)
        {
            gameService = ms;
            TextureName = texturaName;
            recTexture = new Rectangle(X, Y, _Texture2D.Width, _Texture2D.Height);
            frameSize = new Rectangle(0, 0, Width, Height);
            Layer = 0;
            Rotation = 0;
            Origin = Vector2.Zero;
            Scale = Vector2.Zero;
        }

        /// <summary>
        /// Changes the size of the texture
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void SizeRec(int Width, int Height) => recTexture = new Rectangle(recTexture.X, recTexture.Y, Width, Height);
        /// <summary>
        /// Changes the size of the red block for collision
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void RedBlockSize(int Width, int Height)
        {
            redB_Width = Width;
            redB_Height = Height;
        }
        public void Draw(MonoGameService ms)
        {
            if (Visible) ms.spriteBatch.Draw(_Texture2D, recTexture, frameSize, ColorTexture, Rotation, Origin, Effect, Layer);
        }
        public object Clone() => this.MemberwiseClone();

        public enum TypeTexturaEnum
        {
            none,
            bg,
            players
        }
    }
}
