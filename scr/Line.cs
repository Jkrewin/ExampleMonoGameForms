using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Forms.Services;

namespace Editor1.scr
{
    /// <summary>
    /// Draws lines
    /// </summary>
    public class DrawLine
    {
        public bool Visible = true;

        private Color ColorLine;
        private Vector2 Start;
        private Vector2 Ends;
        private readonly Single angle;
        public Texture2D BaseTx;
        private readonly int Thickness = 2;

        public DrawLine (int x1, int y1, int x2, int y2, Color color, Texture2D texture )
        {
            BaseTx = texture;            
            Start = new Vector2(x1, y1);
            Ends = new Vector2(x2, y2);
            Vector2 edge = Ends - Start;
            angle = (float)Math.Atan2(edge.Y, edge.X);
            ColorLine = color;          
        }

        public void Draw(MonoGameService ms)
        {
            if (Visible == false) return;           
            float i = (Ends - Start).Length();
            Rectangle rec = new Rectangle((int)Start.X, (int)Start.Y, (int)i, Thickness);
            ms.spriteBatch.Draw(BaseTx, rec, new Rectangle(), ColorLine, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
