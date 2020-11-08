using System.Linq;
using UnityEngine;
using SweatyChair.MaterialSerializer;

namespace SweatyChair
{

	public static class MaterialUtils
	{

		#region Copy Shader keywords From Material

		public static void CopyAllShaderDataFromMaterial(this Material mat, Material otherMaterial) {
			//Copy both our shader properties and our shader materials
			mat.CopyShaderKeywordsFromMaterial(otherMaterial);
			mat.CopyShaderGIPropertiesFromMaterial(otherMaterial);
			mat.CopyShaderPropertiesFromMaterial(otherMaterial);
		}

		public static void CopyShaderGIPropertiesFromMaterial(this Material mat, Material otherMaterial) {
			mat.globalIlluminationFlags = otherMaterial.globalIlluminationFlags;
		}

		public static void CopyShaderPropertiesFromMaterial(this Material mat, Material otherMaterial) {
			MaterialViewUtils.CopyMaterialProperties(mat, otherMaterial);
		}

		/// <summary>
		/// Copies shader keywords from one material to another material. Ignoring all keywords not shared between the two.
		/// </summary>
		/// <param name="mat"></param>
		/// <param name="otherMaterial"></param>
		public static void CopyShaderKeywordsFromMaterial(this Material mat, Material otherMaterial) {
			//Get all our shared material keywords
			string[] currentShaderKeywords = mat.shaderKeywords;
			string[] otherShaderKeywords = otherMaterial.shaderKeywords;
			string[] sharedKeywords = currentShaderKeywords.Intersect(otherShaderKeywords).ToArray();

			//Go through each of our shared keywords and enable them
			for (int i = 0; i < sharedKeywords.Length; i++) {
				string currentKeyword = sharedKeywords[i];

				//Set our keyword value
				if (otherMaterial.IsKeywordEnabled(currentKeyword)) {
					mat.EnableKeyword(currentKeyword);

				} else {
					mat.DisableKeyword(currentKeyword);
				}
			}
		}

		#endregion

	}

}