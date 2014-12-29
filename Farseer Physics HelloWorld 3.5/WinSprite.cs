using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Samples.DrawingSystem;
using FarseerPhysics.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics;
using FarseerPhysics.Samples;

namespace FarseerModel
{
    public class WinSprite
    {
        public Body _anchor;
        private BasicEffect _basicEffect;
        private Camera2D _camera;
        private ScreenManager _screenManager;

        public WinSprite(World world, ScreenManager screenManager, Camera2D camera)
        {
            _screenManager = screenManager;
            _camera = camera;

            float halfWidth = ConvertUnits.ToSimUnits(screenManager.GraphicsDevice.Viewport.Width) /1.85f;
            float halfHeight = ConvertUnits.ToSimUnits(screenManager.GraphicsDevice.Viewport.Height) /1.6f;
            //Win states for levels
            switch (Globals.currentLevel)
            {
                case 1:
                    _anchor = BodyFactory.CreateEdge(world, new Vector2(halfWidth, -halfHeight), new Vector2(halfWidth, halfHeight));
                    break;
                case 2:
                _anchor = BodyFactory.CreateEdge(world, new Vector2(halfWidth, -halfHeight), new Vector2(halfWidth, halfHeight));
                    break;
                case 3:
                _anchor = BodyFactory.CreateEdge(world, new Vector2(halfWidth, -halfHeight), new Vector2(halfWidth, halfHeight));                    
                    break;
                case 4:
                    _anchor = BodyFactory.CreateEdge(world, new Vector2(-27f, 15f), new Vector2(-27f, 2f));
                    break;
                case 5:
                    _anchor = BodyFactory.CreateEdge(world, new Vector2(halfWidth, -halfHeight), new Vector2(halfWidth, halfHeight));
                    break;
                case 6:
                    _anchor = BodyFactory.CreateEdge(world, new Vector2(0, 18), new Vector2(10, 18));
                    break;
                case 7:
                    _anchor = BodyFactory.CreateEdge(world, new Vector2(halfWidth, -halfHeight), new Vector2(halfWidth, halfHeight));
                    break;
            }
            _anchor.CollisionCategories = Category.All;
            _anchor.CollidesWith = Category.All;
            _basicEffect = new BasicEffect(screenManager.GraphicsDevice);
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.TextureEnabled = true;
            _basicEffect.Texture = screenManager.Content.Load<Texture2D>("Materials/pavement");
          }
           


           
        

        public void Draw()
        {
            GraphicsDevice device = _screenManager.GraphicsDevice;
            LineBatch batch = _screenManager.LineBatch;
            batch.Begin(_camera.SimProjection, _camera.SimView);
            batch.DrawLineShape(_anchor.FixtureList[0].Shape);
            batch.End();
        }
    }
}