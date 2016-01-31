using UnityEngine;
using System.Collections;
using System;

namespace Custom {
	public partial class Task : IEnumerator {
		private IEnumerator enumerator;

		public Task () {
			this.enumerator = dummy ();
		}

		public Task(IEnumerator enumerator) {
			this.enumerator = enumerator;
		}

		public virtual bool MoveNext () {
			return enumerator.MoveNext();
		}

		public virtual void Reset () {
			enumerator.Reset ();
		}
		public virtual object Current {
			get {
				return enumerator.Current;
			}
		}

		private IEnumerator dummy() {
			yield return null;
		}
	}

	public class Noop : Task {
		public Noop() : base(null) {
		}

		public override bool MoveNext () {
			return true;
		}

		public override object Current {
			get {
				return null;
			}
		}
	}

	public class JointTask : Task {
		private IEnumerator[] enumerators;

		public JointTask(IEnumerator enumerator, params IEnumerator[] enumerators) : base(enumerator) {
			this.enumerators = enumerators;
		}

		public override bool MoveNext () {
			bool hasNext = base.MoveNext ();
			for (int i = 0; i < enumerators.Length; i++) {
				hasNext = hasNext && enumerators [i].MoveNext ();
			}
			return hasNext;
		}
	}


	public class MultiTask : Task {
		private IEnumerator[] enumerators;

		public MultiTask(IEnumerator enumerator, params IEnumerator[] enumerators) : base(enumerator) {
			this.enumerators = enumerators;
		}

		public override bool MoveNext () {
			bool hasNext = base.MoveNext ();
			for (int i = 0; i < enumerators.Length; i++) {
				hasNext = enumerators [i].MoveNext () || hasNext;
			}
			return hasNext;
		}
	}

	public class PostTask : Task {
		private IEnumerator child;
		private object current;

		public PostTask(IEnumerator enumerator, IEnumerator child) : base(enumerator) {
			this.child = child;
		}

		public override bool MoveNext () {
			bool hasNext = base.MoveNext ();
			if (hasNext) {
				current = base.Current;
				return true;
			} else {
				hasNext = child.MoveNext ();
				if (hasNext) {
					current = child.Current;
					return true;
				}
			}
			return false;
		}

		public override object Current {
			get {
				return current;
			}
		}
	}

	public class TaskWithAction : Task {
		private Action action;

		public TaskWithAction(IEnumerator enumerator, Action action) : base(enumerator) {
			this.action = action;
		}

		public override bool MoveNext () {
			bool hasNext = base.MoveNext ();
			if (hasNext) {
				action ();
			}
			return hasNext;
		}
	}

	public class TaskWithCompleteAction : Task {
		private Action onCompleted;

		public TaskWithCompleteAction(IEnumerator enumerator, Action onCompleted) : base(enumerator) {
			this.onCompleted = onCompleted;
		}

		public override bool MoveNext () {
			bool hasNext = base.MoveNext ();
			if (!hasNext) {
				onCompleted ();
			}
			return hasNext;
		}
	}

	public class TaskWithPredicate : Task {
		private Func<bool> predicate;

		public TaskWithPredicate(IEnumerator enumerator, Func<bool> predicate) : base(enumerator) {
			this.predicate = predicate;
		}

		public override bool MoveNext () {
			return predicate () && base.MoveNext ();
		}
	}

	public partial class Task {

		public Task Continue(IEnumerator routine) {
			return new PostTask (this, routine);
		}

		public Task Continue(Func<IEnumerator> routiner) {
			return Continue (routiner ());
		}

		public Task And(IEnumerator routine) {
			return new JointTask (this, routine);
		}

		public Task Add(IEnumerator routine) {
			return new MultiTask (this, routine);
		}

		public Task OnCompleted(Action onCompleted) {
			return new TaskWithCompleteAction (this, onCompleted);
		}

		public Task OnUpdate(Action action) {
			return new TaskWithAction (this, action);
		}

		public Task While(Func<bool> predicate) {
			return new TaskWithPredicate (this, predicate);
		}

		public IEnumerator Count(float time) {
			float t = 0f;
			while (t < time) {
				t += Time.deltaTime;
				yield return null;
			}
		}

		public Task WhileInCount(float time) {
			return And (Count (time));
		}
	}

	public static class TaskUtility {

		public static Task ToTask(this IEnumerator enumerator) {
			return new Task (enumerator);
		}

		public static Coroutine StartBy(this IEnumerator enumerator, MonoBehaviour behav) {
			if (behav.gameObject.activeInHierarchy) {
				return behav.StartCoroutine (enumerator);
			} else {
				return null;
			}
		}

		public static Task Continue(this IEnumerator enumerator, IEnumerator routine) {
			return new PostTask (enumerator, routine);
		}

		public static Task Continue(this IEnumerator enumerator, Func<IEnumerator> routiner) {
			return enumerator.Continue (routiner ());
		}

		public static Task And(this IEnumerator enumerator, IEnumerator routine) {
			return new JointTask (enumerator, routine);
		}

		public static Task Add(this IEnumerator enumerator, IEnumerator routine) {
			return new MultiTask (enumerator, routine);
		}

		public static Task OnCompleted(this IEnumerator enumerator, Action onCompleted) {
			return new TaskWithCompleteAction (enumerator, onCompleted);
		}

		public static Task OnUpdate(this IEnumerator enumerator, Action action) {
			return new TaskWithAction (enumerator, action);
		}

		public static Task While(this IEnumerator enumerator, Func<bool> predicate) {
			return new TaskWithPredicate (enumerator, predicate);
		}

		public static Task When(this IEnumerator enumerator, Func<bool> predicate) {
			return enumerator.While (predicate).Add (new Noop ().While (() => !predicate ()));
		}

		public static Task WhileInCount(this IEnumerator enumerator, float time) {
			Task t = new Task (enumerator);
			return new JointTask (enumerator, t.Count(time));
		}

	}

}
