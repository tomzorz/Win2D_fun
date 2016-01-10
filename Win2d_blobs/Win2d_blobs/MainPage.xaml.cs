using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Win2d_blobs
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

	    private const float BlobSize = 60.0f;

	    private const int Threshold = 50;

	    private const int BlobCount = 150;

	    private const float BlobVelocityScale = 0.75f;

	    private CanvasRenderTarget _blobRenderTarget;
	    private PremultiplyEffect _pre;
	    private TableTransferEffect _tte;
	    private UnPremultiplyEffect _un;
	    private CanvasImageBrush _mask;
	    private GaussianBlurEffect _gbe;

		private void CanvasAnimatedControl_OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
	    {
		    using (var ds = _blobRenderTarget.CreateDrawingSession())
		    {
				ds.Clear(sender.ClearColor);
				ds.Blend = CanvasBlend.Add;
				foreach (var blob in _blobs)
				{
					_blobBrush.Transform = Matrix3x2.CreateTranslation(blob.Position);
					ds.FillCircle(blob.Position, BlobSize, _blobBrush);
				}
			}
			_mask.Image = _un;
			using (args.DrawingSession.CreateLayer(_mask))
			{
				args.DrawingSession.DrawImage(_gbe);
			}

			foreach (var blob in _blobs)
		    {
			    blob.Position = Vector2.Add(blob.Position, blob.Velocity);
			    var xc = blob.Position.X < 0.0f || blob.Position.X > sender.Size.Width ? -1.0f : 1.0f;
			    var yc = blob.Position.Y < 0.0f || blob.Position.Y > sender.Size.Height ? -1.0f : 1.0f;
				blob.Velocity = new Vector2(blob.Velocity.X * xc, blob.Velocity.Y * yc);
			}
	    }

		private ICanvasBrush _blobBrush;
	    private List<Blob> _blobs;
	    private List<float> ttt;

	    private void CanvasAnimatedControl_OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
	    {
			// blobs
			_blobs = new List<Blob>();
			var rnd = new Random();
		    for (int i = 0; i < BlobCount; i++)
		    {
			    _blobs.Add(new Blob
			    {
				    Position = new Vector2((float) (rnd.NextDouble()*sender.Size.Width), (float) (rnd.NextDouble()*sender.Size.Height)),
				    Velocity = new Vector2(BlobVelocityScale *  (float) ((rnd.NextDouble()*3.0f) - 1.5f), BlobVelocityScale * (float) ((rnd.NextDouble()*3.0f) - 1.5f))
			    });
		    }
			// texture
		    var rgb = new CanvasRadialGradientBrush(sender, new[]
		    {
				new CanvasGradientStop
				{
					Color = Color.FromArgb(255, 128, 128, 255),
					Position = 0.0f
				},
				new CanvasGradientStop
				{
					Color = Color.FromArgb(128, 128, 128, 255),
					Position = 0.6f
				},
				new CanvasGradientStop
				{
					Color = Color.FromArgb(0, 128, 128, 255),
					Position = 1.0f
				}
		    });
		    rgb.RadiusX = rgb.RadiusY = BlobSize;
		    _blobBrush = rgb;
			// table transfer table
			ttt = new List<float>();
			for (int i = 0; i < 100; i++)
			{
				ttt.Add(i < Threshold ? 0.0f : 1.0f);
			}
			// setup
			_blobRenderTarget = new CanvasRenderTarget(sender, sender.Size);
			_pre = new PremultiplyEffect {Source = _blobRenderTarget};
			_tte = new TableTransferEffect {ClampOutput = true, Source = _pre};
			_tte.AlphaTable = _tte.RedTable = _tte.GreenTable = _tte.BlueTable = ttt.ToArray();
			_un = new UnPremultiplyEffect {Source = _tte};
			_gbe = new GaussianBlurEffect {BlurAmount = 8.0f, Source = _un};
		    _mask = new CanvasImageBrush(sender) {SourceRectangle = new Rect(new Point(), sender.Size)};
	    }
	}
}