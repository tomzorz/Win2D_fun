using System;
using System.Collections.Generic;
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

// inspired by http://gfycat.com/ForsakenHeavyDrongo

namespace Win2d_grid_points
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();
			_staticPoints = new List<GridPoint>();
			_movingPoints = new List<MovingGridPoint>();
		}

		private void CanvasAnimatedControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Resize((CanvasAnimatedControl)sender);
		}

		private List<GridPoint> _staticPoints;
		private List<MovingGridPoint> _movingPoints;

		private void CanvasAnimatedControl_OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
			lock (_staticPoints)
			{
				foreach (var staticPoint in _staticPoints)
				{
					args.DrawingSession.FillCircle(staticPoint.Position, staticPoint.IsInteractive ? 5.0f : 2.0f, Colors.White);
				}
			}
			lock (_movingPoints)
			{

			}
		}

		private float _xCorrection;
		private float _yCorrection;

		private void CanvasAnimatedControl_OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
		{
			CanvasAnimatedControl.SizeChanged += CanvasAnimatedControl_SizeChanged;
			Resize(sender);
		}

		private void Resize(CanvasAnimatedControl sender)
		{
			sender.Paused = true;
			var size = (float)Math.Min(sender.Size.Width, sender.Size.Height);
			if (sender.Size.Width > sender.Size.Height)
			{
				_xCorrection = ((float)sender.Size.Width - size) / 2.0f;
			}
			else
			{
				_yCorrection = ((float)sender.Size.Height - size) / 2.0f;
			}
			var moveBy = size / 10.0f;
			var pad = moveBy / 2.0f;
			lock (_staticPoints)
			{

				_staticPoints.Clear();
				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j < 10; j++)
					{
						_staticPoints.Add(new GridPoint
						{
							IsInteractive = i > 1 && i < 8 && j > 1 && j < 8,
							Position = new Vector2(_xCorrection + pad + i * moveBy, _yCorrection + pad + j * moveBy)
						});
					}
				}
			}
			lock (_movingPoints)
			{
				_movingPoints.Clear();
				//var bp = new BezierPath(new Vector2(_xCorrection + pad + 2 * moveBy, _yCorrection + pad + 2 * moveBy));
				//bp.AddCurve(new Vector2(_xCorrection + pad + 2 * moveBy, _yCorrection + pad + 2 * moveBy));
				var mp = new MovingGridPoint();
				_movingPoints.Add(mp);
			}
			sender.Paused = false;
		}
	}
}