﻿//
// Copyright (c) 2008-2015 the Urho3D project.
// Copyright (c) 2015 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

namespace Urho.Samples
{
	public class _40_Actions : Sample
	{
		public _40_Actions(Context ctx) : base(ctx) { }

		Scene scene;
		Node mushroomNode;
		Node spriteNode;

		public override void Start()
		{
			base.Start();
			CreateScene();
			SimpleCreateInstructionsWithWasd();
			DoActions();
		}

		async void DoActions()
		{
			// Action API was introduced in Cocos2d and it's C# version - https://github.com/mono/CocosSharp/tree/master/src/actions
			
			// Ball sprite:
			FadeIn fadeIn = new FadeIn(durataion: 2);
			FadeOut fadeOut = new FadeOut(durtaion: 2);
			TintTo tintToRed = new TintTo(duration: 1, red: 1, green: 0, blue: 0);
			TintTo tintToGreen = new TintTo(duration: 1, red: 0, green: 1, blue: 0);
			TintTo tintToBlue = new TintTo(duration: 1, red: 0, green: 0, blue: 1);
			// RunActionsAsync accepts a list of actions to be executed consequentially (if you want to do it simultaneously - wrap with new Parallel(...) - see below
			var spriteActionsTask = spriteNode.RunActionsAsync(fadeOut, fadeIn, tintToRed, tintToGreen, tintToBlue);


			// Mushroom:
			MoveBy moveForwardAction = new MoveBy(duration: 1.5f, position: new Vector3(0, 0, 15));
			MoveBy moveRightAction = new MoveBy(duration: 1.5f, position: new Vector3(10, 0, 0));
			ScaleBy makeBiggerAction = new ScaleBy(duration: 1.5f, scale: 3);
			RotateTo rotateYAction = new RotateTo(duration: 2f, deltaAngleX: 0, deltaAngleY: 5, deltaAngleZ: 0);
			MoveTo moveToInitialPositionAction = new MoveTo(duration: 2, position: new Vector3(0, 0, 0));
			await mushroomNode.RunActionsAsync(moveForwardAction,
				new Parallel(moveRightAction, makeBiggerAction), //move right and increase scale simultaneously
				new Parallel(moveToInitialPositionAction, rotateYAction, makeBiggerAction.Reverse()));
			

			JumpBy jumpAction = new JumpBy(duration: 7, position: new Vector3(50, 0, 0), height: 8, jumps: 5);
			moveToInitialPositionAction.Duration = 5; //increase duration from 2s to 5s (2s is too fast for this step)
			await mushroomNode.RunActionsAsync(
				new EaseIn(jumpAction, 2), //you can wrap any action into "Easing function action" (by default it has linear behavior). See functions here: http://easings.net/
				new EaseElasticOut(moveToInitialPositionAction));
			await spriteActionsTask;

			//wait 1.5 seconds
			await Application.Delay(1500);
		}

		void CreateScene()
		{
			var cache = ResourceCache;
			scene = new Scene(Context);

			scene.CreateComponent<Octree>();
			var planeNode = scene.CreateChild("Plane");
			planeNode.Scale = new Vector3(100, 1, 100);
			var planeObject = planeNode.CreateComponent<StaticModel>();
			planeObject.Model = cache.GetModel("Models/Plane.mdl");
			planeObject.SetMaterial(cache.GetMaterial("Materials/StoneTiled.xml"));

			var lightNode = scene.CreateChild("DirectionalLight");
			lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
			var light = lightNode.CreateComponent<Light>();
			light.LightType = LightType.Directional;
			light.CastShadows = true;

			mushroomNode = scene.CreateChild("Mushroom");
			mushroomNode.Position = new Vector3(0, 0, 0);
			mushroomNode.Rotation = new Quaternion(0, 180, 0);
			mushroomNode.SetScale(2f);

			var mushroomObject = mushroomNode.CreateComponent<StaticModel>();
			mushroomObject.Model = cache.GetModel("Models/Mushroom.mdl");
			mushroomObject.SetMaterial(cache.GetMaterial("Materials/Mushroom.xml"));
			mushroomObject.CastShadows = true;

			var sprite = cache.GetSprite2D("Urho2D/ball.png");
			spriteNode = scene.CreateChild("StaticSprite2D");
			spriteNode.Position = new Vector3(0f, 10f, 10.0f);
			spriteNode.SetScale(8f);
			var staticSprite = spriteNode.CreateComponent<StaticSprite2D>();
			staticSprite.BlendMode = BlendMode.Alpha;
			staticSprite.Sprite = sprite;

			CameraNode = scene.CreateChild("camera");
			var camera = CameraNode.CreateComponent<Camera>();
			CameraNode.Position = new Vector3(0, 5, -20);
			Renderer.SetViewport(0, new Viewport(Context, scene, camera, null));
		}

		protected override void OnUpdate(float timeStep)
		{
			SimpleMoveCamera3D(timeStep);
		}
	}
}
