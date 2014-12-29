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
using Microsoft.Xna.Framework.Audio;
using System;

namespace FarseerPhysics.Samples
{
    public class Level2 : PhysicsGameScreen
    {


        //private Border _border;
        private Sprite _spriteObstacle;
        private Sprite _spriteObstacleTop1;
        private Sprite _spriteObstacleTop2;
        private Sprite _spriteObstacleTop3;
        private SoundEffect effect;
        private Sprite _boxSprite;


        private Body[] _obstacles = new Body[2];
        private Body obstacleTop1;
        private Body obstacleTop2;
        private Body obstacleTop3;

        private Body _box;

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition = new Vector2(0, 0);
        public KeyboardState oldState;
        public GamePadState oldPadState;
        KeyboardState state;
        private Spider[] _spider;

        const float BOUNDARY_OBSTACLE_X = 18.5f;
        const float BOUNDARY_OBSTACLE_Y = 11.6f;
        GameOver gameOver;

      
        private void LoadObstacles()
        {
            for (int i = 0; i < 2; ++i)
            {
                    _obstacles[i] = BodyFactory.CreateRectangle(World, 20f, 7f, 1f);
                    setObstacleProperty(_obstacles[i]);
            }

            _obstacles[0].Position = new Vector2(-BOUNDARY_OBSTACLE_X, BOUNDARY_OBSTACLE_Y);
            _obstacles[1].Position = new Vector2(BOUNDARY_OBSTACLE_X, BOUNDARY_OBSTACLE_Y);

            obstacleTop1 = BodyFactory.CreateRectangle(World, 10f, 5f, 1f);
            obstacleTop1.Position = new Vector2(-13.5f, -12.5f);
            obstacleTop2 = BodyFactory.CreateRectangle(World, 5f, 2f, 1f);
            obstacleTop2.Position = new Vector2(-6f, -14.5f);
            obstacleTop3 = BodyFactory.CreateRectangle(World, 10f, 5f, 1f);
            obstacleTop3.Position = new Vector2(1.5f, -12.5f);
            
            setObstacleProperty(obstacleTop1);
            setObstacleProperty(obstacleTop2);


            _box = BodyFactory.CreateRectangle(World, 5f, 3f, 1f);
            _box.BodyType = BodyType.Dynamic;
            _boxSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_box.FixtureList[0].Shape, MaterialType.Dots, Color.DarkViolet, 0.8f));
            _box.Position = new Vector2(-12f, 7f);
            _box.Friction = 0.2f;
            // create sprite based on body
            _spriteObstacle = new Sprite(ScreenManager.Assets.TextureFromShape(_obstacles[0].FixtureList[0].Shape,
                                                                         MaterialType.Dots,
                                                                         Color.DarkViolet, 0.8f));
            _spriteObstacleTop1 = new Sprite(ScreenManager.Assets.TextureFromShape(obstacleTop1.FixtureList[0].Shape,
                                                             MaterialType.Dots,
                                                             Color.DarkViolet, 0.8f));
            _spriteObstacleTop2 = new Sprite(ScreenManager.Assets.TextureFromShape(obstacleTop2.FixtureList[0].Shape,
                                                   MaterialType.Dots,
                                                   Color.DarkViolet, 0.8f));
            _spriteObstacleTop3 = new Sprite(ScreenManager.Assets.TextureFromShape(obstacleTop3.FixtureList[0].Shape,
                                                   MaterialType.Dots,
                                                   Color.DarkViolet, 0.8f));
        }

        public void setObstacleProperty(Body obstacle)
        {
            obstacle.IsStatic = true;
            obstacle.Restitution = 0.2f;
            obstacle.Friction = 0.2f;
        }

        public override void LoadContent()
        {
            // Initialize camera controls
            base.LoadContent();
            LoadObstacles();
            effect = ScreenManager.Content.Load<SoundEffect>("Sound/GameAudio3");
            SoundEffectInstance soundEffectInstance = effect.CreateInstance();
            soundEffectInstance.IsLooped = true;
            soundEffectInstance.Play();
            gameOver = new GameOver();
            LoadInchWorm();
            Globals.currentLevel = 2;
        }

        public void LoadInchWorm()
        {
            World.Gravity = new Vector2(0f, 20f);
            _spider = new Spider[1];
            for (int i = 0; i < _spider.Length; i++)
            {
                _spider[i] = new Spider(World, ScreenManager, new Vector2(-22f, 7.0f));
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
            _view = Matrix.CreateTranslation(new Vector3(_cameraPosition, 0f));

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


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            HandleKeyboard();
            if (IsActive)
            {
                for (int i = 0; i < _spider.Length; i++)
                {
                    _spider[i].Update(gameTime);
                }
            }
            if (_spider[0].getLeftPosition().Y > 17 && _spider[0].getRightPosition().Y > 17)
            {
                ScreenManager.AddScreen(gameOver);
                ScreenManager.RemoveScreen(this);
            }
            Console.WriteLine("Position of Spider : {0}", _spider[0].getLeftPosition());
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
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
            for (int i = 0; i < 2; i++)
            {
                ScreenManager.SpriteBatch.Draw(_spriteObstacle.Texture, ConvertUnits.ToDisplayUnits(_obstacles[i].Position),
                                        null,
                                        Color.White, _obstacles[i].Rotation, _spriteObstacle.Origin, 1f,
                                        SpriteEffects.None, 0f);
            }
            ScreenManager.SpriteBatch.Draw(_boxSprite.Texture, ConvertUnits.ToDisplayUnits(_box.Position), null, Color.White, _box.Rotation, _boxSprite.Origin, 1f, SpriteEffects.None, 0f);

            ScreenManager.SpriteBatch.Draw(_spriteObstacleTop1.Texture, ConvertUnits.ToDisplayUnits(obstacleTop1.Position),
                                        null,
                                        Color.White, obstacleTop1.Rotation, _spriteObstacleTop1.Origin, 1f,
                                        SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(_spriteObstacleTop2.Texture, ConvertUnits.ToDisplayUnits(obstacleTop2.Position),
                                       null,
                                       Color.White, obstacleTop2.Rotation, _spriteObstacleTop2.Origin, 1f,
                                       SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(_spriteObstacleTop3.Texture, ConvertUnits.ToDisplayUnits(obstacleTop3.Position),
                                  null,
                                  Color.White, obstacleTop3.Rotation, _spriteObstacleTop3.Origin, 1f,
                                  SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            drawInchWorm(gameTime);
        }
    }
}