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
using Microsoft.Graphics.Canvas.UI.Xaml;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win2d_swipers
{

	public class FadingLine
	{
		public Vector2 Start;
		public Vector2 End;
		public int Timeout;
	}

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
			_lines = new List<FadingLine>();
        }

	    private Vector2 _leftCenter;
	    private Vector2 _rightCenter;
	    private float _swiperLength;
		private readonly List<FadingLine> _lines;
		private const int FadeStart = 360;

		private void FrameworkElement_OnSizeChanged(object sender, SizeChangedEventArgs e)
	    {
		    var mid = e.NewSize.Height/2.0;
			var w = e.NewSize.Width;
			var center = w/2.0;
		    _leftCenter = new Vector2((float) (center - w/10.0), (float) mid);
		    _rightCenter = new Vector2((float) (center + w/10.0), (float) mid);
		    _swiperLength = (float) w/4.0f;
	    }

	    private float _loopCounter;

	    private float ToRad(double angle)
	    {
		    return (float) ((Math.PI/180.0)*angle);
	    }

	    private void CanvasAnimatedControl_OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
	    {
		    Tick(args);
		    Tick(args);
		    Tick(args);
		}

	    private void Tick(CanvasAnimatedDrawEventArgs args)
	    {
			//swipe
		    var leftN = new Vector2(_leftCenter.X + (float) Math.Sin(ToRad(_loopCounter))*_swiperLength,
			    _leftCenter.Y + (float) Math.Cos(ToRad(_loopCounter))*_swiperLength);
		    var rightN = new Vector2(_rightCenter.X + (float) Math.Sin(ToRad(-1*_loopCounter + 180))*_swiperLength,
			    _rightCenter.Y + (float) Math.Cos(ToRad(-1*_loopCounter + 180))*_swiperLength);
		    args.DrawingSession.DrawLine(_leftCenter, leftN, Colors.Black, 1.0f);
		    args.DrawingSession.DrawLine(_rightCenter, rightN, Colors.Black, 1.0f);

		    //cleanup
		    for (var i = _lines.Count - 1; i >= 0; i--)
		    {
			    if (_lines[i].Timeout == 0) _lines.Remove(_lines[i]);
		    }
		    //draw
		    foreach (var fadingLine in _lines)
		    {
			    args.DrawingSession.DrawLine(fadingLine.Start, fadingLine.End,
				    Color.FromArgb((byte) (80*((float) fadingLine.Timeout/FadeStart)), 0, 0, 0));
			    fadingLine.Timeout -= 1;
		    }
		    //add new
		    _lines.Add(new FadingLine {End = rightN, Start = leftN, Timeout = FadeStart});
		    _lines.Add(new FadingLine {End = rightN, Start = _rightCenter, Timeout = FadeStart});
		    _lines.Add(new FadingLine {End = leftN, Start = _leftCenter, Timeout = FadeStart});

		    //loop
		    _loopCounter += 0.5f;
		    if (_loopCounter > 360.0) _loopCounter = 0.5f;
	    }
    }
}
