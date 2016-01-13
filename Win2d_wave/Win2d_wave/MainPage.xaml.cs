using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win2d_wave
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

		private const int ParticleCount = 60;

	    private void CanvasAnimatedControl_OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
	    {
			_particles = new List<WaveParticle>();
		    var modX = (float)((sender.Size.Width/ParticleCount)/2.0);
			for (int i = 0; i < ParticleCount; i++)
			{
				_particles.Add(new WaveParticle
				{
					Velocity = 0.0f,
					Position = new Vector2((float)((sender.Size.Width / ParticleCount) * i) + modX, WaveParticle.HeightLevel),
					DrawPosition = new Vector2((float)((sender.Size.Width / ParticleCount) * i) + modX, 0.0f),
					DrawVelocity = new Vector2((float)((sender.Size.Width / ParticleCount) * i) + modX, 0.0f),
					RenderHeight = sender.Size.Height
				});
			}
	    }

	    private List<WaveParticle> _particles;

	    private void CanvasAnimatedControl_OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
	    {
			foreach (var waveParticle in _particles)
			{
				args.DrawingSession.DrawLine(waveParticle.DrawPosition, waveParticle.DrawVelocity, Colors.IndianRed, 3.0f);
				args.DrawingSession.FillCircle(waveParticle.DrawPosition, 3.0f, Colors.Aqua);
			}

		    foreach (var waveParticle in _particles) waveParticle.Tick();

			//?
		    var diff = new float[_particles.Count];
		    diff[0] = _particles[1].Position.Y - _particles[0].Position.Y;
		    for (int index = 1; index < _particles.Count - 1; index++)
		    {
			    var l = _particles[index - 1].Position.Y - _particles[index].Position.Y;
			    var r = _particles[index].Position.Y - _particles[index + 1].Position.Y;
			    diff[index] = (l - r)/2.0f;
		    }
		    diff[_particles.Count - 1] = _particles[_particles.Count - 1].Position.Y - _particles[_particles.Count - 2].Position.Y;

		    for (int index = 0; index < _particles.Count; index++)
		    {
			    //todo
		    }
	    }

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
	    {
		    _particles[ParticleCount/2].Position.Y += 150.0f;
	    }
    }

	public class WaveParticle
	{
		public const float Springiness = 0.025f;

		public const float Dampening = 0.055f;

		public const float Spread = 5.0f;

		public const float HeightLevel = 400.0f;

		public Vector2 Position;

		public float Velocity;

		public double RenderHeight;

		public Vector2 DrawPosition;

		public Vector2 DrawVelocity;

		public void Tick()
		{
			var acceleration = CalculateAcceleration(Position.Y, Velocity);
			Position.Y += Velocity;
			Velocity += acceleration;

			DrawPosition.Y = (float) (RenderHeight - Position.Y);
			DrawVelocity.Y = (float) (RenderHeight - Position.Y) + Velocity;
		}

		public static float CalculateAcceleration(float position, float velocity)
		{
			return (-1*Springiness*(position - HeightLevel)) - (Dampening*velocity);
		}
	}

}
