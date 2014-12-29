using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using Microsoft.Xna.Framework;
using FarseerPhysics.Samples.ScreenSystem;
using FarseerPhysics.Samples.DrawingSystem;
using FarseerPhysics.Samples.Demos.Prefabs;
using FarseerModel;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;

namespace FarseerPhysics.Samples
{
    public class Level5 : PhysicsGameScreen, IDemoScreen
    {


        private Border _border;
        private WinSprite _winSprite;

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

        private Body _compound;
        private Vector2 _origin;
        private Texture2D _polygonTexture;
        private Texture2D _alphabet;
        private float _scale;

        private Body _board;
        private Sprite _teeter;
        private Body _teeterAnchor;

        private Body _board2;
        private Sprite _teeter2;
        private Body _teeterAnchor2;

        private Body _board3;
        private Sprite _teeter3;
        private Body _teeterAnchor3;

        public KeyboardState oldState;
        public GamePadState oldPadState;
        AssetCreator creator;
        GameOver gameOver;
        Level6 level6;
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
            
            {
                _teeterAnchor = new Body(World);
                {
                    Vertices terrain = new Vertices();
                    terrain.Add(new Vector2(0f, -1f));

                    for (int i = 0; i < terrain.Count - 1; ++i)
                    {
                        FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], _teeterAnchor);
                    }

                    _teeterAnchor.Friction = 0.6f;
                }
                
                _board = new Body(World);
                _board.BodyType = BodyType.Dynamic;
                _board.Mass = 5f;
                _board.Position = new Vector2(8.0f, 4.0f);

                PolygonShape box = new PolygonShape(PolygonTools.CreateRectangle(3f, 0.5f), 1);
                _teeter =
                    new Sprite(ScreenManager.Assets.TextureFromShape(box, MaterialType.Pavement, Color.LightGray, 1.2f));

                _board.CreateFixture(box);

                RevoluteJoint teeterAxis = JointFactory.CreateRevoluteJoint(World, _teeterAnchor, _board, Vector2.Zero);

                teeterAxis.MotorSpeed = -5;
               
                teeterAxis.MaxMotorTorque = 1000;
                teeterAxis.MotorImpulse = -15;
                teeterAxis.MotorEnabled = true;
                

                _board.ApplyAngularImpulse(-100.0f);
            }

            {
                _teeterAnchor2 = new Body(World);
                {
                    Vertices terrain = new Vertices();
                    terrain.Add(new Vector2(0f, -1f));

                    for (int i = 0; i < terrain.Count - 1; ++i)
                    {
                        FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], _teeterAnchor2);
                    }

                    _teeterAnchor2.Friction = 0.6f;
                }

                _board2 = new Body(World);
                _board2.BodyType = BodyType.Dynamic;
                _board2.Position = new Vector2(-0.0f, 4.0f);
                _board2.Rotation = 1;

                PolygonShape box = new PolygonShape(PolygonTools.CreateRectangle(3f, 0.5f), 1);
                _teeter2 =
                    new Sprite(ScreenManager.Assets.TextureFromShape(box, MaterialType.Pavement, Color.LightGray, 1.2f));

                _board2.CreateFixture(box);

                RevoluteJoint teeterAxis = JointFactory.CreateRevoluteJoint(World, _teeterAnchor2, _board2, Vector2.Zero);

                teeterAxis.MotorSpeed = 15;
                teeterAxis.MaxMotorTorque = 100;
                teeterAxis.MotorImpulse = 20;
                teeterAxis.MotorEnabled = true;


                 _board2.ApplyAngularImpulse(100.0f);
            }

            // teeter 3

            {
                _teeterAnchor3 = new Body(World);
                {
                    Vertices terrain = new Vertices();
                    terrain.Add(new Vector2(0f, -1f));

                    for (int i = 0; i < terrain.Count - 1; ++i)
                    {
                        FixtureFactory.AttachEdge(terrain[i], terrain[i + 1], _teeterAnchor3);
                    }

                    _teeterAnchor3.Friction = 0.6f;
                }

                _board3 = new Body(World);
                _board3.BodyType = BodyType.Dynamic;
                _board3.Position = new Vector2(-8.0f, 4.0f);
                _board3.Rotation = 1;

                PolygonShape box = new PolygonShape(PolygonTools.CreateRectangle(3f, 0.5f), 1);
                _teeter3 =
                    new Sprite(ScreenManager.Assets.TextureFromShape(box, MaterialType.Pavement, Color.LightGray, 1.2f));

                _board3.CreateFixture(box);

                RevoluteJoint teeterAxis = JointFactory.CreateRevoluteJoint(World, _teeterAnchor3, _board3, Vector2.Zero);
                //teeterAxis.LowerLimit = -8.0f * Settings.Pi / 180.0f;
                //teeterAxis.UpperLimit = 8.0f * Settings.Pi / 180.0f;
                //teeterAxis.LimitEnabled = true;
                teeterAxis.MotorSpeed = 10;
                teeterAxis.MaxMotorTorque = 100;
                teeterAxis.MotorImpulse = 20;
                teeterAxis.MotorEnabled = true;


                 _board3.ApplyAngularImpulse(100.0f);
            }

            // LEVEL LOADER 

            //load texture that will represent the physics body
            _polygonTexture = ScreenManager.Content.Load<Texture2D>("LevelArt/Level6");

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

            //Adjust the scale of the object for WP7's lower resolution
#if WINDOWS_PHONE
            _scale = 0.6f;
#else
            _scale = 1f;
#endif

            //scale the vertices from graphics space to sim space
            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * _scale;
            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref vertScale);
            }

            //Create a single body with multiple fixtures
            _compound = BodyFactory.CreateCompoundPolygon(World, list, 1f, BodyType.Dynamic);
            _compound.BodyType = BodyType.Static;
            _compound.Restitution = 0.2f;
            _compound.Friction = 0.5f;
            _compound.Position = new Vector2(-5f, 8f);

// END LEVEL LOADER 

           // LoadBreakableObjects();

        }

        public void LoadBreakableObjects()
        {
           // DebugView.AppendFlags(DebugViewFlags.Shape);
            // BREAKABLE OBJECTS LOADER
            _alphabet = ScreenManager.Content.Load<Texture2D>("LevelArt/alphabet");

            uint[] data = new uint[_alphabet.Width * _alphabet.Height];
            _alphabet.GetData(data);

            List<Vertices> list = PolygonTools.CreatePolygon(data, _alphabet.Width, 3.5f, 20, true, true);

            float yOffset = -5f;
            float xOffset = -14f;
            for (int i = 0; i < list.Count; i++)
            {
                if (i == 9)
                {
                    yOffset = 0f;
                    xOffset = -14f;
                }
                if (i == 18)
                {
                    yOffset = 5f;
                    xOffset = -12.25f;
                }
                Vertices polygon = list[i];
                Vector2 centroid = -polygon.GetCentroid();
                polygon.Translate(ref centroid);
                polygon = SimplifyTools.CollinearSimplify(polygon);
                polygon = SimplifyTools.ReduceByDistance(polygon, 4);
                List<Vertices> triangulated = Triangulate.ConvexPartition(polygon, TriangulationAlgorithm.Bayazit);

#if WINDOWS_PHONE
                const float scale = 0.6f;
#else
                const float scale = 1f;
#endif
                Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * scale;
                foreach (Vertices vertices in triangulated)
                {
                    vertices.Scale(ref vertScale);
                }

                BreakableBody breakableBody = new BreakableBody(triangulated, World, 1);
                breakableBody.MainBody.Position = new Vector2(xOffset, yOffset);
                breakableBody.Strength = 100;
                World.AddBreakableBody(breakableBody);

                xOffset += 3.5f;
            }

            //END BREAKABLE OBJECTS
        }

        public override void LoadContent()
        {
            // Initialize camera controls
            base.LoadContent();
            LoadObstacles();
            Globals.currentLevel = 5;
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
                _spider[i] = new Spider(World, ScreenManager, new Vector2(-22f, -5f - (i + 1) * 2f));

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
            level6 = new Level6();

                ScreenManager.RemoveScreen(this);
                ScreenManager.AddScreen(level6);
            
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
            

            ScreenManager.SpriteBatch.End();
            _border.Draw();
            _winSprite.Draw();

            base.Draw(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            ScreenManager.SpriteBatch.Draw(_polygonTexture, ConvertUnits.ToDisplayUnits(_compound.Position), null, Color.Tomato, _compound.Rotation, _origin, _scale, SpriteEffects.None, 0f);
            ScreenManager.SpriteBatch.Draw(_polygonTexture, ConvertUnits.ToDisplayUnits(_compound.Position), null, Color.Tomato, _compound.Rotation, _origin, _scale, SpriteEffects.None, 0f);

           // draw teeter
           ScreenManager.SpriteBatch.Draw(_teeter.Texture, ConvertUnits.ToDisplayUnits(_board.Position), null, Color.White, _board.Rotation, _teeter.Origin, 1f, SpriteEffects.None, 0f);
           ScreenManager.SpriteBatch.Draw(_teeter2.Texture, ConvertUnits.ToDisplayUnits(_board2.Position), null, Color.White, _board2.Rotation, _teeter2.Origin, 1f, SpriteEffects.None, 0f);
           ScreenManager.SpriteBatch.Draw(_teeter3.Texture, ConvertUnits.ToDisplayUnits(_board3.Position), null, Color.White, _board3.Rotation, _teeter3.Origin, 1f, SpriteEffects.None, 0f);
         //  _circles.Draw();
            ScreenManager.SpriteBatch.End();
            
            drawInchWorm(gameTime);
        }
        public override void UnloadContent()
        {
            DebugView.RemoveFlags(DebugViewFlags.Shape);

            base.UnloadContent();
        }
    }
}