using UnityEngine;

public class StylesManager
{
	public GUIStyle layoutStyle;
	public GUIStyle texStyle;
	public GUIStyle toggleStyle;
	public GUIStyle timeLabelStyle;
	
	protected bool stylesLoaded; 
	
	public void loadStyles()
	{
		if(stylesLoaded)
			return;
		
		stylesLoaded = true;
		layoutStyle = new GUIStyle(GUI.skin.box); 
		layoutStyle.normal.textColor = layoutStyle.focused.textColor = Color.white;
		layoutStyle.hover.textColor = layoutStyle.active.textColor = Color.yellow;
		layoutStyle.onNormal.textColor = layoutStyle.onFocused.textColor = layoutStyle.onHover.textColor = layoutStyle.onActive.textColor = Color.green;
		layoutStyle.alignment = TextAnchor.UpperLeft;
		layoutStyle.padding = new RectOffset(8, 8, 8, 8);
		
		texStyle = new GUIStyle(GUI.skin.label);
		texStyle.margin = new RectOffset(0,0,0,0);
		texStyle.padding = new RectOffset(0,0,0,0);
		
		timeLabelStyle = new GUIStyle(GUI.skin.label);
		timeLabelStyle.normal.textColor = Color.green;
		
		toggleStyle = new GUIStyle(GUI.skin.toggle);
		toggleStyle.margin = new RectOffset(0, 70, 0, 0);
	}
	
}
