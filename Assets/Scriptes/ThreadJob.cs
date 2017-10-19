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

	public List<NoArgFunc> threadFunctions = new List<NoArgFunc>();
	public List<NoArgFunc> onFinish = new List<NoArgFunc>();
	public Dictionary<string, object> args = new Dictionary<string, object>();

	private void Run() {
		Debug.Log(threadFunctions.Count);
		for (int i = 0; i < threadFunctions.Count; i++) {
			Debug.Log("hehe");
			threadFunctions[i]();
			Debug.Log("hehe");
		}
			
		isDone = true;
		//OnFinished();
	}

	public IEnumerator WaitFor() {
		while (!isDone) {
			yield return null;
		}
		for (int i = 0; i < onFinish.Count; i++)
			onFinish[i]();
	}
}