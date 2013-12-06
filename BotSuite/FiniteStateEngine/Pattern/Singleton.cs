﻿// -----------------------------------------------------------------------
//  <copyright file="Singleton.cs" company="HoovesWare">
//      Copyright (c) HoovesWare
//  </copyright>
//  <project>BotSuite.Net</project>
//  <purpose>framework for creating bots</purpose>
//  <homepage>http://botsuite.net/</homepage>
//  <license>http://botsuite.net/license/index/</license>
// -----------------------------------------------------------------------

namespace BotSuite.FiniteStateEngine.Pattern
{
	/// <summary>
	///     singleton pattern
	/// </summary>
	/// <typeparam name="T">
	///     type of object
	/// </typeparam>
	public static class Singleton<T>
		where T : class, new()
	{
		/// <summary>
		///     private handle
		/// </summary>
		private static T pInstance;

		/// <summary>
		///     public constructor
		/// </summary>
		public static T Instance
		{
			get
			{
				return pInstance ?? (pInstance = new T());
			}
		}
	}
}