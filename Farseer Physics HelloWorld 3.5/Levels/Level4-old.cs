
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using FarseerPhysics.Samples.ScreenSystem;
using FarseerPhysics.Samples.DrawingSystem;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common;
using FarseerModel;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace FarseerPhysics.Samples
{
    public class Level4old : PhysicsGameScreen, IDemoScreen
    {


        //private Border _border;
        private Body[] _obstacles = new Body[5];

        private Body _compound;
        private Vector2 _origin;
        private Texture2D _polygonTexture;
        private float _scale;

        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition = new Vector2(0, 0);
 
        KeyboardState state;
        private Spider[] _spider;
        private Vector2 _currentLeftPosition;
        private Vector2 _currentRightPosition;
        const float BOUNDARY_OBSTACLE_X = 18.5f;
        const float BOUNDARY_OBSTACLE_Y = 11.6f;
        GameOver gameOver;

        KeyboardState oldState;
        GamePadState oldPadState;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Level 3";
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
            /*
             //Create Ski jump
             _skiJump = new Body(World);
             {
                 Vertices terrain = new Vertices();
                 //terrain.Add(new Vector2(-32f, -10f)); // 1
                 terrain.Add(new Vector2(-30f, -8f)); // 1
                 terrain.Add(new Vector2(-20f, -8f));
                 terrain.Add(new Vector2(-12f, -2f));
                 terrain.Add(new Vector2(-3f, 10.6f));
                 terrain.Add(new Vector2(-2f, 11f));
                 terrain.Add(new Vector2(0f, 11.6f));
                 terrain.Add(new Vector2(1f, 11.6f)); //5
                 terrain.Add(new Vector2(3f, 10.6f)); //5
                 terrain.Add(new Vector2(3f, 15.6f)); //5
         

                 for (int i = 0; i < terrain.Count - 1; ++i)
                 {
                     FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], _skiJump);
                 }

                 _skiJump.Friction = 0.6f;
             }


            
             _skiJumpPoly = new Body(World);
             {
                 Vertices terrain = new Vertices();
                 //terrain.Add(new Vector2(-32f, -10f)); // 1
                 terrain.Add(new Vector2(-30f, -8f)); // 1
                 terrain.Add(new Vector2(-20f, -8f));
                 terrain.Add(new Vector2(-12f, -2f));
                 terrain.Add(new Vector2(-3f, 10.6f));
                 terrain.Add(new Vector2(-2f, 11f));
                 terrain.Add(new Vector2(0f, 11.6f));
                 terrain.Add(new Vector2(1f, 11.6f)); //5
                 terrain.Add(new Vector2(3f, 10.6f)); //5
                 terrain.Add(new Vector2(3f, 15.6f)); //5

                 List<Vertices> vertices = new List<Vertices>() ;
                 vertices.Add(terrain);

                 _skiJumpPoly = BodyFactory.CreateCompoundPolygon(World, vertices, 1f);
                 _skiJumpPoly.BodyType = BodyType.Static;
                 _skiJumpPoly.Friction = 0.6f;
             }
           
              &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&   
            // Create landing

            _landing = BodyFactory.CreateRectangle(World, 10f, 20f, 1f);
            _landing.IsStatic = true;
            _landing.Restitution = 0.2f;
            _landing.Friction = 0.2f;
            _landing.Position = new Vector2(25f, 5f);

            //Create Sprites
            _landingSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_landing.FixtureList[0].Shape,
                                                                         MaterialType.Dots,
                                                                         Color.DarkViolet, 0.8f));
           /* _skiJumpPolySprite = new Sprite(ScreenManager.Assets.TextureFromShape(_skiJumpPoly.FixtureList[0].Shape,
                                                                         MaterialType.Dots,
                                                                         Color.Pink, 0.8f));
            */

            //load texture that will represent the physics body
            _polygonTexture = ScreenManager.Content.Load<Texture2D>("LevelArt/Level4");

            //Create an array to hold the data from the texture
            uint[] data = new uint[_polygonTexture.Width * _polygonTexture.Height];

            //Transfer the texture data to the array
            _polygonTexture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            Vertices textureVertices = PolygonTools.CreatePolygon(data, _polygonTexture.Width, false);

            //The tool return vertices as they were found in the texture.
            //We need to find the real center (centroid) of the vertices for 2 reasons:

            //1. To translate the vertices so the polygon is centered around the centroid.
            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            //2. To draw the texture the correct place.
            _origin = -centroid;

            //We simplify the vertices found in the texture.
            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4f);

            //Since it is a concave polygon, we need to partition it into several smaller convex polygons
            List<Vertices> list = Triangulate.ConvexPartition(textureVertices, TriangulationAlgorithm.Bayazit);
            
            _scale = 1f;

            //scale the vertices from graphics space to sim space
            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * _scale;
            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref vertScale);
            }

            //Create a single body with multiple fixtures
            _compound = BodyFactory.CreateCompoundPolygon(World, list, 1f, BodyType.Dynamic);
            _compound.Position = new Vector2(-1f, 8f);
            _compound.BodyType = BodyType.Static;
            _compound.Friction = 0.5f;
            _compound.Restitution = 0.2f;

        
        }
        



        public override void LoadContent()
        {
            // Initialize camera controls
            base.LoadContent();
            LoadObstacles();
            gameOver = new GameOver();

            //_box = BodyFactory.CreateRectangle(World, 2f, 2f, 1f);
            //_box.BodyType = BodyType.Dynamic;
            //_boxSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_box.FixtureList[0].Shape, MaterialType.Squares, Color.Orange, 1f));
             LoadInchWorm();
             _currentLeftPosition = _spider[0].getLeftPosition();
             _currentRightPosition = _spider[0].getRightPosition();

        }

        public void LoadInchWorm()
        {
            World.Gravity = new Vector2(0f, 20f);
            //_border = new Border(World, ScreenManager, Camera);
            _spider = new Spider[1];
            for (int i = 0; i < _spider.Length; i++)
            {
                //default position
                //_spider[i] = new Spider(World, ScreenManager, new Vector2(10f, 8f - (i + 1) * 2f));
                _spider[i] = new Spider(World, ScreenManager, new Vector2(-24f, -11f - (i + 1) * 2f));

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

        public float getGravity()
        {
            return World.Gravity.Y;
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
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null,Camera.View);
            for (int i = 0; i < _spider.Length; i++)
            {
                _spider[i].Draw();
          
            }
            //drawing obstacles
         /*   for (int i = 0; i < 3; i++)
            {
                ScreenManager.SpriteBatch.Draw(_obstacle.Texture, ConvertUnits.ToDisplayUnits(_obstacles[i].Position),
                                        null,
                                        Color.White, _obstacles[i].Rotation, _obstacle.Origin, 1f,
                                        SpriteEffects.None, 0f);
            }
            */
            //ScreenManager.SpriteBatch.Draw(_boxSprite.Texture, ConvertUnits.ToDisplayUnits(_box.Position), null, Color.White, _box.Rotation, _boxSprite.Origin, 1f, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            
   
            ScreenManager.LineBatch.Begin(Camera.SimProjection, Camera.SimView);
           
           // ScreenManager.SpriteBatch.Draw(_landingSprite.Texture, ConvertUnits.ToDisplayUnits(_landing.Position), _landing, Color.White, _landing.Rotation, _landingSprite.Origin = _landing.Position, SpriteEffects.None, 0f);
            // draw ground
          /*  for (int i = 0; i < _skiJump.FixtureList.Count; ++i)
            {
                ScreenManager.LineBatch.DrawLineShape(_skiJump.FixtureList[i].Shape, Color.White);
            }*/
          
            ScreenManager.LineBatch.End();
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            /*  ScreenManager.SpriteBatch.Draw(_landingSprite.Texture, ConvertUnits.ToDisplayUnits(_landing.Position),
                                null,
                                Color.White, _landing.Rotation, _landingSprite.Origin, 1f,
                                SpriteEffects.None, 0f);
              ScreenManager.SpriteBatch.Draw(_skiJumpPolySprite.Texture, ConvertUnits.ToDisplayUnits(_skiJumpPoly.Position),
                                null,
                                Color.White, _skiJumpPoly.Rotation, _skiJumpPolySprite.Origin, 1f,
                                SpriteEffects.None, 0f);*/
            ScreenManager.SpriteBatch.Draw(_polygonTexture, ConvertUnits.ToDisplayUnits(_compound.Position), null, Color.White, _compound.Rotation, _origin, _scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.End();
            drawInchWorm(gameTime);
        }
    }
}