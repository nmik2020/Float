using System;
using System.Text;
using FarseerPhysics.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Samples.DrawingSystem;
using Microsoft.Xna.Framework.Content;


namespace FarseerPhysics.Samples
{
   public class GameOver : PhysicsGameScreen
   {
        private Texture2D texture;
        
        public override void LoadContent()
        {
            base.LoadContent();
            texture = ScreenManager.Content.Load<Texture2D>("Materials/game_over_screen");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleKeyboard();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            ScreenManager.SpriteBatch.Draw(texture, new Rectangle(-800, -500, 1600, 900), Color.Green);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public void HandleKeyboard()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.R))
            {
                ScreenManager.RemoveScreen(this);
                switch (Globals.currentLevel)
                {
                    case 1:
                        Level1 game = new Level1();
                        ScreenManager.AddScreen(game);
                        break;
                    case 2:
                        Level2 game2 = new Level2();
                        ScreenManager.AddScreen(game2);
                        break;
                    case 3:
                        Level3 game3 = new Level3();
                        ScreenManager.AddScreen(game3);
                        break;
                    case 4:
                        Level4 game4 = new Level4();
                        ScreenManager.AddScreen(game4);
                        break;
                    case 5:
                        Level5 game5 = new Level5();
                        ScreenManager.AddScreen(game5);
                        break;
                    case 6:
                        Level6 game6 = new Level6();
                        ScreenManager.AddScreen(game6);
                        break;
                }
                
            }

        }
    }
}
