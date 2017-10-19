using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class ThreadedJob {
	
	private object handle = new object();
	private Thread thread = null;

	private bool _isDone = false;
	public bool isDone {
		get {
			bool tmp;
			lock (handle) {
				tmp = _isDone;
			}
			return tmp;
		}
		set {
			lock (handle) {
				_isDone = value;
			}
		}
	}

	public virtual void Start() {
		thread = new Thread(Run);
		thread.Start();
	}
	public virtual void Abort() {
		thread.Abort();
	}

	protected virtual void ThreadFunction() { }

	protected virtual void OnFinished() { }

	private void Run() {
		ThreadFunction();
		isDone = true;
		OnFinished();
	}
}