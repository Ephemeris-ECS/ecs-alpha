using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zenject;

namespace Engine.Systems.Scoring
{
	public abstract class ScoringSystem : ISystem
	{
		protected List<IScoringExtension> ScoringExtensions;

		protected ScoringSystem([InjectOptional] List<IScoringExtension> scoringExtensions)
		{
			ScoringExtensions = scoringExtensions;
		}

		public virtual void Dispose()
		{
			foreach (var extension in ScoringExtensions)
			{
				extension.Dispose();
			}
		}
	}
}
