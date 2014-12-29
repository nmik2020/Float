using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerPhysics.Samples.ScreenSystem
{
    public class SpriteFonts
    {
        public SpriteFont DetailsFont;
        public SpriteFont FrameRateCounterFont;
        public SpriteFont MenuSpriteFont;

        public SpriteFonts(ContentManager contentManager)
        {
            MenuSpriteFont = contentManager.Load<SpriteFont>("Font/menuFont");
            FrameRateCounterFont = contentManager.Load<SpriteFont>("Font/frameRateCounterFont");
            DetailsFont = contentManager.Load<SpriteFont>("Font/detailsFont");
        }
    }
}