using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Samples.ScreenSystem;
using FarseerPhysics.Samples.DrawingSystem;
using FarseerModel;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace FarseerPhysics.Samples
{
    public class Level1old : PhysicsGameScreen, IDemoScreen
    {


        private Border _border;
        private WinSprite _winSprite;

        private Sprite _obstacle;
        private Body[] _obstacles = new Body[5];
        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition = new Vector2(0, 0);
        //bool collidedWithBorder;
        KeyboardState state;
        private Spider[] _spider;
        private Vector2 _currentLeftPosition;
        private Vector2 _currentRightPosition;
        const float BOUNDARY_OBSTACLE_X = 18.5f;
        const float BOUNDARY_OBSTACLE_Y = 11.6f;

        public KeyboardState oldState;
        public GamePadState oldPadState;
        AssetCreator creator;
        GameOver gameOver;
        Level3 level3;
        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Dynamic Angle Joints";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo demonstrates the use of revolute joints combined");
            sb.AppendLine("with angle joints that have a dynamic target angle.");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Rotate agent: left and right triggers");
            sb.AppendLine("  - Move agent: right thumbstick");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Rotate agent: left and right arrows");
            sb.AppendLine("  - Move agent: A,S,D,W");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

#if !XBOX360
        const string Text = "Press A or D to rotate the ball\n" +
                            "Press Space to jump\n" +
                            "Use arrow keys to move the camera";
#else
                const string Text = "Use left stick to move\n" +
                                    "Use right stick to move camera\n" +
                                    "Press A to jump\n";
#endif


        private void LoadObstacles()
        {
            for (int i = 0; i < 3; ++i)
            {
                if(i !=2)
                _obstacles[i] = BodyFactory.CreateRectangle(World, 20f, 7f, 1f);
                else
                    _obstacles[i] = BodyFactory.CreateRectangle(World, 25f, 7f, 1f);
                _obstacles[i].IsStatic = true;
                _obstacles[i].Restitution = 0.2f;
                _obstacles[i].Friction = 0.2f;
            }
            _obstacles[0].Position = new Vector2(-BOUNDARY_OBSTACLE_X, BOUNDARY_OBSTACLE_Y);
            _obstacles[1].Position = new Vector2(BOUNDARY_OBSTACLE_X, BOUNDARY_OBSTACLE_Y);
            _obstacles[2].Position = new Vector2(0f, -12f);
            
            // create sprite based on body
            _obstacle = new Sprite(ScreenManager.Assets.TextureFromShape(_obstacles[0].FixtureList[0].Shape,
                                                                         MaterialType.Dots,
                                                                         Color.DarkViolet, 0.8f));

        }

        public override void LoadContent()
        {
            // Initialize camera controls
            base.LoadContent();
            LoadObstacles();
            Globals.currentLevel = 1;
            creator = ScreenManager.Assets;
            gameOver = new GameOver();

            //collidedWithBorder = false;
             LoadInchWorm();
             _currentLeftPosition = _spider[0].getLeftPosition();
             _currentRightPosition = _spider[0].getRightPosition();

             oldState = Keyboard.GetState();
             GamePadState currentState = GamePad.GetState(PlayerIndex.One);
             if (currentState.IsConnected)
             {
                 oldPadState = currentState;
             }
             _spider[0].setGravity(World.Gravity.Y);

            //Trigger collision event
             _border._anchor.OnCollision += body_OnCollision;
             _winSprite._anchor.OnCollision += wonLevel;


        }

        public void LoadInchWorm()
        {
            World.Gravity = new Vector2(0f, 20f);
            _border = new Border(World, ScreenManager, Camera);
            _winSprite = new WinSprite(World, ScreenManager, Camera);

            _spider = new Spider[1];
            for (int i = 0; i < _spider.Length; i++)
            {
                //default position
                //_spider[i] = new Spider(World, ScreenManager, new Vector2(10f, 8f - (i + 1) * 2f));
                _spider[i] = new Spider(World, ScreenManager, new Vector2(-22f, 8f - (i + 1) * 2f));

            }
        }
        


        private void HandleKeyboard()
        {
            KeyboardState newState = Keyboard.GetState();

            state = Keyboard.GetState();
            _spider[0].setGravity(World.Gravity.Y);
            for (int i = 0; i < _spider.Length; i++)
            {
                _spider[i].handleKeyboard(state);
            }

            if (newState.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space))
            {
                World.Gravity = new Vector2(0f, -World.Gravity.Y);
            }
            oldState = newState;
            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition, 0f)) ;

        }

        private void HandleGamePad(GamePadState currentState)
        {
            GamePadState newPadState = currentState;

            _spider[0].HandleGamePad(currentState);
            _spider[0].setGravity(World.Gravity.Y);

            if (newPadState.IsButtonDown(Buttons.A) && !oldPadState.IsButtonDown(Buttons.A))
            {
                World.Gravity = new Vector2(0f, -World.Gravity.Y);
            }
            oldPadState = newPadState;
        }

        public float getGravity()
        {
            return World.Gravity.Y;
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected)
            {
                HandleGamePad(currentState);
            }
            else
            {

                HandleKeyboard();
            }

            if (IsActive)
            {
                for (int i = 0; i < _spider.Length; i++)
                {
                    _spider[i].Update(gameTime);
                }
            }

            Console.WriteLine("Position of Spider : {0}", _spider[0].getRightPosition());
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        //For GameOver
        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            Console.WriteLine("Inside on collision ");

            _border._anchor.OnCollision -= body_OnCollision;
            //_winSprite._anchor.OnCollision -= body_OnCollision;


            //if (!collidedWithBorder)
            //{
                ScreenManager.RemoveScreen(this);
                ScreenManager.AddScreen(gameOver);
            //}
            //collidedWithBorder = true;
            return false;
        }

        bool wonLevel(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            Console.WriteLine("Inside on collision ");

            _winSprite._anchor.OnCollision -= wonLevel;
            level3 = new Level3();

                ScreenManager.RemoveScreen(this);
                ScreenManager.AddScreen(level3);
            
            return false;
        }
        public void changeCamera(Vector2 position)
        {
            Vector2 camMove = Vector2.Zero;
            camMove.X -= position.X * 0.01f;
            Camera.MoveCamera(camMove);
        }

        public void drawInchWorm(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
    
            for (int i = 0; i < _spider.Length; i++)
            {
                _spider[i].Draw();
            }
            //drawing obstacles
            for (int i = 0; i < 3; i++)
            {
                ScreenManager.SpriteBatch.Draw(_obstacle.Texture, ConvertUnits.ToDisplayUnits(_obstacles[i].Position),
                                        null,
                                        Color.White, _obstacles[i].Rotation, _obstacle.Origin , 1f,
                                        SpriteEffects.None, 0f);
            }

            ScreenManager.SpriteBatch.End();
            _border.Draw();
            _winSprite.Draw();

            base.Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            drawInchWorm(gameTime);
        }
    }
}