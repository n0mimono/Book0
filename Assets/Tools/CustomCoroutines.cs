using UnityEngine;
using System.Collections;
using System;

namespace Custom {

	public class ActionWithTime : CustomYieldInstruction {
		private Func<float, float> timeToValue;
		private Action<float> actionByTime;
		private float timeScale;
		private Func<bool> keepWaitCondition;

		private float time;

		public override bool keepWaiting {
			get {
				if (!keepWaitCondition()) return false;

				time += Time.deltaTime * timeScale;
				bool isKeepWait = time < 1f;
				if (isKeepWait) {
					actionByTime (timeToValue (time));
				} else {
					actionByTime (timeToValue (1f));
				}
				return isKeepWait;
			}
		}

		public ActionWithTime(Func<float, float> timeToValue, Action<float> actionByTime, float timeScale, Func<bool> keepWaitCondition = null) {
			this.timeToValue = timeToValue;
			this.actionByTime = actionByTime;
			this.timeScale = timeScale;
			this.keepWaitCondition = keepWaitCondition != null ? keepWaitCondition : () => true;

			time = 0f;
		}
	}

}
