using System;
using UnityEngine;

public class StylesManager
{
	public GUIStyle layoutStyle;
	public GUIStyle texStyle;
	public GUIStyle toggleStyle;
	public GUIStyle timeLabelStyle;
    public GUIStyle timeStyle;
	public GUIStyle windowStyle;

    protected bool stylesLoaded; 
	
	public void loadStyles()
	{
		if(stylesLoaded)
			return;
		
		stylesLoaded = true;
		layoutStyle = new GUIStyle(HighLogic.Skin.box);
		layoutStyle.fontSize = (int)Math.Round(16 * GameSettings.UI_SCALE);
		layoutStyle.normal.textColor = layoutStyle.focused.textColor = Color.white;
		layoutStyle.hover.textColor = layoutStyle.active.textColor = Color.yellow;
		layoutStyle.onNormal.textColor = layoutStyle.onFocused.textColor = layoutStyle.onHover.textColor = layoutStyle.onActive.textColor = Color.green;
		layoutStyle.alignment = TextAnchor.UpperLeft;
		layoutStyle.padding = new RectOffset(8, 8, 8, 8);

        windowStyle = new GUIStyle(HighLogic.Skin.window);
        windowStyle.fontSize = (int)Math.Round(16 * GameSettings.UI_SCALE);
        windowStyle.normal.textColor = Color.white;

        timeLabelStyle = new GUIStyle(HighLogic.Skin.label);
        timeLabelStyle.fontSize = (int)Math.Round(14 * GameSettings.UI_SCALE);
        timeLabelStyle.normal.textColor = Color.green;

        timeStyle = new GUIStyle(HighLogic.Skin.label);
        timeStyle.fontSize = (int)Math.Round(14 * GameSettings.UI_SCALE);
        timeStyle.normal.textColor = Color.yellow;

        toggleStyle = new GUIStyle(HighLogic.Skin.toggle);
		toggleStyle.margin = new RectOffset(0, 70, 0, 0);
        toggleStyle.fontSize = (int)Math.Round(14 * GameSettings.UI_SCALE);
    }
	
}
