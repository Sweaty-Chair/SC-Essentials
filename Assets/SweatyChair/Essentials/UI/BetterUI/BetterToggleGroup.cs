using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI {

	/// <summary>
	/// Duplicate of the original source for toggle groups. For use with better toggles
	/// </summary>
	[AddComponentMenu("UI/BetterUI/BetterToggleGroup", 32)]
	[DisallowMultipleComponent]
	public class BetterToggleGroup : UIBehaviour {
		[SerializeField] private bool m_AllowSwitchOff = false;
		public bool allowSwitchOff { get { return m_AllowSwitchOff; } set { m_AllowSwitchOff = value; } }

		private List<BetterToggle> m_BetterToggles = new List<BetterToggle>();

		protected BetterToggleGroup() { }

		private void ValidateToggleIsInGroup(BetterToggle betterToggle) {
			if (betterToggle == null || !m_BetterToggles.Contains(betterToggle))
				throw new ArgumentException(string.Format("BetterToggle {0} is not part of BetterToggleGroup {1}", new object[] { betterToggle, this }));
		}

		public void NotifyToggleOn(BetterToggle betterToggle) {
			ValidateToggleIsInGroup(betterToggle);

			// disable all toggles in the group
			for (var i = 0; i < m_BetterToggles.Count; i++) {
				if (m_BetterToggles[i] == betterToggle)
					continue;

				m_BetterToggles[i].isOn = false;
			}
		}

		public void UnregisterToggle(BetterToggle betterToggle) {
			if (m_BetterToggles.Contains(betterToggle))
				m_BetterToggles.Remove(betterToggle);
		}

		public void RegisterToggle(BetterToggle betterToggle) {
			if (!m_BetterToggles.Contains(betterToggle))
				m_BetterToggles.Add(betterToggle);
		}

		public bool AnyTogglesOn() {
			return m_BetterToggles.Find(x => x.isOn) != null;
		}

		public IEnumerable<BetterToggle> ActiveToggles() {
			return m_BetterToggles.Where(x => x.isOn);
		}

		public void SetAllTogglesOff() {
			bool oldAllowSwitchOff = m_AllowSwitchOff;
			m_AllowSwitchOff = true;

			for (var i = 0; i < m_BetterToggles.Count; i++)
				m_BetterToggles[i].isOn = false;

			m_AllowSwitchOff = oldAllowSwitchOff;
		}
	}
}