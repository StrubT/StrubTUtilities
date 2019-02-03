using System;
using System.IO;
using System.Text;
using System.Threading;

namespace StrubT {

	public class ConsoleConfigResetter : IDisposable {

		static object Lock = new object();
		bool LockTaken = false;
		bool Disposed = false;

		#region console configuration fields

		ConsoleColor ForegroundColor { get; }

		ConsoleColor BackgroundColor { get; }

		string Title { get; }

		int BufferWidth { get; }

		int BufferHeight { get; }

		int WindowTop { get; }

		int WindowLeft { get; }

		int WindowWidth { get; }

		int WindowHeight { get; }

		int CursorTop { get; }

		int CursorLeft { get; }

		int CursorSize { get; }

		bool CursorVisible { get; }

		bool TreatControlCAsInput { get; }

		Encoding InputEncoding { get; }

		Encoding OutputEncoding { get; }

		TextReader In { get; }

		TextWriter Out { get; }

		TextWriter Error { get; }
		#endregion

		public ConsoleConfigScope Scope { get; }

		public ConsoleConfigResetter(ConsoleConfigScope scope = ConsoleConfigScope.Safe, bool locked = false) {

			if (locked)
				Monitor.Enter(Lock, ref LockTaken);

			Scope = scope;

			if (Scope.HasFlag(ConsoleConfigScope.Colors)) {
				ForegroundColor = Console.ForegroundColor;
				BackgroundColor = Console.BackgroundColor;
			}

			if (Scope.HasFlag(ConsoleConfigScope.Title))
				Title = Console.Title;

			if (Scope.HasFlag(ConsoleConfigScope.BufferSize)) {
				BufferWidth = Console.BufferWidth;
				BufferHeight = Console.BufferHeight;
			}

			if (Scope.HasFlag(ConsoleConfigScope.WindowPosition)) {
				WindowTop = Console.WindowTop;
				WindowLeft = Console.WindowLeft;
			}

			if (Scope.HasFlag(ConsoleConfigScope.WindowSize)) {
				WindowWidth = Console.WindowWidth;
				WindowHeight = Console.WindowHeight;
			}

			if (Scope.HasFlag(ConsoleConfigScope.CursorPosition)) {
				CursorTop = Console.CursorTop;
				CursorLeft = Console.CursorLeft;
			}

			if (Scope.HasFlag(ConsoleConfigScope.CursorSize))
				CursorSize = Console.CursorSize;

			if (Scope.HasFlag(ConsoleConfigScope.CursorVisible))
				CursorVisible = Console.CursorVisible;

			if (Scope.HasFlag(ConsoleConfigScope.TreatControlCAsInput))
				TreatControlCAsInput = Console.TreatControlCAsInput;

			if (Scope.HasFlag(ConsoleConfigScope.Encodings)) {
				InputEncoding = Console.InputEncoding;
				OutputEncoding = Console.OutputEncoding;
			}

			if (Scope.HasFlag(ConsoleConfigScope.Reader))
				In = Console.In;

			if (Scope.HasFlag(ConsoleConfigScope.Writers)) {
				Out = Console.Out;
				Error = Console.Error;
			}
		}

		public void Dispose() {

			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~ConsoleConfigResetter() => Dispose(false);

		protected virtual void Dispose(bool disposing) {

			if (Disposed)
				return;

			if (disposing) {
				if (Scope.HasFlag(ConsoleConfigScope.Colors)) {
					Console.ForegroundColor = ForegroundColor;
					Console.BackgroundColor = BackgroundColor;
				}

				if (Scope.HasFlag(ConsoleConfigScope.Title))
					Console.Title = Title;

				if (Scope.HasFlag(ConsoleConfigScope.BufferSize)) {
					Console.BufferWidth = BufferWidth;
					Console.BufferHeight = BufferHeight;
				}

				if (Scope.HasFlag(ConsoleConfigScope.WindowPosition)) {
					Console.WindowTop = WindowTop;
					Console.WindowLeft = WindowLeft;
				}

				if (Scope.HasFlag(ConsoleConfigScope.WindowSize)) {
					Console.WindowWidth = WindowWidth;
					Console.WindowHeight = WindowHeight;
				}

				if (Scope.HasFlag(ConsoleConfigScope.CursorPosition)) {
					Console.CursorTop = CursorTop;
					Console.CursorLeft = CursorLeft;
				}

				if (Scope.HasFlag(ConsoleConfigScope.CursorSize))
					Console.CursorSize = CursorSize;

				if (Scope.HasFlag(ConsoleConfigScope.CursorVisible))
					Console.CursorVisible = CursorVisible;

				if (Scope.HasFlag(ConsoleConfigScope.TreatControlCAsInput))
					Console.TreatControlCAsInput = TreatControlCAsInput;

				if (Scope.HasFlag(ConsoleConfigScope.Encodings)) {
					Console.InputEncoding = InputEncoding;
					Console.OutputEncoding = OutputEncoding;
				}

				if (Scope.HasFlag(ConsoleConfigScope.Reader))
					Console.SetIn(In);

				if (Scope.HasFlag(ConsoleConfigScope.Writers)) {
					Console.SetOut(Out);
					Console.SetError(Error);
				}
			}

			if (LockTaken) {
				Monitor.Exit(Lock);
				LockTaken = false;
			}

			Disposed = true;
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
