using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2d_grid_points
{
	public class BezierCurve
	{
		public Vector2 Start { get; private set; }

		public Vector2 ControlStart { get; private set; }

		public Vector2 ControlEnd { get; private set; }

		public Vector2 End { get; private set; }

		public BezierCurve(Vector2 start, Vector2 controlStart, Vector2 controlEnd, Vector2 end)
		{
			Start = start;
			ControlStart = controlStart;
			ControlEnd = controlEnd;
			End = end;
		}

		public Vector2 CalculatePoint(float f)
		{
			var u = 1.0f - f;
			var ff = f * f;
			var uu = u * u;
			var uuu = uu * u;
			var fff = ff * f;
			var p = uuu * Start;
			p += 3 * uu * f * ControlStart;
			p += 3 * u * ff * ControlEnd;
			p += fff * End;
			return p;
		}
	}
}