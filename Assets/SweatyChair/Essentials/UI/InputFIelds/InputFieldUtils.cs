using UnityEngine;
using UnityEngine.UI;

namespace SweatyChair.UI
{

	public static class InputFieldUtils
	{

		public static void SetValidationData(this InputField field, InputFieldValidationData validationData)
		{
			if (validationData == null) return;
			field.characterLimit = validationData.characterLimit;
			field.characterValidation = validationData.characterValidation;
			field.inputType = validationData.inputType;
			field.keyboardType = validationData.keyboardType;
		}

	}

	public class InputFieldValidationData
	{

		public int characterLimit = 0;
		public InputField.CharacterValidation characterValidation = InputField.CharacterValidation.None;
		public InputField.InputType inputType = InputField.InputType.Standard;
		public TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;
		public bool removeSpecialCharacters = false;

		public InputFieldValidationData(int characterLimit = 0, InputField.CharacterValidation characterValidation = InputField.CharacterValidation.None,
		                                InputField.InputType inputType = InputField.InputType.Standard, TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default,
										bool removeSpecialCharacters = false)
		{
			this.characterLimit = characterLimit;
			this.characterValidation = characterValidation;
			this.inputType = inputType;
			this.keyboardType = keyboardType;
			this.removeSpecialCharacters = removeSpecialCharacters;
		}

	}

}