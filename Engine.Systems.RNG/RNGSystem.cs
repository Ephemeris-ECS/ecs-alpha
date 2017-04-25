using Engine.Configuration;

namespace Engine.Systems.RNG
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// A system for providing deterministically generated random numbers, these must be consistent across all clients so the seed is provided in configuration
	/// </summary>
	public class RNGSystem : ISystem
	{
		// TODO: abstract the RNG behind an interface to allow for different sources
		private readonly Util.Random _random;

		// counter to verify our RNGS are in sync
		// TODO: this should be included in the sync state checksum - it isnt currently
		private long _counter;

		public long Count => _counter;

		private int _last;
		public int Last => _last;

		public RNGSystem(ECSConfiguration configuration)
		{
			_random = new Util.Random(configuration.RNGSeed);
		}

		public int Next()
		{
			_counter++;
			_last = _random.Next();
			return _last;
		}

		public int Next(int maxValue)
		{
			_counter++;
			_last = _random.Next(maxValue);
			return _last;
		}

		public int Next(int minValue, int maxValue)
		{
			_counter++;
			_last = _random.Next(minValue, maxValue);
			return _last;
		}

		//public void NextBytes(byte[] buffer)
		//{
		//	_counter++;
		//	_random.NextBytes(buffer);
		//}
		//public double NextDouble()
		//{
		//	_counter++;
		//	return _random.NextDouble();
		//}
		public void Dispose()
		{
			// nothing to dispose
		}
	}
}
