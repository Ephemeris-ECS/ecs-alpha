using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Evaluators;
using ModestTree;

namespace Engine.Sequencing
{
	// ReSharper disable once InconsistentNaming
	public class SequenceFrame<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		// TODO: definig these with delegates isnt very nice from a data driven perspective, but we wont worry about serialization till later

		public string Name { get; set; }

		/// <summary>
		/// Actions performed when the frame is entered
		/// </summary>
		public List<ECSAction<TECS, TConfiguration>> OnEnterActions { get; set; }

		/// <summary>
		/// Actions performed every time the frame is ticked
		/// </summary>
		public List<ECSAction<TECS, TConfiguration>> OnTickActions { get; set; }

		/// <summary>
		/// Actions performed when the frame is exited
		/// </summary>
		public List<ECSAction<TECS, TConfiguration>> OnExitActions { get; set; }

		/// <summary>
		/// Condition to satisfy before proceeding to the next frame
		/// TODO: this could be a collection, or an expression tree with logic implied by aggregate nodes
		/// </summary>
		public IEvaluator<TECS, TConfiguration> ExitCondition { get; set; }

		private void ExecuteActions(IEnumerable<ECSAction<TECS, TConfiguration>> actions, TECS ecs, TConfiguration configuration)
		{
			foreach (var action in actions)
			{
				try
				{
					action.Execute(ecs, configuration);
				}
				catch (Exception ex)
				{
					throw new SequenceException($"Error performing action '{action.Name}' on frame '{Name}'", ex);
				}
			}
		}

		private void InitializeActions(TECS ecs, TConfiguration configuration)
		{
			foreach (var action in (OnEnterActions ?? new List<ECSAction<TECS, TConfiguration>>())
				.Concat(OnTickActions ?? new List<ECSAction<TECS, TConfiguration>>())
				.Concat(OnExitActions ?? new List<ECSAction<TECS, TConfiguration>>()))
			{
				try
				{
					action.Initialize(ecs, configuration);
				}
				catch (Exception ex)
				{
					throw new SequenceException($"Error initializing action '{action.Name}' on frame '{Name}'", ex);
				}
			}
		}

		public void Enter(TECS ecs, TConfiguration configuration)
		{
			InitializeActions(ecs, configuration);
			ExitCondition.Initialize(ecs, configuration);
			if (OnEnterActions != null)
			{
				ExecuteActions(OnEnterActions, ecs, configuration);
			}
		}

		public void Exit(TECS ecs, TConfiguration configuration)
		{
			ExitCondition.Dispose();
			if (OnExitActions != null)
			{
				ExecuteActions(OnExitActions, ecs, configuration);
			}
		}

		public void Tick(TECS ecs, TConfiguration configuration)
		{
			if (OnTickActions != null)
			{
				ExecuteActions(OnTickActions, ecs, configuration);
			}
		}
	}
}
