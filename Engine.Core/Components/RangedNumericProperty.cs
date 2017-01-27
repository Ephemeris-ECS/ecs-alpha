using System;

namespace Engine.Components
{
	public abstract class RangedNumericProperty<TValue> : Property<TValue>
		where TValue : struct
	{
		public TValue MaxValue { get; private set; }
		public TValue MinValue { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">Initial value</param>
		/// <param name="minValue">Minimum value</param>
		/// <param name="maxValue">Maximum value</param>
		protected RangedNumericProperty(TValue value,
			TValue minValue, 
			TValue maxValue)
			: base(value)
		{
			MaxValue = maxValue;
			MinValue = minValue;
		}

		protected RangedNumericProperty()
		{
			
		}

		public abstract float GetValueAsProportion();
	}

	public abstract class RangedIntegerProperty : RangedNumericProperty<int>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">Initial value</param>
		/// <param name="minValue">Minimum value</param>
		/// <param name="maxValue">Maximum value</param>
		protected RangedIntegerProperty(int value, 
			int minValue, 
			int maxValue) 
			: base(value, minValue, maxValue)
		{
		}

		public override void SetValue(int value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.SetValue(value);
		}

		public override float GetValueAsProportion()
		{
			var range = (MaxValue - MinValue);
			return range > 0 ? Value / range : 0;
		}
	}

	public abstract class RangedDoubleProperty : RangedNumericProperty<double>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">Initial value</param>
		/// <param name="minValue">Minimum value</param>
		/// <param name="maxValue">Maximum value</param>
		protected RangedDoubleProperty(double value,
		double minValue, 
			double maxValue) 
			: base(value, minValue, maxValue)
		{
		}

		public override void SetValue(double value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.SetValue(value);
		}

		public override float GetValueAsProportion()
		{
			var range = (MaxValue - MinValue);
			return (float) (range > 0 ? Value / range : 0);
		}
	}

	public abstract class RangedFloatProperty : RangedNumericProperty<float>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value">Initial value</param>
		/// <param name="minValue">Minimum value</param>
		/// <param name="maxValue">Maximum value</param>
		protected RangedFloatProperty(float value,
			float minValue,
			float maxValue)
			: base(value, minValue, maxValue)
		{
		}

		public override void SetValue(float value)
		{
			value = Math.Max(MinValue, Math.Min(MaxValue, value));
			base.SetValue(value);
		}

		public override float GetValueAsProportion()
		{
			var range = (MaxValue - MinValue);
			return range > 0 ? Value / range : 0;
		}
	}
}
