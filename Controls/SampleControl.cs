using Microsoft.Xna.Framework;
using MonoGame.Forms.Controls;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Editor1.scr;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Threading;

namespace MonoGame.Forms.DX.Controls
{
    public class SampleControl : MonoGameControl
    {

        private readonly List<TextureGame> gameTx = new List<TextureGame>();
        private readonly BlockerClass RedBlock = new BlockerClass();
        private readonly List<DrawLine> Lines = new List<DrawLine>();
        private Texture2D BaseTxLine;
        private double totalElapsed;
        private bool PlayerKeyPress = false;
        private PlayerClass PlayerA;

        Thread LineReBulder;

        public List<AnimatorClass> Animator = new List<AnimatorClass>();
        public bool RedBlock_Visible = false;
        public delegate void VisibleUI(bool vis);
        public event VisibleUI VisibleUI_Form1;

        protected override void Initialize()
        {
            base.Initialize();

            // Сreate platforms
            var txx = new TextureGame(@"tiles\Assets", Editor, 50, 149) { Width = 16, Height = 16 };
            txx.FrameSize = new Rectangle(32, 0, 16, 16);
            for (int i = 0; i < 20; i++)
            {
                gameTx.Add(txx.Clone() as TextureGame);
                gameTx.LastOrDefault().X = 50 + (16 * i);
                RedBlock.AddRedBlock(gameTx.LastOrDefault());
            }
            for (int i = 0; i < 20; i++)
            {
                gameTx.Add(txx.Clone() as TextureGame);
                gameTx.LastOrDefault().Y = 200;
                gameTx.LastOrDefault().X = -10 + (16 * i);
                RedBlock.AddRedBlock(gameTx.LastOrDefault());
            }

            // Create block
            txx = new TextureGame(@"tiles\Assets", Editor, 300, 125) { Width = 16, Height = 16 };
            txx.FrameSize = new Rectangle(32, 32, 16, 16);  // Сrop one frame so that other frames are not visible
            gameTx.Add(txx);
            RedBlock.AddRedBlock(txx);

            // Create player
            gameTx.Add(new TextureGame(@"player\player crouch-walk 48x48", Editor, 100, 90));
            gameTx.LastOrDefault().RedBlockSize(20, 20);         // To have fewer collisions, we reduced the square
            gameTx.LastOrDefault().Origin = new Vector2(-5, -10); // Since the texture is slightly higher, makes it lower while the square of the texture does not change.
            RedBlock.AddRedBlock(gameTx.LastOrDefault());

            // Here is the animation for the player
            var anim = new AnimatorClass(gameTx.LastOrDefault());
            PlayerA = new PlayerClass(anim, RedBlock, "PlayerA");
            anim.AddAnimation("move", AnimatorClass.CreateFrames(10, 48, 48, 0.1, "player/player crouch-walk 48x48"));
            anim.AddAnimation("role", AnimatorClass.CreateFrames(7, 48, 48, 0.1, "player/Player Roll 48x48"));
            anim.AddAnimation("idle", new AnimatorClass.FrameStruct[] {
                new AnimatorClass.FrameStruct() {
                  Cut = new Rectangle (0,0,48,48),
                  NameTextura = "player/player crouch-walk 48x48",
                  Second =1} });
            anim.Pause = false;
            Animator.Add(anim);

            // Draws lines regardless of the game engine
            LineReBulder = new Thread(() =>
            {
                while (true)
                {
                    LineBuilder();
                    Thread.Sleep(600);
                }
            });
            LineReBulder.Start();

            // Avoids filling memory when creating textures every time
            BaseTxLine = new Texture2D(Editor.graphics, 1, 1);
            BaseTxLine.SetData<Color>(new Color[] { Color.Red });
        }


        protected override void Update(GameTime gameTime)
        {
            PlayerA.Update(gameTime);                   //Logic time for the player
            Animator.ForEach(x => x.Update(gameTime));  //Updates the animation

            // Player key control
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                PlayerKeyPress = true;
                PlayerA.LookAway = PlayerClass.LookAwayEnum.LookRight;
                PlayerA.Move();
                VisibleUI_Form1(false);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                PlayerKeyPress = true;
                PlayerA.LookAway = PlayerClass.LookAwayEnum.LookLeft;
                PlayerA.Move();
                VisibleUI_Form1(false);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Up) )
            {
                PlayerKeyPress = true;
                PlayerA.Role();
                VisibleUI_Form1(false);
            }
            if (PlayerKeyPress == false)
            {
                PlayerA.Stand();
                VisibleUI_Form1(true);
            }

            // Delay in keystrokes
            totalElapsed += gameTime.ElapsedGameTime.TotalSeconds;
            if (totalElapsed > 0.2)
            {
                totalElapsed = 0;
                PlayerKeyPress = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw()
        {
            base.Draw();
            Editor.spriteBatch.Begin();

            gameTx.ForEach(x => x.Draw(Editor));
            if (RedBlock_Visible) Lines.ForEach(x => x.Draw(Editor));

            Editor.spriteBatch.End();
        }

        void LineBuilder()
        {
            Lines.Clear();
            Lines.TrimExcess(); // Don't forget to save memory
            foreach (var item in RedBlock.RedBlock_Rectangle)
            {
                Lines.Add(new DrawLine(item.X, item.Y, item.X + item.Width, item.Y, Color.Red, BaseTxLine));
                Lines.Add(new DrawLine(item.X, item.Y, item.X, item.Y + item.Height, Color.Red, BaseTxLine));
                Lines.Add(new DrawLine(item.X, item.Y + item.Height, item.X + item.Width, item.Y + item.Height, Color.Red, BaseTxLine));
                Lines.Add(new DrawLine(item.X + item.Width, item.Y, item.X + item.Width, item.Y + item.Height, Color.Red, BaseTxLine));
            }
        }

        //******************************************************************

        private class PlayerClass
        {

            private double totalElapsed;
            private bool roleUp = false;
            private readonly BlockerClass redBlock;

            public TextureGame TexturePl { get => AnimPl.TexturaControl; }
            public readonly AnimatorClass AnimPl;
            public string Name { get => TexturePl.Label; }
            public LookAwayEnum LookAway = LookAwayEnum.LookRight;

            public void Move()
            {
                if (roleUp) return;
                AnimPl.SelectedAnimation = "move";
                LogikMove(1);
            }

            public void Role()
            {
                if (roleUp) return;
                roleUp = true;
                AnimPl.SelectedAnimation = "role";
                AnimPl.Pause = true;
            }

            public void Stand()
            {
                if (roleUp == false) AnimPl.SelectedAnimation = "idle";
            }

            public void Update(GameTime gameTime)
            {
                totalElapsed += gameTime.ElapsedGameTime.TotalSeconds;
                if (totalElapsed > 0.1 && roleUp == true)
                {                   
                    totalElapsed = 0;
                    if (!AnimPl.NextFrame())
                    {
                        roleUp = false;                        
                    }
                    else
                    {
                        LogikMove(8);
                    }
                }
                //gravity
                redBlock.GravityFloor(TexturePl, 3);
            }


            public PlayerClass(AnimatorClass a, BlockerClass rb, string name)
            {
                AnimPl = a;
                redBlock = rb;
                TexturePl.Label = name;
            }

            public enum LookAwayEnum
            {
                LookLeft,
                LookRight
            }

            private void LogikMove(int step)
            {
                var v1 = TexturePl.Locate;
                if (LookAway == LookAwayEnum.LookRight)
                {
                    TexturePl.Effect = SpriteEffects.None;
                    TexturePl.X += step;
                }
                else
                {
                    TexturePl.Effect = SpriteEffects.FlipHorizontally;
                    TexturePl.X -= step;
                }
                if (redBlock.FindCollisions(TexturePl)) TexturePl.Locate = v1;
            }
        }
    }
}


