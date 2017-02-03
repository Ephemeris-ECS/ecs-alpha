using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Evaluators;
using ModestTree;

namespace Engine.Sequencing
{
	public class SequenceFrame<TECS>
		where TECS : class, IECS
	{
		// TODO: definig these with delegates isnt very nice from a data driven perspective, but we wont worry about serialization till later

		public string Name { get; set; }

		/// <summary>
		/// Actions performed when the frame is entered
		/// </summary>
		public List<ECSAction<TECS>> OnEnterActions { get; set; }

		/// <summary>
		/// Actions performed when the frame is exited
		/// </summary>
		public List<ECSAction<TECS>> OnExitActions { get; set; }

		/// <summary>
		/// Condition to satisfy before proceeding to the next frame
		/// TODO: this could be a collection, or an expression tree with logic implied by aggregate nodes
		/// </summary>
		public IECSEvaluator<TECS> Evaluator { get; set; }

		private void ExecuteActions(IEnumerable<ECSAction<TECS>> actions, TECS ecs)
		{
			foreach (var action in actions)
			{
				try
				{
					action.Action(ecs);
				}
				catch (Exception ex)
				{
					throw new SequenceException($"Error performing action '{action.Name}' on frame '{Name}'", ex);
				}
			}
		}

		public void Enter(TECS ecs)
		{
			if (OnEnterActions != null)
			{
				ExecuteActions(OnEnterActions, ecs);
			}
		}

		public void Exit(TECS ecs)
		{
			if (OnExitActions != null)
			{
				ExecuteActions(OnExitActions, ecs);
			}
		}
	}
}
