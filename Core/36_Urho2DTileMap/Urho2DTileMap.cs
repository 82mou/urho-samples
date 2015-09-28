using System;

namespace Urho.Samples
{
	public class _36_Urho2DTileMap : Sample
	{
		Scene scene;

		public _36_Urho2DTileMap(Context ctx) : base(ctx) { }

		public override void Start()
		{
			base.Start();
			CreateScene();
			SimpleCreateInstructionsWithWASD(", use PageUp PageDown keys to zoom.");
			SetupViewport();
		}

		protected override void OnSceneUpdate(float timeStep, Scene scene)
		{
			// Unsubscribe the SceneUpdate event from base class to prevent camera pitch and yaw in 2D sample
		}

		protected override void OnUpdate(float timeStep)
		{
			SimpleMoveCamera2D(timeStep);
		}

		void SetupViewport()
		{
			var renderer = Renderer;
			renderer.SetViewport(0, new Viewport(Context, scene, CameraNode.GetComponent<Camera>(), null));
		}

		void CreateScene()
		{
			scene = new Scene(Context);
			scene.CreateComponent<Octree>();

			// Create camera node
			CameraNode = scene.CreateChild("Camera");
			// Set camera's position
			CameraNode.Position = (new Vector3(0.0f, 0.0f, -10.0f));

			Camera camera = CameraNode.CreateComponent<Camera>();
			camera.SetOrthographic(true);

			var graphics = Graphics;
			camera.OrthoSize=(float)graphics.Height * PixelSize;
			camera.Zoom = (1.0f * Math.Min((float)graphics.Width / 1280.0f, (float)graphics.Height / 800.0f)); // Set zoom according to user's resolution to ensure full visibility (initial zoom (1.0) is set for full visibility at 1280x800 resolution)

			var cache = ResourceCache;
			// Get tmx file
			TmxFile2D tmxFile = cache.GetTmxFile2D("Urho2D/isometric_grass_and_water.tmx");
			if (tmxFile == null)
				return;

			Node tileMapNode = scene.CreateChild("TileMap");
			tileMapNode.Position = new Vector3(0.0f, 0.0f, -1.0f);

			TileMap2D tileMap = tileMapNode.CreateComponent<TileMap2D>();
			// Set animation
			tileMap.TmxFile = tmxFile;

			// Set camera's position
			TileMapInfo2D info = tileMap.Info;
			float x = info.MapWidth * 0.5f;
			float y = info.MapHeight * 0.5f;
			CameraNode.Position = new Vector3(x, y, -10.0f);
		}

		protected override string JoystickLayoutPatch => JoystickLayoutPatches.WithZoomInAndOut;
	}
}
