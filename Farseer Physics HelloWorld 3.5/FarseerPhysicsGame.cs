using System;
using FarseerPhysics.Samples.ScreenSystem;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Samples
{

    public class FarseerPhysicsGame : Game
    {
        private GraphicsDeviceManager _graphics;

        public FarseerPhysicsGame()
        {
            Window.Title = "Farseer Samples Framework";
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferMultiSampling = true;
#if WINDOWS || XBOX
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(24f);
            IsFixedTimeStep = true;
#elif WINDOWS_PHONE
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
            ConvertUnits.SetDisplayUnitToSimUnitRatio(16f);
            IsFixedTimeStep = false;
#endif
#if WINDOWS
            _graphics.IsFullScreen = false;
#elif XBOX || WINDOWS_PHONE
            _graphics.IsFullScreen = true;
#endif

            Content.RootDirectory = "Content";

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

           // FrameRateCounter frameRateCounter = new FrameRateCounter(ScreenManager);
           // frameRateCounter.DrawOrder = 101;
           // Components.Add(frameRateCounter);
        }

        public ScreenManager ScreenManager { get; set; }


        protected override void Initialize()
        {
            base.Initialize();
            LevelBonus game1 = new LevelBonus();
            MenuScreen menuScreen = new MenuScreen("Clapper Tentacle Games!!!!!");
            menuScreen.AddMenuItem(game1.GetTitle(), EntryType.Screen, game1);
            //menuScreen.AddMenuItem("Game Samples", EntryType.Separator, null);
            //menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("Exit", EntryType.ExitItem, null);
            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(menuScreen);
            //ScreenManager.AddScreen(new LogoScreen(TimeSpan.FromSeconds(3.0)));
        }
    }
}