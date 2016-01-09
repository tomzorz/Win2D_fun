using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Win2d_circling
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private const float StartOffset = 6.0f;
		private const float EndOffset = 10.0f;
		private const float RingPadding = 4.0f;
		private const float RingStrokeWidth = 2.0f;

		public MainPage()
		{
			InitializeComponent();
			Loaded += MainPage_Loaded;
		}

		private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			var mw = Math.Min(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
			CanvasAnimatedControl.Height = mw;
			CanvasAnimatedControl.Width = mw;
		}

		private int _animationCounter;

		private void CanvasAnimatedControl_OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
		{
			for (int i = 0; i <= _ringCount; i++)
			{
				var currentOffset = StartOffset + i*(RingPadding + RingStrokeWidth);
				var c = new CanvasPathBuilder(sender);
				c.BeginFigure(currentOffset, 0.0f);
				c.AddArc(new Vector2(0.0f, 0.0f), currentOffset, currentOffset, 0.0f, (float)(Math.PI * 2));
				c.EndFigure(CanvasFigureLoop.Open);
				var g = CanvasGeometry.CreatePath(c);
				var m = _pattern[_animationCounter + (_ringCount - i)];
				_brush.Transform = Matrix3x2.CreateRotation((float) (Math.PI*2*m), _center);
				using (args.DrawingSession.CreateLayer(_brush))
				{
					args.DrawingSession.DrawGeometry(g, _center, Color.FromArgb(255,
						m < 0.5 ? Convert.ToByte(Math.Floor(m*2*255)) : Convert.ToByte(Math.Floor((1.5-m)*255)),
						m < 0.5 ? (byte)128 : Convert.ToByte(Math.Floor(m*255)),
						255), RingStrokeWidth);
				}
			}
			_animationCounter++;
			if (_animationCounter > _pattern.Count - _ringCount - 1) _animationCounter = 0;
		}

		private ICanvasBrush _brush;
		private Vector2 _center;
		private float _size;
		private float _half;
		private int _ringCount;
		private List<double> _pattern;

		private void CanvasAnimatedControl_OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
		{
			//guides
			_center = new Vector2((float)(sender.Size.Width / 2.0), (float)(sender.Size.Height / 2.0f));
			_size = (float) Math.Min(sender.Size.Height, sender.Size.Width);
			_half = _size / 2.0f;
			_ringCount = Convert.ToInt32(Math.Floor((_half - (StartOffset + EndOffset))/(RingPadding + RingStrokeWidth)));
			var nudge = _size * 0.164f;

			//mask
			var crt = new CanvasRenderTarget(sender, _size, _size, sender.Dpi);
			using (var session = crt.CreateDrawingSession())
			{
				session.Clear(new Vector4(0, 0, 0, 0));
				var pa = new CanvasGradientMeshPatch[4];
				pa[0] = CanvasGradientMesh.CreateCoonsPatch(
					new[]
					{
						new Vector2(_half, 0.0f), //center top
						new Vector2(_half, 0.0f), //center top
						new Vector2(_size, 0.0f), //right top
						new Vector2(_size, _half), //right middle
						new Vector2(_half-nudge, _half), //center middle (nudged left)
						new Vector2(_half-nudge, _half), //center middle (nudged left)
						new Vector2(_size, _half), //right middle
						new Vector2(_size, _size), //right bottom
						new Vector2(_half, _size), //center bottom
						new Vector2(_half, _size), //center bottom
						new Vector2(-nudge, _size), //left bottom
						new Vector2(-nudge, 0.0f), //left top
					},
					new[]
					{
						new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, 1),
						new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0),
					},
					new[]
					{
						CanvasGradientMeshPatchEdge.Antialiased, CanvasGradientMeshPatchEdge.Antialiased,
						CanvasGradientMeshPatchEdge.Antialiased, CanvasGradientMeshPatchEdge.Antialiased
					});

				var gm = new CanvasGradientMesh(session, pa);
				session.DrawGradientMesh(gm);
			}
			var ib = new CanvasImageBrush(sender, crt);
			_brush = ib;

			//animation pattern
			_pattern = new List<double>();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < _ringCount; j++)
				{
					_pattern.Add(0.0);
				}
			}
			for (int i = 0; i < _ringCount; i++)
			{
				_pattern.Add((1.0/_ringCount)*(i + 1));
			}
			for (int i = _ringCount - 1; i > 0; i--)
			{
				_pattern.Add((1.0 / _ringCount) * (i));
			}
			for (int j = 0; j < _ringCount; j++)
			{
				_pattern.Add(0.0);
			}
		}
	}
}