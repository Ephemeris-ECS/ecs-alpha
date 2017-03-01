using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;

namespace Engine.Systems
{
	// ReSharper disable once InconsistentNaming
	public class RNGSystem : ISystem
	{
		// TODO: abstract the RNG behind an interface to allow for different sources
		private readonly Random _random;

		// counter to verify our RNGS are in sync
		// TODO: this should be included in the sync state checksum - it isnt currently
		private long _counter;

		public RNGSystem(ECSConfiguration configuration)
		{
			_random = new Random(configuration.RNGSeed);
		}

		public int Next()
		{
			_counter++;
			return _random.Next();
		}

		public int Next(int maxValue)
		{
			_counter++;
			return _random.Next(maxValue);
		}

		public int Next(int minValue, int maxValue)
		{
			_counter++;
			return _random.Next(minValue, maxValue);
		}

		public void NextBytes(byte[] buffer)
		{
			_counter++;
			_random.NextBytes(buffer);
		}
		public double NextDouble()
		{
			_counter++;
			return _random.NextDouble();
		}
	}
}
