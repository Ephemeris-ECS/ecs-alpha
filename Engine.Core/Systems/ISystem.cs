﻿using System;

namespace Engine.Systems
{
	/// <summary>
	/// This is the base interface for systems. 
	/// Systems implement the majority of the game logic by colelcting groups of entities that have certain component 
	/// types associated with them and manipulating the state of those components
	/// </summary>
	public interface ISystem : IDisposable
	{
	}

	/// <summary>
	/// this is the base interface for systems that need to perform pre-game setup tasks
	/// </summary>
	public interface IInitializingSystem : ISystem
	{
		void Initialize();
	}

	/// <summary>
	/// This is the base interface for systems that perform a specific behaviour every cycle
	/// </summary>
	public interface ITickableSystem : ISystem, ITickable
	{
	}

	/// <summary>
	/// This is the base interface for systems that perform a task in response to some sort of signal
	/// </summary>
	public interface IReactiveSystem : ISystem
	{
		/// <summary>
		/// Excute the action, currently this ahs to be called externally as I havent defined a message passing or event system 
		/// </summary>
		void Execute();

	}
}
