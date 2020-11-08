using System.Collections;
using UnityEngine;

namespace SweatyChair {

	public class WaitForFrames : CustomYieldInstruction {

		#region Variables

		private int framesToWaitUntil;	//We store when we can execute our method again

		#endregion

		#region Constructor

		public WaitForFrames(int frames)
		{
			framesToWaitUntil = Time.frameCount + frames;		//add our frames onto our frame count
		}

		#endregion

		#region Keep Waiting

		public override bool keepWaiting {
			get {
				return Time.frameCount <= framesToWaitUntil;	//We wait until our frame count is less or equal to the number of frames we wanted to wait
			}
		}

		#endregion

	}

}
