﻿// -----------------------------------------------------------------------
//  <copyright file="ApplicationTunnel.cs" company="HoovesWare">
//      Copyright (c) HoovesWare
//  </copyright>
//  <project>BotSuite.Net</project>
//  <purpose>framework for creating bots</purpose>
//  <homepage>http://botsuite.net/</homepage>
//  <license>http://botsuite.net/license/index/</license>
// -----------------------------------------------------------------------

namespace BotSuite
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Text;

	using global::BotSuite.Logging;

	/// <summary>
	///     control extern application by reading values, writing values, (to do: click controls)
	/// </summary>
	public class ApplicationTunnel
	{
		/// <summary>
		///     intern id of process
		/// </summary>
		private readonly int processId;

		/// <summary>
		///     process data
		/// </summary>
		private Process attachedProcess;

		/// <summary>
		///     handle of modul
		/// </summary>
		private ProcessModule attachedProcessModule;

		/// <summary>
		///     handle of process
		/// </summary>
		private IntPtr processHandle;

		/// <summary>
		///     Gets the base address.
		/// </summary>
		protected int BaseAddress { get; private set; }

		/// <summary>
		///     Initializes a new instance of the <see cref="ApplicationTunnel" /> class.
		/// </summary>
		/// <param name="id">
		///     id of process
		/// </param>
		public ApplicationTunnel(int id)
		{
			this.processId = id;
			this.BaseAddress = 0;
			this.processHandle = IntPtr.Zero;
			this.AttachProcess();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="ApplicationTunnel" /> class.
		/// </summary>
		/// <param name="processName">
		///     name of process
		/// </param>
		public ApplicationTunnel(string processName)
		{
			Process[] programmInstances = Process.GetProcessesByName(processName);

			if (programmInstances.Length == 0)
			{
				throw new ArgumentNullException();
			}

			this.processId = programmInstances[0].Id;
			this.BaseAddress = 0;
			this.processHandle = IntPtr.Zero;
			this.AttachProcess();
		}

		/// <summary>
		///     Finalizes an instance of the <see cref="ApplicationTunnel" /> class.
		/// </summary>
		~ApplicationTunnel()
		{
			this.DetachProcess();
		}

		/// <summary>
		///     get id of process by given name
		/// </summary>
		/// <example>
		///     <code>
		/// Process[] ListOfProcess = Memory.GetProcessIdByName("the name");
		/// </code>
		/// </example>
		/// <param name="name">
		///     name of process
		/// </param>
		/// <returns>
		///     an array of processes
		/// </returns>
		public static Process[] GetProcessIdByName(string name)
		{
			return Process.GetProcessesByName(name);
		}

		/// <summary>
		///     Gets the module base.
		/// </summary>
		/// <param name="procName">
		///     Name of the proc.
		/// </param>
		/// <param name="moduleName">
		///     Name of the module.
		/// </param>
		/// <returns>
		///     the module base
		/// </returns>
		public static int GetModuleBase(string procName, string moduleName)
		{
			return (from ProcessModule pm in Process.GetProcessesByName(procName)[0].Modules
					where string.Equals(moduleName, pm.ModuleName, StringComparison.CurrentCultureIgnoreCase)
					select (int)pm.BaseAddress).FirstOrDefault();
		}

		/// <summary>
		///     convert a hex string into int
		/// </summary>
		/// <example>
		///     <code>
		/// int result = Memory.Hex2Int"00B28498");
		/// </code>
		/// </example>
		/// <param name="hex">
		///     the hex string
		/// </param>
		/// <returns>
		///     result as integer
		/// </returns>
		public static int Hex2Int(string hex)
		{
			return int.Parse(hex, NumberStyles.HexNumber);
		}

		/// <summary>
		///     private function to attach a running process
		/// </summary>
		protected void AttachProcess()
		{
			this.attachedProcess = Process.GetProcessById(this.processId);
			const NativeMethods.ProcessAccessType AccessFlags =
				NativeMethods.ProcessAccessType.ProcessVmRead | NativeMethods.ProcessAccessType.ProcessVmWrite
				| NativeMethods.ProcessAccessType.ProcessVmOperation;

			this.processHandle = NativeMethods.OpenProcess((uint)AccessFlags, 1, (uint)this.processId);
			this.attachedProcessModule = this.attachedProcess.MainModule;
			this.BaseAddress = (int)this.attachedProcessModule.BaseAddress;
		}

		/// <summary>
		///     private function to detach a running process
		/// </summary>
		protected void DetachProcess()
		{
			int closeHandleReturn = NativeMethods.CloseHandle(this.processHandle);
			if (closeHandleReturn == 0)
			{
				// Code Zur Fehler Bearbeitung
			}
		}

		/// <summary>
		///     private function to read memory
		/// </summary>
		/// <param name="memoryAddress">
		///     the address
		/// </param>
		/// <param name="bytesToRead">
		///     the byte count to read
		/// </param>
		/// <param name="bytesRead">
		///     the byte count read
		/// </param>
		/// <returns>
		///     the value at the given address
		/// </returns>
		protected byte[] ReadMemoryAtAdress(IntPtr memoryAddress, uint bytesToRead, out int bytesRead)
		{
			byte[] buffer = new byte[bytesToRead];
			IntPtr ptrBytesRead;
			NativeMethods.ReadProcessMemory(this.processHandle, memoryAddress, buffer, bytesToRead, out ptrBytesRead);
			bytesRead = ptrBytesRead.ToInt32();
			return buffer;
		}

		/// <summary>
		///     read a value at a adress
		/// </summary>
		/// <example>
		///     <code>
		/// <![CDATA[
		/// // direct access at 00B28498
		/// int MyValue1 = Trainer.Read<int>("00B28498");
		/// // direct access at "001AAAC4", 0x464
		/// int MyValue2 = Trainer.Read<int>("001AAAC4", 0x464);
		/// float MyValue1 = Trainer.Read<float>("00B28498");
		/// double MyValue1 = Trainer.Read<double>("00B28498");
		/// uint MyValue1 = Trainer.Read<uint>("00B28498");
		/// ]]>
		/// </code>
		/// </example>
		/// <typeparam name="T">
		///     type of value
		/// </typeparam>
		/// <param name="address">
		///     address as string
		/// </param>
		/// <param name="offsets">
		///     offsets to follow to get value
		/// </param>
		/// <returns>
		///     value to read
		/// </returns>
		public T Read<T>(string address, params int[] offsets)
		{
			int off = int.Parse(address, NumberStyles.HexNumber);
			return this.Read<T>(off, offsets);
		}

		/// <summary>
		///     Read a value, see other Read-method
		/// </summary>
		/// <typeparam name="T">
		///     type of value
		/// </typeparam>
		/// <param name="address">
		///     address as integer
		/// </param>
		/// <param name="offsets">
		///     offsets to follow to get value
		/// </param>
		/// <returns>
		///     value to read
		/// </returns>
		public T Read<T>(int address, params int[] offsets)
		{
			IntPtr readAdress = (IntPtr)address;
			if (offsets.Length > 0)
			{
				readAdress = (IntPtr)this.Pointer(address, offsets);
			}

			uint size;
			byte[] buffer;
			int ptrBytesRead;

			if (typeof(T) == typeof(byte))
			{
				size = 1;
				buffer = this.ReadMemoryAtAdress(readAdress, size, out ptrBytesRead);
				return (T)(object)buffer[0];
			}

			if (typeof(T) == typeof(short))
			{
				size = 2;
				buffer = this.ReadMemoryAtAdress(readAdress, size, out ptrBytesRead);
				return (T)(object)((int)BitConverter.ToInt16(buffer, 0));
			}

			if (typeof(T) == typeof(int))
			{
				size = 4;
				buffer = this.ReadMemoryAtAdress(readAdress, size, out ptrBytesRead);
				return (T)(object)BitConverter.ToInt32(buffer, 0);
			}

			if (typeof(T) == typeof(uint))
			{
				size = 4;
				buffer = this.ReadMemoryAtAdress(readAdress, size, out ptrBytesRead);
				return (T)(object)((int)BitConverter.ToUInt32(buffer, 0));
			}

			if (typeof(T) == typeof(float))
			{
				size = 4;
				buffer = this.ReadMemoryAtAdress(readAdress, size, out ptrBytesRead);
				return (T)(object)BitConverter.ToSingle(buffer, 0);
			}

			if (typeof(T) == typeof(double))
			{
				size = 8;
				buffer = this.ReadMemoryAtAdress(readAdress, size, out ptrBytesRead);
				return (T)(object)BitConverter.ToDouble(buffer, 0);
			}

			return default(T);
		}

		/// <summary>
		///     private function to write at memory
		/// </summary>
		/// <param name="memoryAddress">
		///     the address
		/// </param>
		/// <param name="bytesToWrite">
		///     bytes to write
		/// </param>
		/// <returns>
		///     count of bytes written
		/// </returns>
		protected int WriteMemoryAtAdress(IntPtr memoryAddress, byte[] bytesToWrite)
		{
			IntPtr ptrBytesWritten;
			NativeMethods.WriteProcessMemory(
				this.processHandle,
				memoryAddress,
				bytesToWrite,
				(uint)bytesToWrite.Length,
				out ptrBytesWritten);
			return ptrBytesWritten.ToInt32();
		}

		/// <summary>
		///     write a value at memory
		/// </summary>
		/// <example>
		///     <code>
		/// <![CDATA[
		/// Memory Trainer = new Memory(...); 
		/// // direct access at 00B28498 
		/// Trainer.Write<int>("00B28498", an integer);
		/// // follow pointer access at "001AAAC4", 0x464
		/// Trainer.Write<int>("001AAAC4", an integer,0x464);
		/// Trainer.Write<float>("00B28498", a float var);
		/// Trainer.Write<double>("00B28498", a double var);
		/// Trainer.Write<uint>("00B28498", an unsigned integer);
		/// ]]>
		/// </code>
		/// </example>
		/// <typeparam name="T">
		///     type of value
		/// </typeparam>
		/// <param name="address">
		///     address to write
		/// </param>
		/// <param name="writeData">
		///     data to write
		/// </param>
		/// <param name="offsets">
		///     the offsets
		/// </param>
		public void Write<T>(string address, T writeData, params int[] offsets)
		{
			int off = int.Parse(address, NumberStyles.HexNumber);
			this.Write(off, writeData, offsets);
		}

		/// <summary>
		///     write at memory
		/// </summary>
		/// <typeparam name="T">
		///     type of data to write
		/// </typeparam>
		/// <param name="address">
		///     address to write
		/// </param>
		/// <param name="writeData">
		///     data to write
		/// </param>
		/// <param name="offsets">
		///     the offsets
		/// </param>
		public void Write<T>(int address, T writeData, params int[] offsets)
		{
			IntPtr writeAdress = (IntPtr)address;
			if (offsets.Length > 0)
			{
				writeAdress = (IntPtr)this.Pointer(address, offsets);
			}

			if (typeof(T) == typeof(byte))
			{
				this.WriteMemoryAtAdress(writeAdress, BitConverter.GetBytes(Convert.ToInt16(writeData)));
			}
			else if (typeof(T) == typeof(double))
			{
				this.WriteMemoryAtAdress(writeAdress, BitConverter.GetBytes(Convert.ToDouble(writeData)));
			}
			else if (typeof(T) == typeof(float))
			{
				this.WriteMemoryAtAdress(writeAdress, BitConverter.GetBytes(Convert.ToSingle(writeData)));
			}
			else if (typeof(T) == typeof(int))
			{
				this.WriteMemoryAtAdress(writeAdress, BitConverter.GetBytes(Convert.ToInt32(writeData)));
			}
		}

		/// <summary>
		///     Write a string of ASCII
		/// </summary>
		/// <param name="address">
		///     address to write
		/// </param>
		/// <param name="stringToWrite">
		///     string to write
		/// </param>
		public void WriteAscii(int address, string stringToWrite)
		{
			this.WriteMemoryAtAdress((IntPtr)address, Encoding.ASCII.GetBytes(stringToWrite + "\0"));
		}

		/// <summary>
		///     Writes a unicode string
		/// </summary>
		/// <param name="address">
		///     address to write
		/// </param>
		/// <param name="stringToWrite">
		///     string to write
		/// </param>
		public void WriteUnicode(int address, string stringToWrite)
		{
			this.WriteMemoryAtAdress((IntPtr)address, Encoding.Unicode.GetBytes(stringToWrite + "\0"));
		}

		/// <summary>
		///     follow a pointer by start address
		/// </summary>
		/// <example>
		///     <code>
		/// // start in BaseAddress add follow the pointers by adding the offsets
		/// int MyPointer2 = Trainer.Pointer( 0x284, 0xE4, 0xE4, 0x30, 0x108);
		/// </code>
		/// </example>
		/// <param name="start">
		///     start address
		/// </param>
		/// <param name="offsets">
		///     the offsets
		/// </param>
		/// <returns>
		///     a pointer
		/// </returns>
		public int Pointer(int start, params int[] offsets)
		{
			if (offsets.Length <= 0)
			{
				return start;
			}

			// target = this.Read<int>(pAddress);
			foreach (int offset in offsets)
			{
				start = this.Read<int>(start);
				start += offset;
			}

			return start;
		}

		/// <summary>
		///     follow a pointer by start address
		/// </summary>
		/// <example>
		///     <code>
		/// // start in 00B28498 add follow the pointers by adding the offsets
		/// int MyPointer2 = Trainer.Pointer("00B28498", 0x284, 0xE4, 0xE4, 0x30, 0x108);
		/// </code>
		/// </example>
		/// <param name="start">
		///     start address
		/// </param>
		/// <param name="offsets">
		///     the offsets
		/// </param>
		/// <returns>
		///     a pointer
		/// </returns>
		protected int Pointer(string start, params int[] offsets)
		{
			int target = Hex2Int(start);
			return this.Pointer(target, offsets);
		}

		/// <summary>
		///     returns handle of extern process
		/// </summary>
		/// <returns>a handle</returns>
		public IntPtr GetHandle()
		{
			return this.processHandle;
		}

		/// <summary>
		///     tries to close the main window of process
		/// </summary>
		public void Close()
		{
			this.attachedProcess.CloseMainWindow();
			this.attachedProcess.WaitForExit(4000);

			if (!this.attachedProcess.HasExited)
			{
				this.Kill();
			}
			else
			{
				this.attachedProcess.Dispose();
			}
		}

		/// <summary>
		///     kills radical the process
		/// </summary>
		public void Kill()
		{
			try
			{
				this.attachedProcess.Kill();
				this.attachedProcess.WaitForExit();
			}
			catch (Exception exception)
			{
				Logger.LogException(exception);
			}
		}
	}
}