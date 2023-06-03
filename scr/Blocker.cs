using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Editor1.scr
{
    public class BlockerClass
    {
        private readonly List<TextureGame> RedBlock = new List<TextureGame>();

        public List<Rectangle> RedBlock_Rectangle { get => (from TV in RedBlock select TV.RedBlock).ToList(); }

        public bool GravityFloor(TextureGame pl, int step)
        {
            Rectangle rec = pl.TxRectangle;
            pl.Y += step;
            foreach (TextureGame item in RedBlock)
            {
                if (pl.RedBlock.Bottom > item.RedBlock.Top)
                {
                    if (pl.RedBlock == item.RedBlock) break;
                    if (pl.RedBlock.Intersects(item.RedBlock))
                    {
                        pl.Y = rec.Y;
                        return true;
                    }
                }
            }
            return false;
        }
        public void AddRedBlock(TextureGame texture)
        {
            if (RedBlock.FindIndex(x => x.RedBlock == texture.RedBlock) != -1) return;
            RedBlock.Add(texture);
        }
        public void DelRedBlock(TextureGame texture)
        {
            if (RedBlock.FindIndex(x => x.RedBlock == texture.RedBlock) != -1) return;
            RedBlock.Remove(texture);
        }
        public bool FindCollisions(TextureGame texture)
        {
            foreach (var item in RedBlock)
            {
                if (texture.RedBlock == item.RedBlock) break;
                if (item.RedBlock.Intersects(texture.RedBlock)) return true;
            }
            return false;
        }
        public bool FindCollisions(Rectangle rec)
        {
            foreach (var item in RedBlock)
            {
                if (rec == item.RedBlock) break;
                if (item.RedBlock.Intersects(rec))  return true; 
            }
            return false;
        }

    }
}
