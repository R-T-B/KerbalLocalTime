using System;
using UnityEngine;

namespace KLTUtils
{

	public static class Utils
	{
		public static void drawblanks(TexturesManager texman, StylesManager styman, String size, bool mySecs, bool myAm, bool existsSecs, bool existsAm, bool existsBoth)
		{
			if((mySecs && myAm) || (mySecs && !existsBoth))	
				return;
		
			if(!mySecs && !myAm)
			{
				if(existsBoth)
				{
					GUILayout.Label(texman.getTexture("secsblank_" + size), styman.texStyle);
					GUILayout.Label(texman.getTexture("amblank_" + size), styman.texStyle);
				}
				else if(existsSecs)
					GUILayout.Label(texman.getTexture("secsblank_" + size), styman.texStyle);
				else if(existsAm)
					GUILayout.Label(texman.getTexture("amblank_" + size), styman.texStyle);
			}
			else if(!mySecs && myAm)
			{
				if(existsBoth)
					GUILayout.Label(texman.getTexture("secsblank_" + size), styman.texStyle);
				else if(existsSecs)
					GUILayout.Label(texman.getTexture("interblank_" + size), styman.texStyle);
			}
			else if(mySecs && !myAm && existsBoth)
				GUILayout.Label(texman.getTexture("amblank_" + size), styman.texStyle);
		}
	}
	
} //ns KLCUtils