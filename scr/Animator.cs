using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Editor1.scr
{
    public class AnimatorClass
    {
        private double totalElapsed;
        private int isFrame = 0;
        private bool pause = true;
        /// <summary>
        /// (!) the dictionary supports some types of serialization, so it is not used here
        /// </summary>
        private List<String> animationName = new List<string>();            // Stores animation names
        private List<FrameStruct[]> lsFrames = new List<FrameStruct[]>();   // Animation frames here       
        private int indexAnimatiom = 0;                                     // Current animation

        public FrameStruct[] Frames;
        public readonly TextureGame TexturaControl;  
        /// <summary>
        /// True - endless animation, false - until the last frame
        /// </summary>
        public bool Loop = true;

        /// <summary>
        /// Select animation by name
        /// </summary>
        public string SelectedAnimation
        {
            get => animationName[indexAnimatiom];
            set
            {
                var i = animationName.FindIndex(x => x.ToLower() == value.ToLower());
                if (i == indexAnimatiom) return;
                else { indexAnimatiom = i; }
                if (indexAnimatiom == -1) indexAnimatiom = 0;
                Frames = lsFrames[indexAnimatiom];
                isFrame = 0;
                SelFrame();
            }
        }
        /// <summary>
        /// true - pause animation
        /// </summary>
        public bool Pause
        {
            get
            {
                if (Frames.Length == 0) return true;
                return pause;
            }
            set
            {
                pause = value;
                if (value == false)
                {
                    if (Frames.Length != 0)
                    {
                        TexturaControl.FrameSize = Frames[0].Cut;
                    }
                }
            }
        }      

        public delegate void UpdateAnim();
        /// <summary>
        /// When you change the list in the animation warns about it. If you use an animation list to update it 
        /// </summary>
        public event UpdateAnim AnimationСhanges;
        /// <summary>
        /// The animation has finished. In that case, it works if non-infinite animation is enabled
        /// </summary>
        public event UpdateAnim AnimationEnd;

        public AnimatorClass(TextureGame texture)
        {
            TexturaControl = texture;
            AnimationСhanges += FndNull;
            AnimationEnd += FndNull;
        }

        /// <summary>
        /// Creates frames from left to right
        /// </summary>
        /// <param name="allFrame">Number of frames in texture</param>
        /// <param name="wFrame">Width frame</param>
        /// <param name="hFrame">Height frame</param>
        /// <param name="second">Time between frames</param>
        /// <param name="nameTx">texture name. Changes the texture</param>
        /// <returns>Automatically generated frames</returns>
        public static FrameStruct[] CreateFrames(int allFrame, int wFrame, int hFrame, double second, string nameTx = "")
        {
            List<FrameStruct> ls = new List<FrameStruct>();
            for (int i = 0; i < allFrame; i++)
            {
                ls.Add(new FrameStruct(new Rectangle(i * wFrame, 0, wFrame, hFrame), second, nameTx));
            }
            return ls.ToArray();
        }

        /// <summary>
        /// Next frame
        /// </summary>
        /// <returns>true-there are more shots; false - nope</returns>
        public bool NextFrame()
        {
            isFrame += 1;
            if (Frames.Length == isFrame)
            {
                isFrame = 0;
                return false;
            }
            else
            {
                SelFrame();
                return true;
            }
        }
        public void Update(GameTime gametime)
        {
            if (Pause) return;
            totalElapsed += gametime.ElapsedGameTime.TotalSeconds;

            if (totalElapsed > Frames[isFrame].Second)
            {
                totalElapsed = 0;
                SelFrame(); //Frame selection

                isFrame += 1;
                if (isFrame == Frames.Length)
                {
                    isFrame = 0;
                    if (Loop == false) { pause = true; AnimationEnd(); }
                }
            }
        }
        public void RemoveAnimation(int index)
        {
            RemoveAnim(index);
        }
        public void RemoveAnimation(string name)
        {
            int index = animationName.FindIndex(x => x.ToLower() == name.ToLower());
            if (index == -1) return;
            RemoveAnim(index);
        }
        public void AddAnimation(string name, FrameStruct[] frameStructs)
        {
            if (animationName.FindIndex(x => x.ToLower() == name.ToLower()) != -1) throw new ArgumentException("This animation name already exists.");
            animationName.Add(name);
            lsFrames.Add(frameStructs);
            if (animationName.Count == 1) { Frames = frameStructs; }        // Selects this animation as the first existing one
            AnimationСhanges();
        }      
        public bool Equals(TextureGame tx) => TexturaControl == tx;
        /// <summary>
        /// Frame selection
        /// </summary>
        private void SelFrame()
        {
            TexturaControl.FrameSize = Frames[isFrame].Cut;
            TexturaControl.SizeRec(Frames[isFrame].Cut.Width, Frames[isFrame].Cut.Height);

            if (Frames[isFrame].NameTextura != TexturaControl.TextureName)
            { TexturaControl.TextureName = Frames[isFrame].NameTextura; }
        }
        private void RemoveAnim(int index)
        {
            if (indexAnimatiom == index) throw new ArgumentException("This animation is currently running");
            lsFrames.RemoveAt(index);
            animationName.RemoveAt(index);
            AnimationСhanges();
        }

        private void FndNull() { }

        /// <summary>
        /// Stop motion animation here
        /// </summary>
        public struct FrameStruct
        {
            public Rectangle Cut;
            public double Second;
            public string NameTextura;

            public FrameStruct(Rectangle rectangleCut, double second, string nameTextura2D = "")
            {
                Cut = rectangleCut;
                Second = second;
                NameTextura = nameTextura2D;
            }          

            public override string ToString() => $"Frame: {Cut.X }-{Cut.Y}; {Cut.Width},{Cut.Height}; Sec: {Second}; Textura2D: {NameTextura};";
        }
    }
}
