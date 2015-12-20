using UnityEngine;
using System.Collections;
using System;

namespace Custom {

	public class ActionWithTime : CustomYieldInstruction {
		private Action<float,float> actionByTime;
		private float timeScale;
		private Func<bool> keepWaitCondition;

		private float time;

		public override bool keepWaiting {
			get {
				if (!keepWaitCondition()) return false;

				time += Time.deltaTime * timeScale;
				bool isKeepWait = time < 1f;
				if (isKeepWait) {
					actionByTime (time, Time.deltaTime);
				} else {
					actionByTime (1f, Time.deltaTime);
				}
				return isKeepWait;
			}
		}

		public ActionWithTime(Action<float,float> actionByTime, float timeScale, Func<bool> keepWaitCondition = null) {
			this.actionByTime = actionByTime;
			this.timeScale = timeScale;
			this.keepWaitCondition = keepWaitCondition != null ? keepWaitCondition : () => true;

			time = 0f;
		}
	}

}
