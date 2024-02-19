#region [Copyright (c) 2015 Cristian Alexandru Geambasu]
//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2015 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
//	and associated documentation files (the "Software"), to deal in the Software without restriction, 
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial 
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using UnityEngine;
using UnityEditor;
using System;

namespace TeamUtilityEditor
{
	/// <summary>
	/// Clase que transforma de tipo Textura a tipo Base64, par pasar los de las escenas al cliente
	/// </summary>
	public class TextureToBase64Tool
	{
		[SerializeField] private Texture2D m_texture;
		//[SerializeField] private string m_encodedData = "";
		
		public static string Texture2DToBase64(Texture2D texture)
		{
			byte[] imageData = texture.EncodeToPNG();
			return Convert.ToBase64String(imageData);
		}
		
		public static Texture2D Base64ToTexture2D(string encodedData)
		{
			byte[] imageData = Convert.FromBase64String(encodedData);
			
			int width, height;
			GetImageSize(imageData, out width, out height);
			
			Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false, true);
			texture.hideFlags = HideFlags.HideAndDontSave;
			texture.filterMode = FilterMode.Point;
			texture.LoadImage(imageData);
			
			return texture;
		}
		
		private static void GetImageSize(byte[] imageData, out int width, out int height)
		{
			width = ReadInt(imageData, 3 + 15);
			height = ReadInt(imageData, 3 + 15 + 2 + 2);
		}
		
		private static int ReadInt(byte[] imageData, int offset)
		{
			return (imageData[offset] << 8) | imageData[offset + 1];
		}
	}
}
