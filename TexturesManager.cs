using System;
using UnityEngine;

public class TexturesManager
{
	protected System.Collections.Generic.Dictionary<String, Texture2D> texDictionnary = new System.Collections.Generic.Dictionary<String, Texture2D>();
		
	public Texture2D getTexture(String name)
	{
		if(texDictionnary.ContainsKey(name))
			return texDictionnary[name];
		else
		{
			Texture2D newtex;
			newtex = new Texture2D(20, 32, TextureFormat.ARGB32, false);
			newtex.LoadImage(KSP.IO.File.ReadAllBytes<LocalTimePart>(name + ".png"));
			
			texDictionnary[name] = newtex;
			
			return newtex;
		}
	}
}