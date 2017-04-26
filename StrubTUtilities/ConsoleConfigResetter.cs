using System;
using System.IO;
using System.Text;
using System.Threading;

namespace StrubT {

	class ConsoleConfigResetter : IDisposable {

		static object @lock = new object();
		bool lockTaken = false;
		bool disposed = false;

		#region console configuration fields

		readonly ConsoleColor foregroundColor;
		readonly ConsoleColor backgroundColor;
		readonly string title;
		readonly int bufferWidth;
		readonly int bufferHeight;
		readonly int windowTop;
		readonly int windowLeft;
		readonly int windowWidth;
		readonly int windowHeight;
		readonly int cursorTop;
		readonly int cursorLeft;
		readonly int cursorSize;
		readonly bool cursorVisible;
		readonly bool treatControlCAsInput;
		readonly Encoding inputEncoding;
		readonly Encoding outputEncoding;
		readonly TextReader @in;
		readonly TextWriter @out;
		readonly TextWriter error;
		#endregion

		readonly ConsoleConfigScope _scope;

		public ConsoleConfigScope Scope { get { return _scope; } }

		public ConsoleConfigResetter(ConsoleConfigScope scope = ConsoleConfigScope.Safe, bool locked = false) {

			if (locked)
				Monitor.Enter(@lock, ref lockTaken);

			_scope = scope;

			if (Scope.HasFlag(ConsoleConfigScope.Colors)) {
				foregroundColor = Console.ForegroundColor;
				backgroundColor = Console.BackgroundColor;
			}

			if (Scope.HasFlag(ConsoleConfigScope.Title))
				title = Console.Title;

			if (Scope.HasFlag(ConsoleConfigScope.BufferSize)) {
				bufferWidth = Console.BufferWidth;
				bufferHeight = Console.BufferHeight;
			}

			if (Scope.HasFlag(ConsoleConfigScope.WindowPosition)) {
				windowTop = Console.WindowTop;
				windowLeft = Console.WindowLeft;
			}

			if (Scope.HasFlag(ConsoleConfigScope.WindowSize)) {
				windowWidth = Console.WindowWidth;
				windowHeight = Console.WindowHeight;
			}

			if (Scope.HasFlag(ConsoleConfigScope.CursorPosition)) {
				cursorTop = Console.CursorTop;
				cursorLeft = Console.CursorLeft;
			}

			if (Scope.HasFlag(ConsoleConfigScope.CursorSize))
				cursorSize = Console.CursorSize;

			if (Scope.HasFlag(ConsoleConfigScope.CursorVisible))
				cursorVisible = Console.CursorVisible;

			if (Scope.HasFlag(ConsoleConfigScope.TreatControlCAsInput))
				treatControlCAsInput = Console.TreatControlCAsInput;

			if (Scope.HasFlag(ConsoleConfigScope.Encodings)) {
				inputEncoding = Console.InputEncoding;
				outputEncoding = Console.OutputEncoding;
			}

			if (Scope.HasFlag(ConsoleConfigScope.Reader))
				@in = Console.In;

			if (Scope.HasFlag(ConsoleConfigScope.Writers)) {
				@out = Console.Out;
				error = Console.Error;
			}
		}

		public void Dispose() {

			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ConsoleConfigResetter() {

			Dispose(false);
		}

		protected virtual void Dispose(bool disposing) {

			if (disposed)
				return;

			if (disposing) {
				if (Scope.HasFlag(ConsoleConfigScope.Colors)) {
					Console.ForegroundColor = foregroundColor;
					Console.BackgroundColor = backgroundColor;
				}

				if (Scope.HasFlag(ConsoleConfigScope.Title))
					Console.Title = title;

				if (Scope.HasFlag(ConsoleConfigScope.BufferSize)) {
					Console.BufferWidth = bufferWidth;
					Console.BufferHeight = bufferHeight;
				}

				if (Scope.HasFlag(ConsoleConfigScope.WindowPosition)) {
					Console.WindowTop = windowTop;
					Console.WindowLeft = windowLeft;
				}

				if (Scope.HasFlag(ConsoleConfigScope.WindowSize)) {
					Console.WindowWidth = windowWidth;
					Console.WindowHeight = windowHeight;
				}

				if (Scope.HasFlag(ConsoleConfigScope.CursorPosition)) {
					Console.CursorTop = cursorTop;
					Console.CursorLeft = cursorLeft;
				}

				if (Scope.HasFlag(ConsoleConfigScope.CursorSize))
					Console.CursorSize = cursorSize;

				if (Scope.HasFlag(ConsoleConfigScope.CursorVisible))
					Console.CursorVisible = cursorVisible;

				if (Scope.HasFlag(ConsoleConfigScope.TreatControlCAsInput))
					Console.TreatControlCAsInput = treatControlCAsInput;

				if (Scope.HasFlag(ConsoleConfigScope.Encodings)) {
					Console.InputEncoding = inputEncoding;
					Console.OutputEncoding = outputEncoding;
				}

				if (Scope.HasFlag(ConsoleConfigScope.Reader))
					Console.SetIn(@in);

				if (Scope.HasFlag(ConsoleConfigScope.Writers)) {
					Console.SetOut(@out);
					Console.SetError(error);
				}
			}

			if (lockTaken) {
				Monitor.Exit(@lock);
				lockTaken = false;
			}

			disposed = true;
		}
	}

	[Flags]
	public enum ConsoleConfigScope {

		Colors = 0x1,
		Title = 0x2,
		Basic = Colors | Title,

		BufferSize = 0x4,

		WindowPosition = 0x8,
		WindowSize = 0x10,
		Window = WindowPosition | WindowSize,

		BufferAndWindow = BufferSize | Window,

		CursorPosition = 0x20,
		CursorSize = 0x40,
		CursorVisible = 0x80,
		Cursor = CursorPosition | CursorSize | CursorVisible,

		TreatControlCAsInput = 0x100,
		Behaviour = TreatControlCAsInput,

		Encodings = 0x200,
		Reader = 0x400,
		Writers = 0x800,
		InOut = Encodings | Reader | Writers,

		Full = Basic | BufferAndWindow | Cursor | Behaviour | InOut,
		Safe = Full & ~(WindowPosition | CursorPosition)
	}
}
