using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public delegate void NoArgFunc();

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

	//protected virtual void ThreadFunction() { }

	//protected virtual void OnFinished() { }

	public List<NoArgFunc> threadFunctions = new List<NoArgFunc>(); //cannot use Unity API
	public List<NoArgFunc> onFinish = new List<NoArgFunc>();  //can use Unity API
	public Dictionary<string, object> args = new Dictionary<string, object>();

	private void Run() {
		foreach (NoArgFunc func in threadFunctions)
			func();
		isDone = true;
	}

	public IEnumerator WaitFor() {
		while (!isDone) {
			yield return null;
		}
		foreach (NoArgFunc func in onFinish)
			func();
	}
}