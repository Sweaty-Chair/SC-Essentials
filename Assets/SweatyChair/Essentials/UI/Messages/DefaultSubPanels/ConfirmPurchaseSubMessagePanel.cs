using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public class ConfirmPurchaseSubMessagePanel : MessageSubPanel
	{

		#region Variables

		[Header("Core")]
		[SerializeField] private Text _titleText = null;
		[SerializeField] private Text _contentText = null;

		[Header("Buttons")]
		[SerializeField] private GameObject _confirmButtonGO = null;

		#endregion

		#region Set

		protected override void OnSet()
		{
			SetText(_titleText, _currentMessage.title);
			SetText(_contentText, _currentMessage.content);

			// TODO: Complete this.
		}

		#endregion

	}

}
