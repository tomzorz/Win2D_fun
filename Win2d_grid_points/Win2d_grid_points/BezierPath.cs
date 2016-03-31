using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2d_grid_points
{
	public class BezierPath
	{
		private readonly Vector2 _start;

		public bool IsFinalized { get; private set; }

		public BezierPath(Vector2 start)
		{
			_start = start;
			_curves = new List<BezierCurve>();
			_approximatedCurveLengths = new List<float>();
		}

		public void AddCurve(Vector2 controlSource, Vector2 controlTarget, Vector2 target)
		{
			if(IsFinalized) throw new Exception("Can't edit finalized curve!");
			var startp = !_curves.Any() ? _start : _curves.Last().End;
			_curves.Add(new BezierCurve(startp, controlSource, controlTarget, target));
		}

		private List<BezierCurve> _curves;

		private List<float> _approximatedCurveLengths;

		private const int ApproximationResolution = 100;

		public ImmutableList<BezierCurve> Curves => _curves.ToImmutableList();

		public void FinalizeCurve(Vector2 controlSource, Vector2 controlTarget, Vector2? target = null)
		{
			if (IsFinalized) throw new Exception("Can't edit finalized curve!");
			_curves.Add(new BezierCurve(_curves.Last().End, controlSource, controlTarget, target ?? _curves[0].Start));
			CloseItUp();
		}

		private void CloseItUp()
		{
			IsFinalized = true;
			foreach (var bezierCurve in _curves)
			{
				var currentLength = 0.0f;
				var prevLoc = bezierCurve.CalculatePoint(0.0f);
				for (var i = 1; i < ApproximationResolution; i++)
				{
					var nextLoc = bezierCurve.CalculatePoint(i / (float)ApproximationResolution);
					currentLength += Vector2.Distance(prevLoc, nextLoc);
					prevLoc = nextLoc;
				}
				_approximatedCurveLengths.Add(currentLength);
			}
		}
	}
}