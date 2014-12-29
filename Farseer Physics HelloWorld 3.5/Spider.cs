
using FarseerPhysics.Samples.DrawingSystem;
using FarseerPhysics.Samples.ScreenSystem;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace FarseerModel
{
    public class Spider
    {
        private const float SpiderBodyRadius = 0.325f;
        private RevoluteJoint _leftShoulderAngleJoint;
        private RevoluteJoint _rightShoulderAngleJoint;
        private float _s;
        private SpriteBatch _batch;
        private bool _shoulderFlexed;
        private float _shoulderTargetAngle = -1.0f;
        private Sprite _torso;
        private Sprite _leftLeg;
        private Sprite _rightLeg;
        private Vector2 _upperLegSize = new Vector2(2.2f, 0.6f);

        private Body _circle;
        private Body _leftUpper;
        private Body _rightUpper;

        private float gravity;

        public Spider(World world, ScreenManager screenManager, Vector2 position)
        {
            _batch = screenManager.SpriteBatch;

            //Load bodies
            _circle = BodyFactory.CreateCircle(world, SpiderBodyRadius, 1.0f, new Vector2(position.X, position.Y - 1));
            _circle.BodyType = BodyType.Dynamic;

            //Left upper leg
            _leftUpper = BodyFactory.CreateRectangle(world, _upperLegSize.X, _upperLegSize.Y, 1.0f, _circle.Position - new Vector2(SpiderBodyRadius, 0f) - new Vector2(_upperLegSize.X / 2f, 0f));
            _leftUpper.BodyType = BodyType.Dynamic;

            //Right upper leg
            _rightUpper = BodyFactory.CreateRectangle(world, _upperLegSize.X, _upperLegSize.Y, 1.0f, _circle.Position + new Vector2(SpiderBodyRadius, 0f) + new Vector2(_upperLegSize.X / 2f, 0f));
            _rightUpper.BodyType = BodyType.Dynamic;


            //Create joints
            _leftShoulderAngleJoint = JointFactory.CreateRevoluteJoint(world, _circle, _leftUpper, new Vector2(_upperLegSize.X / 2f, 0f));
            _leftShoulderAngleJoint.MotorEnabled = true;
            _leftShoulderAngleJoint.MaxMotorTorque = 1000f;

            _rightShoulderAngleJoint = JointFactory.CreateRevoluteJoint(world, _circle, _rightUpper, new Vector2(-_upperLegSize.X / 2f, 0f));
            _rightShoulderAngleJoint.MotorEnabled = true;
            _rightShoulderAngleJoint.MaxMotorTorque = 1000f;

            //addMass();
            //GFX
            AssetCreator creator = screenManager.Assets;
            _torso = new Sprite(creator.SpiderTexture(MaterialType.Pivot));
            _leftLeg = new Sprite(creator.SpiderTexture(MaterialType.sideB));
            _rightLeg = new Sprite(creator.SpiderTexture(MaterialType.sideA));



        }

        //This can be used to add mass to the objects
        public void addMass()
        {
            _leftUpper.Mass = 1;
            _rightUpper.Mass = 1;
            _circle.Mass = 1;
        }
        public void Update(GameTime gameTime)
        {
            _s += gameTime.ElapsedGameTime.Milliseconds;

                _shoulderFlexed = !_shoulderFlexed;
        }

        public Vector2 getLeftPosition()
        {
            return _leftUpper.Position;
        }

        public Vector2 getRightPosition()
        {
            return _leftUpper.Position;
        }

        public void setGravity(float gravity)
        {
            this.gravity = gravity;
        }
       
        public void handleKeyboard(  KeyboardState state)
        {

            if (state.IsKeyUp(Keys.A) || state.IsKeyUp(Keys.D))
            {
                _leftShoulderAngleJoint.MotorSpeed = 0.0f;
            }

            if (state.IsKeyUp(Keys.J) || state.IsKeyUp(Keys.L))
            {
                _rightShoulderAngleJoint.MotorSpeed = 0.0f;
            }

            if (state.IsKeyDown(Keys.A))
            {
                _leftShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
            }

            if (state.IsKeyDown(Keys.D))
            {
                _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
            }

            if (state.IsKeyDown(Keys.L))
            {
                _rightShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
            }

            if (state.IsKeyDown(Keys.J))
            {
                _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
            }

            //Move Right
            if (state.IsKeyDown(Keys.L) && state.IsKeyDown(Keys.D))
            {
                Console.WriteLine("1");
                if (gravity > 1 && _leftShoulderAngleJoint.JointAngle < 0.0f && _rightShoulderAngleJoint.JointAngle > 0.0f)
                {
                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X + 0.01f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X + 0.1f, _rightUpper.Position.Y);
                }
                else if (gravity < 1 && _leftShoulderAngleJoint.JointAngle > 0.0f && _rightShoulderAngleJoint.JointAngle < 0.0f)
                {
                    Console.WriteLine("2");
                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X + 0.01f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X + 0.1f, _rightUpper.Position.Y);
                }
                else
                {
                    _leftShoulderAngleJoint.MotorSpeed = 0.0f;
                    _rightShoulderAngleJoint.MotorSpeed = 0.0f;
                }


            }

            //Move Left
            if (state.IsKeyDown(Keys.J) && state.IsKeyDown(Keys.A))
            {
                if (gravity > 1 && _leftShoulderAngleJoint.JointAngle < 0.0f && _rightShoulderAngleJoint.JointAngle > 0.0f)
                {
                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X - 0.1f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X - 0.01f, _rightUpper.Position.Y);
                }
                else if (gravity < 1 && _leftShoulderAngleJoint.JointAngle > 0.0f && _rightShoulderAngleJoint.JointAngle < -0.35f)
                {
                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X - 0.1f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X - 0.01f, _rightUpper.Position.Y);
                }
                else
                {
                    _leftShoulderAngleJoint.MotorSpeed = 0.0f;
                    _rightShoulderAngleJoint.MotorSpeed = 0.0f;
                }

            }

            if (state.IsKeyDown(Keys.D) && state.IsKeyDown(Keys.J))
            {
                _leftShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                _rightShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
            }

            if (state.IsKeyDown(Keys.A) && state.IsKeyDown(Keys.L))
            {
                _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
            }

            //Increase angle
            if (state.IsKeyDown(Keys.RightShift))
            {
                _shoulderTargetAngle += 0.05f;


            }
            //Decrese angle
            if (state.IsKeyDown(Keys.RightControl))
            {
                _shoulderTargetAngle -= 0.05f;


            }
         

         
        }
        
             public void HandleGamePad(GamePadState padState)
            {


            if (padState.ThumbSticks.Left.X == 0) 
            {
                _leftShoulderAngleJoint.MotorSpeed = 0.0f;
            }
            if (padState.ThumbSticks.Right.X == 0)
            {
                _rightShoulderAngleJoint.MotorSpeed = 0.0f;
            }

            if (padState.ThumbSticks.Left.X >= 0.9f)
            {
                _leftShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
            }

            if (padState.ThumbSticks.Left.X <= -0.9)
            {
                _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
            }

            if (padState.ThumbSticks.Right.X <= -0.9)
            {
                _rightShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
            }

            if (padState.ThumbSticks.Right.X >= 0.9)
            {
                _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
            }

            //Move Right
            if (padState.ThumbSticks.Right.X >= 0.9 && padState.ThumbSticks.Left.X >= 0.9)
            {
                if (gravity > 1 && _leftShoulderAngleJoint.JointAngle < 0.0f && _rightShoulderAngleJoint.JointAngle > 0.0f)
                {

                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X + 0.01f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X + 0.1f, _rightUpper.Position.Y);
                }
                else if (gravity < 1 && _leftShoulderAngleJoint.JointAngle > .35f && _rightShoulderAngleJoint.JointAngle < 0.0f)
                {
                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X + 0.01f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X + 0.1f, _rightUpper.Position.Y);
                }
                else
                {
                    _leftShoulderAngleJoint.MotorSpeed = 0.0f;
                    _rightShoulderAngleJoint.MotorSpeed = 0.0f;
                }


            }

            //Move Left
            if (padState.ThumbSticks.Right.X <= -0.9 && padState.ThumbSticks.Left.X <= -0.9)
            {
                if (gravity > 1 && _leftShoulderAngleJoint.JointAngle < 0.0f && _rightShoulderAngleJoint.JointAngle > 0.0f)
                {
                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X - 0.1f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X - 0.01f, _rightUpper.Position.Y);
                }
                else if (gravity < 1 && _leftShoulderAngleJoint.JointAngle > 0.0f && _rightShoulderAngleJoint.JointAngle < -0.35f)
                {
                    _leftShoulderAngleJoint.MotorSpeed = -_shoulderTargetAngle;
                    _rightShoulderAngleJoint.MotorSpeed = +_shoulderTargetAngle;
                    _leftUpper.Position = new Vector2(_leftUpper.Position.X - 0.1f, _leftUpper.Position.Y);
                    _rightUpper.Position = new Vector2(_rightUpper.Position.X - 0.01f, _rightUpper.Position.Y);
                }
                else
                {
                    _leftShoulderAngleJoint.MotorSpeed = 0.0f;
                    _rightShoulderAngleJoint.MotorSpeed = 0.0f;
                }

            }
            
        }


        public void Draw()
        {
            _batch.Draw(_leftLeg.Texture, ConvertUnits.ToDisplayUnits(_leftUpper.Position), null, Color.White, _leftUpper.Rotation, _leftLeg.Origin, 0.1f, SpriteEffects.None, 0f);
            _batch.Draw(_rightLeg.Texture, ConvertUnits.ToDisplayUnits(_rightUpper.Position), null, Color.White, _rightUpper.Rotation, _rightLeg.Origin, 0.1f, SpriteEffects.None, 0f);
            _batch.Draw(_torso.Texture, ConvertUnits.ToDisplayUnits(_circle.Position), null, Color.White, _circle.Rotation, _torso.Origin, 0.03f, SpriteEffects.None, 0f);
        }

    }
}
