using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace SweatyChair.UI
{
	
	public static class FloatingTextManager
	{

		public static event UnityAction<FloatingText> shownEvent;
		public static event UnityAction<FloatingTextPosition> hidEvent;

		public static void Show(FloatingText floatingText)
		{
			if (shownEvent != null)
				shownEvent(floatingText);
		}

		public static void Hide(FloatingTextPosition position)
		{
			if (hidEvent != null)
				hidEvent(position);
		}

	}

}