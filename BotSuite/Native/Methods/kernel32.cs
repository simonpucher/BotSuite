﻿// -----------------------------------------------------------------------
//  <copyright file="Kernel32.cs" company="Wieschoo &amp; Binary Overdrive">
//      Copyright (c) Wieschoo &amp; Binary Overdrive.
//  </copyright>
//  <project>BotSuite.Net</project>
//  <purpose>framework for creating bots</purpose>
//  <homepage>http://botsuite.net/</homepage>
//  <license>http://botsuite.net/license/index/</license>
// -----------------------------------------------------------------------

namespace BotSuite.Native.Methods
{
	using System;
	using System.Runtime.InteropServices;
	using System.Text;

	internal class Kernel32
	{
		/// <summary>
		///     The load library.
		/// </summary>
		/// <param name="lpFileName">
		///     The lp file name.
		/// </param>
		/// <returns>
		///     The <see cref="IntPtr" />.
		/// </returns>
		[DllImport("kernel32")]
		internal static extern IntPtr LoadLibrary(string lpFileName);

		/// <summary>
		///     The open process.
		/// </summary>
		/// <param name="dwDesiredAccess">
		///     The dw desired access.
		/// </param>
		/// <param name="bInheritHandle">
		///     The b inherit handle.
		/// </param>
		/// <param name="dwProcessId">
		///     The dw process id.
		/// </param>
		/// <returns>
		///     The <see cref="IntPtr" />.
		/// </returns>
		[DllImport("kernel32")]
		internal static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		/// <summary>
		///     The close handle.
		/// </summary>
		/// <param name="hObject">
		///     The h object.
		/// </param>
		/// <returns>
		///     The <see cref="int" />.
		/// </returns>
		[DllImport("kernel32")]
		internal static extern int CloseHandle(IntPtr hObject);

		/// <summary>
		///     The read process memory.
		/// </summary>
		/// <param name="hProcess">
		///     The h process.
		/// </param>
		/// <param name="lpBaseAddress">
		///     The lp base address.
		/// </param>
		/// <param name="buffer">
		///     The buffer.
		/// </param>
		/// <param name="size">
		///     The size.
		/// </param>
		/// <param name="lpNumberOfBytesRead">
		///     The lp number of bytes read.
		/// </param>
		/// <returns>
		///     The <see cref="int" />.
		/// </returns>
		[DllImport("kernel32")]
		internal static extern int ReadProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			[In] [Out] byte[] buffer,
			uint size,
			out IntPtr lpNumberOfBytesRead);

		/// <summary>
		///     The write process memory.
		/// </summary>
		/// <param name="hProcess">
		///     The h process.
		/// </param>
		/// <param name="lpBaseAddress">
		///     The lp base address.
		/// </param>
		/// <param name="buffer">
		///     The buffer.
		/// </param>
		/// <param name="size">
		///     The size.
		/// </param>
		/// <param name="lpNumberOfBytesWritten">
		///     The lp number of bytes written.
		/// </param>
		/// <returns>
		///     The <see cref="int" />.
		/// </returns>
		[DllImport("kernel32")]
		internal static extern int WriteProcessMemory(
			IntPtr hProcess,
			IntPtr lpBaseAddress,
			[In] [Out] byte[] buffer,
			uint size,
			out IntPtr lpNumberOfBytesWritten);

		/// <summary>
		///     The write private profile string.
		/// </summary>
		/// <param name="section">
		///     The section.
		/// </param>
		/// <param name="key">
		///     The key.
		/// </param>
		/// <param name="value">
		///     The value.
		/// </param>
		/// <param name="filePath">
		///     The file path.
		/// </param>
		/// <returns>
		///     The <see cref="long" />.
		/// </returns>
		[DllImport("kernel32")]
		internal static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

		/// <summary>
		///     The get private profile string.
		/// </summary>
		/// <param name="section">
		///     The section.
		/// </param>
		/// <param name="key">
		///     The key.
		/// </param>
		/// <param name="Default">
		///     The default.
		/// </param>
		/// <param name="retVal">
		///     The ret val.
		/// </param>
		/// <param name="size">
		///     The size.
		/// </param>
		/// <param name="filePath">
		///     The file path.
		/// </param>
		/// <returns>
		///     The <see cref="int" />.
		/// </returns>
		[DllImport("kernel32")]
		internal static extern int GetPrivateProfileString(
			string section,
			string key,
			string Default,
			StringBuilder retVal,
			int size,
			string filePath);
	}
}