
//TOCHECK MULTIPLE PARTS
//TOCHECK SIDERAL +-12
//TOCHECK MAIN BODY HOUR DISPLAY/OPTIONS

//CHECKED CHECK USINGS
//TOCHECK FIX BLANKS
//TODO CHECK CONFIG SAVE
//TODO COMMENTS


using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using KSP.IO;
using UnityEngine;
using KLTUtils;
 
public class LocalTimePart : Part
{
	TexturesManager texman = new TexturesManager();
	StylesManager styman = new StylesManager();
	
	protected Rect windowPos;
	protected Rect optionsWindowPos;
	protected bool toggled = false;
	protected bool options = false;

	protected bool displaystrings = true;
	protected bool displayksc = true;
	protected bool displaylocal = true;
	protected bool displayrel = true;
	protected bool displayreal = true;
	protected bool display24ksc = true;
	protected bool display24local = true;
	protected bool display24rel = true;
	protected bool display24real = true;
	protected bool displaySecsksc = true;
	protected bool displaySecslocal = true;
	protected bool displaySecsrel = true;
	protected bool displaySecsreal = true;
	protected bool displayTZksc = true;
	protected bool displayTZlocal = true;
	protected bool displayTZrel = true;
	
	private static PluginConfiguration config = null;
	
	private bool existsSecs()
	{
		return ((displaySecslocal && displaylocal) || (displaySecsrel && displayrel) || (displaySecsksc && displayksc) || (displaySecsreal && displayreal));
	}
	
	private bool existsAm()
	{
		return ((!display24local && displaylocal) || (!display24rel && displayrel) || (!display24ksc && displayksc) || (!display24real && displayreal));
	}
	
	private bool existsBoth()
	{
		return ((!display24local && displaySecslocal && displaylocal) 
		        || (!display24rel && displaySecsrel && displayrel) 
		        || (!display24ksc && displaySecsksc && displayksc) 
		        || (!display24real && displaySecsreal && displayreal));
	}
 
	private void WindowGUI(int windowID)
	{
		GUILayout.BeginVertical(styman.layoutStyle);

		if(displaylocal)
		{
			if(displaystrings)
				GUILayout.Label("Local Time", styman.timeLabelStyle);
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(texman.getTexture("local"), styman.texStyle);
				Utils.drawblanks(texman, styman, "top", false, false, existsSecs(), existsAm(), existsBoth());
				
				GUILayout.EndHorizontal();
			}
			timeDoubleToDisplay(localTime(), timeZone(), display24local, displaySecslocal, displayTZlocal, vessel.mainBody.bodyName.First());
		}
		if(displayrel)
		{
			if(displaystrings)
				GUILayout.Label(vessel.mainBody.referenceBody.bodyName + " Related Time", styman.timeLabelStyle);
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(texman.getTexture("rel"), styman.texStyle);
				Utils.drawblanks(texman, styman, "top", false, false, existsSecs(), existsAm(), existsBoth());
				
				GUILayout.EndHorizontal();
			}
			timeDoubleToDisplay(localRelativeTime(), timeZone(), display24rel, displaySecsrel, displayTZrel, vessel.mainBody.bodyName.First());
		}
		if(displayksc)
		{
			if(displaystrings)
				GUILayout.Label("KSC Time", styman.timeLabelStyle);
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(texman.getTexture("ksc"), styman.texStyle);
				Utils.drawblanks(texman, styman, "top", false, false, existsSecs(), existsAm(), existsBoth());
				
				GUILayout.EndHorizontal();
			}
			
			timeDoubleToDisplay(kscTime(), -5, display24ksc, displaySecsksc, displayTZksc, 'K');
		}
		if(displayreal)
		{
			if(displaystrings)
				GUILayout.Label("Real World Time", styman.timeLabelStyle);
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(texman.getTexture("earth"), styman.texStyle);
				Utils.drawblanks(texman, styman, "top", false, false, existsSecs(), existsAm(), existsBoth());
				
				GUILayout.EndHorizontal();
				
			}
			realWorldTime();
		}
		GUILayout.EndVertical();
		//DragWindow makes the window draggable. The Rect specifies which part of the window it can by dragged by, and is 
		//clipped to the actual boundary of the window. You can also pass no argument at all and then the window can by
		//dragged by any part of it. Make sure the DragWindow command is AFTER all your other GUI input stuff, or else
		//it may "cover up" your controls and make them stop responding to the mouse.
		GUI.DragWindow(new Rect(0, 0, 10000, 20));
 
	}
	
	private void OptionsGUI(int windowID)
	{
		bool before;
		bool wndChanged = false;
		bool optChanged = false;

		GUILayout.BeginVertical();
		before=displaystrings;
		displaystrings = GUILayout.Toggle(displaystrings, "Display as text", styman.toggleStyle);
		wndChanged = (before != displaystrings) || wndChanged;
			
		before=displaylocal;
		displaylocal = GUILayout.Toggle(before, "Local Time", styman.toggleStyle);
		wndChanged = (before != displaylocal) || wndChanged;
		optChanged = (before != displaylocal) || optChanged;
			
		if(displaylocal)
		{
			GUILayout.BeginHorizontal(styman.layoutStyle);
			before=display24local;
			display24local = GUILayout.Toggle(display24local, "24hours", styman.toggleStyle);
			wndChanged = (before != display24local) || wndChanged;
			before=displaySecslocal;
			displaySecslocal = GUILayout.Toggle(displaySecslocal, "seconds", styman.toggleStyle);
			wndChanged = (before != displaySecslocal) || wndChanged;
			before=displayTZlocal;
			displayTZlocal = GUILayout.Toggle(displayTZlocal, "time zone", styman.toggleStyle);
			wndChanged = (before != displayTZlocal) || wndChanged;
			GUILayout.EndHorizontal();
		}
		
		before=displayrel;
		displayrel = GUILayout.Toggle(before, "Relative Time to Planet", styman.toggleStyle);
		wndChanged = (before != displayrel) || wndChanged;
		optChanged = (before != displayrel) || optChanged;
			
		if(displayrel)
		{
			GUILayout.BeginHorizontal(styman.layoutStyle);
			before=display24rel;
			display24rel = GUILayout.Toggle(display24rel, "24hours", styman.toggleStyle);
			wndChanged = (before != display24rel) || wndChanged;
			before=displaySecsrel;
			displaySecsrel = GUILayout.Toggle(displaySecsrel, "seconds", styman.toggleStyle);
			wndChanged = (before != displaySecsrel) || wndChanged;
			before=displayTZrel;
			displayTZrel = GUILayout.Toggle(displayTZrel, "time zone", styman.toggleStyle);
			wndChanged = (before != displayTZrel) || wndChanged;
			GUILayout.EndHorizontal();
		}
			
		before = displayksc;
		displayksc = GUILayout.Toggle(before, "KSC Time", styman.toggleStyle);
		wndChanged = (before != displayksc) || wndChanged;
		optChanged = (before != displayksc) || optChanged;
		
		if(displayksc)
		{
			GUILayout.BeginHorizontal(styman.layoutStyle);
			before = display24ksc;
			display24ksc = GUILayout.Toggle(display24ksc, "24hours", styman.toggleStyle);
			wndChanged = (before != display24ksc) || wndChanged;
			before = displaySecsksc;
			displaySecsksc = GUILayout.Toggle(displaySecsksc, "seconds", styman.toggleStyle);
			wndChanged = (before != displaySecsksc) || wndChanged;
			before = displayTZksc;
			displayTZksc = GUILayout.Toggle(displayTZksc, "time zone", styman.toggleStyle);
			wndChanged = (before != displayTZksc) || wndChanged;
			GUILayout.EndHorizontal();
		}
			
		before = displayreal;
		displayreal = GUILayout.Toggle(before, "Real Time", styman.toggleStyle);
		wndChanged = (before != displayreal) || wndChanged;

		if(displayreal)
		{
			GUILayout.BeginHorizontal(styman.layoutStyle);
			before = display24real;
			display24real = GUILayout.Toggle(display24real, "24hours", styman.toggleStyle);
			wndChanged = (before != display24real) || wndChanged;
			before = displaySecsreal;
			displaySecsreal = GUILayout.Toggle(displaySecsreal, "seconds", styman.toggleStyle);
			wndChanged = (before != displaySecsreal) || wndChanged;
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		
		if(wndChanged)
		{
			windowPos.height = 0;
			windowPos.width = 110;
		}
		if(optChanged)
		{
			optionsWindowPos.height = 0;
			optionsWindowPos.width = 0;
		}
		GUI.DragWindow(new Rect(0, 0, 10000, 20));
	}


	
	private void drawGUI()
	{
		print("Kerbal Local Time : " + this.GetInstanceID().ToString());
		if(toggled && this.vessel == FlightGlobals.ActiveVessel)
		{
			GUI.skin = HighLogic.Skin;
			styman.loadStyles();
			windowPos = GUILayout.Window(this.GetInstanceID(), windowPos, WindowGUI, "Kerbal Local Time");
		}
		if(options && this.vessel == FlightGlobals.ActiveVessel)
		{
			GUI.skin = HighLogic.Skin;
			styman.loadStyles();
			optionsWindowPos = GUILayout.Window(this.GetInstanceID()+1, optionsWindowPos, OptionsGUI, "KLT Options");
		}
	}
	
	[KSPEvent(guiActive = true, guiName = "Toggle display")]

	public void Toggle()
	{
		print("Kerbal Local Time : Toggled");
		toggled = !toggled;
	}
	
	[KSPAction("Toggle display", actionGroup = KSPActionGroup.None)]
	public void ToggleAction(KSPActionParam param)
	{
		Toggle();
	}
	
	[KSPEvent(guiActive = true, guiName = "Options")]
	
	public void ToggleOptions()
	{
		print("Kerbal Local Time : Options Toggled");
		options = !options;
	}
	
	[KSPAction("Options", actionGroup = KSPActionGroup.None)]
	public void ToggleOptionsAction(KSPActionParam param)
	{
		ToggleOptions();
	}
	
	protected override void onFlightStart()  //Called when vessel is placed on the launchpad
	{
		print("Kerbal Local Time : Flight Started");
		RenderingManager.AddToPostDrawQueue(3, new Callback(drawGUI));//start the GUI
	}
	protected override void onPartStart()
	{
		print("Kerbal Local Time : Part Started");
		loadConfig();
	}
	protected override void onPartDestroy() 
	{
		print("Kerbal Local Time : Part Destroyed");
		RenderingManager.RemoveFromPostDrawQueue(3, new Callback(drawGUI)); //close the GUI
		saveConfig();
	}
	protected double localSideralTime(CelestialBody body) //may be used later for planets rise
	{
		Orbit orb = body.orbit;
		//LAN + Arctan[ tan( tA + APe ) * cos(i) ]
		double vesselArgument = orb.trueAnomaly*Math.PI/180f + orb.argumentOfPeriapsis*Math.PI/180f;
		double locSidT;
		locSidT = orb.LAN + 180*Math.Atan(Math.Tan(vesselArgument)*Math.Cos(orb.inclination*Math.PI/180f))/Math.PI;
		
		if(vesselArgument > Math.PI / 2 && vesselArgument < 3*Math.PI/2)
			locSidT += 180;
		
		while(locSidT < 0)
			locSidT += 360;
		
		return locSidT % 360;
	}
	protected double sideralTime(CelestialBody body)
	{
		if(body.bodyName == "Sun")
			return 0;
		
		if(body.referenceBody.bodyName != "Sun")
			return sideralTime(body.referenceBody);
		else
			return localSideralTime(body);
	}
	protected double trueRelativeTime() //relative to reference body
	{
		if(vessel.mainBody.bodyName == "Sun")
			return 12;
		
		double trueT = (vessel.longitude+vessel.mainBody.rotationAngle - localSideralTime(vessel.mainBody))*24/360;

		while (trueT < 0)
			trueT += 24;

		return trueT % 24;
	}
	protected double trueTime()
	{
		if(vessel.mainBody.bodyName == "Sun")
			return 12;
		
		double trueT = (vessel.longitude+vessel.mainBody.rotationAngle - sideralTime(vessel.mainBody))*24/360;

		while (trueT < 0)
			trueT += 24;

		return trueT % 24;
	}
	protected double kmtRelativeTime() //relative to reference body
	{
		if(vessel.mainBody.bodyName == "Sun")
			return 12;
		
		double kmtT = (vessel.mainBody.rotationAngle - localSideralTime(vessel.mainBody))*24/360;

		while (kmtT < 0)
			kmtT += 24;

		return kmtT % 24;
	}
	protected double kmtTime()
	{
		if(vessel.mainBody.bodyName == "Sun")
			return 12;
		
		double kmtT = (vessel.mainBody.rotationAngle - sideralTime(vessel.mainBody))*24/360;

		while (kmtT < 0)
			kmtT += 24;

		return kmtT % 24;
	}
	protected double timeZone()
	{
		if(vessel.mainBody.bodyName == "Sun")
			return 0;
		
		double timeZ = Math.Round(vessel.longitude * 24 / 360);
		while(timeZ > 12)
			timeZ -= 24;
		while(timeZ < -11)
			timeZ += 24;
		return timeZ;
	}
	protected double localRelativeTime()
	{
		if(vessel.mainBody.bodyName == "Sun")
			return 12;
		
		double localT = kmtRelativeTime() + timeZone();

		while (localT < 0)
			localT += 24;

		return localT % 24;
	}
	protected double localTime()
	{
		if(vessel.mainBody.bodyName == "Sun")
			return 12;
		
		double localT = kmtTime() + timeZone();

		while (localT < 0)
			localT += 24;

		return localT % 24;
	}
	protected double kscTime()
	{
		CelestialBody kerbin = getKerbin();
		double kscT = (kerbin.rotationAngle - kerbin.orbit.trueAnomaly) * 24 / 360 - 5;//works because of Kerbin orbit

		while (kscT < 0)
			kscT += 24;

		return kscT % 24;
	}
	protected void realWorldTime()
	{
		if(displaystrings)
		{
			String realWorldT = "";
			if(display24real)
				realWorldT = System.DateTime.Now.ToLocalTime().ToString("HH:mm");
			else
				realWorldT = System.DateTime.Now.ToLocalTime().ToString("hh:mm");

			if(displaySecsreal)
				realWorldT += System.DateTime.Now.ToLocalTime().ToString(":ss");

			if(!display24real)
				realWorldT += System.DateTime.Now.ToLocalTime().ToString("tt", CultureInfo.InvariantCulture);

			GUILayout.Label(realWorldT);
		}
		else
		{
			double hour = System.DateTime.Now.Hour;
			double minute = System.DateTime.Now.Minute;
			double second = System.DateTime.Now.Second;
		
			double amState = 0;

			if(!display24real)
			{
				if(hour >= 12)
				{
					amState = 1;
					hour -= 12;
				}
				
				if(hour < 1)
					hour +=12;
			}
		
			double hourfirdig = Math.Floor(hour / 10);
			double hoursecdig = Math.Floor(hour) % 10;
		
			double minfirdig = Math.Floor(minute / 10);
			double minsecdig = Math.Floor(minute) % 10;
		
			double secfirdig = Math.Floor(second / 10);
			double secsecdig = Math.Floor(second) % 10;
		
			bool dispminsep = (System.DateTime.Now.Millisecond < 500) || displaySecsreal;
		
			GUILayout.BeginHorizontal();

			GUILayout.Label(texFromDigit(hourfirdig), styman.texStyle);
			GUILayout.Label(texFromDigit(hoursecdig), styman.texStyle);
			GUILayout.Label(dispminsep ? texman.getTexture("separator") : texman.getTexture("separator_black"), styman.texStyle);
			GUILayout.Label(texFromDigit(minfirdig), styman.texStyle);
			GUILayout.Label(texFromDigit(minsecdig), styman.texStyle);
			if(displaySecsreal)
			{
				GUILayout.Label(texman.getTexture("separator"), styman.texStyle);
				GUILayout.Label(texFromDigit(secfirdig), styman.texStyle);
				GUILayout.Label(texFromDigit(secsecdig), styman.texStyle);
			}
		
			if(!display24real)
			{
				GUILayout.Label((amState == 0) ? texman.getTexture("AM") : texman.getTexture("PM"), styman.texStyle);
			}
			
			Utils.drawblanks(texman, styman, "mid", displaySecsreal, !display24real, existsSecs(), existsAm(), existsBoth());
		
			GUILayout.EndHorizontal();
		}
	}
	protected void timeDoubleToDisplay(double time, double timeZone, bool display24, bool displaySecs, bool displayTZ, char TZfirstLetter)
	{
		double amState = 0;

		if(!display24)
		{
			if(time >= 12)
			{
				amState = 1;
				time -= 12;
			}
			
			if(time < 1)
				time +=12;
		}
		
		double timecpy = time;
		double hourfirdig = Math.Floor(time / 10);
		double hoursecdig = Math.Floor(time) % 10;
		
		timecpy = 60*(time - Math.Floor(time));
		
		double minfirdig = Math.Floor(timecpy / 10);
		double minsecdig = Math.Floor(timecpy) % 10;
		
		timecpy = 60*(timecpy - Math.Floor(timecpy));
		
		double secfirdig = Math.Floor(timecpy / 10);
		double secsecdig = Math.Floor(timecpy) % 10;
		
		double gmtSign = (timeZone < 0) ? -1 : 1;
		double timeZonecpy = timeZone * gmtSign;
		double gmtfirdig = Math.Floor(timeZonecpy / 10);
		double gmtsecdig = Math.Floor(timeZonecpy) % 10;
		
		bool dispminsep = (System.DateTime.Now.Millisecond < 500) || displaySecs;
		
		if(displaystrings)
		{
			String result = "";

			result += hourfirdig.ToString() + hoursecdig.ToString() + ":" + minfirdig.ToString() + minsecdig.ToString();

			if(displaySecs)
				result += ":" + secfirdig.ToString() + secsecdig.ToString();
			
			if(!display24)
				result += (amState == 0) ? "AM" : "PM";

			if(displayTZ)
				result += " (" + TZfirstLetter.ToString() + "MT" + ((timeZone >= 0) ? "+" : "") + timeZone.ToString() + ")";

			GUILayout.Label(result);
		}
		else
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();

			GUILayout.Label(texFromDigit(hourfirdig), styman.texStyle);
			GUILayout.Label(texFromDigit(hoursecdig), styman.texStyle);
			GUILayout.Label(dispminsep ? texman.getTexture("separator") : texman.getTexture("separator_black"), styman.texStyle);
			GUILayout.Label(texFromDigit(minfirdig), styman.texStyle);
			GUILayout.Label(texFromDigit(minsecdig), styman.texStyle);
			if(displaySecs)
			{
				GUILayout.Label(texman.getTexture("separator"), styman.texStyle);
				GUILayout.Label(texFromDigit(secfirdig), styman.texStyle);
				GUILayout.Label(texFromDigit(secsecdig), styman.texStyle);
			}
			
			if(!display24)
			{
				GUILayout.Label((amState == 0) ? texman.getTexture("AM") : texman.getTexture("PM"), styman.texStyle);
			}
			Utils.drawblanks(texman, styman, "mid", displaySecs, !display24, existsSecs(), existsAm(), existsBoth());
			
			GUILayout.EndHorizontal();
			if(displayTZ)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(texman.getTexture(TZfirstLetter.ToString()), styman.texStyle);
				GUILayout.Label(texman.getTexture("MT"), styman.texStyle);
				GUILayout.Label((gmtSign < 0) ? texman.getTexture("minus") : texman.getTexture("plus"), styman.texStyle);
				GUILayout.Label(texsFromDigit(gmtfirdig), styman.texStyle);
				GUILayout.Label(texsFromDigit(gmtsecdig), styman.texStyle);
				Utils.drawblanks(texman, styman, "bot", false, false, existsSecs(), existsAm(), existsBoth());
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			
		}
	}
	protected Texture2D texFromDigit(double digit)
	{
		return texman.getTexture(digit.ToString());
	}
	protected Texture2D texsFromDigit(double digit)
	{
		return texman.getTexture(digit.ToString() + "s");
	}
	protected CelestialBody getKerbin()
	{
		List<CelestialBody> planets = FlightGlobals.Bodies;

		foreach(CelestialBody body in planets)
		{
			if(body.bodyName == "Kerbin")
				return body;
		}
		return vessel.mainBody;
	}
	protected void saveConfig()
	{
		print("Kerbal Local Time : saving config");
		config.SetValue("windowPos", windowPos);
		config.SetValue("optionsWindowPos", optionsWindowPos);
		config.SetValue("toggled", toggled);
		config.SetValue("options", options);

		config.SetValue("displayksc", displayksc);
		config.SetValue("displaylocal", displaylocal);
		config.SetValue("displayrel", displayrel);
		config.SetValue("displayreal", displayreal);

		config.SetValue("display24ksc", display24ksc);
		config.SetValue("display24local", display24local);
		config.SetValue("display24rel", display24rel);
		config.SetValue("display24real", display24real);

		config.SetValue("displaySecsksc", displaySecsksc);
		config.SetValue("displaySecslocal", displaySecslocal);
		config.SetValue("displaySecsrel", displaySecsrel);
		config.SetValue("displaySecsreal", displaySecsreal);

		config.SetValue("displayTZksc", displayTZksc);
		config.SetValue("displayTZlocal", displayTZlocal);
		config.SetValue("displayTZrel", displayTZrel);

		config.save();
	}
	protected void loadConfig()
	{
		print("Kerbal Local Time : loading config");
		config = KSP.IO.PluginConfiguration.CreateForType<LocalTimePart>(null);
		config.load();

		windowPos = config.GetValue<Rect>("windowPos", new Rect(Screen.width / 2, Screen.height / 2, 0, 0));
		optionsWindowPos = config.GetValue<Rect>("optionsWindowPos", new Rect(Screen.width / 2, Screen.height / 2, 0, 0));

		toggled = config.GetValue<bool>("toggled", false);
		options = config.GetValue<bool>("options", false);
		
		displaystrings = config.GetValue<bool>("displaystrings", false);

		displayksc = config.GetValue<bool>("displayksc", true);
		displaylocal = config.GetValue<bool>("displaylocal", true);
		displayrel = config.GetValue<bool>("displayrel", false);
		displayreal = config.GetValue<bool>("displayreal", false);

		display24ksc = config.GetValue<bool>("display24ksc", false);
		display24local = config.GetValue<bool>("display24local", false);
		display24rel = config.GetValue<bool>("display24rel", false);
		display24real = config.GetValue<bool>("display24real", false);

		displaySecsksc = config.GetValue<bool>("displaySecsksc", false);
		displaySecslocal = config.GetValue<bool>("displaySecslocal", false);
		displaySecsrel = config.GetValue<bool>("displaySecsrel", false);
		displaySecsreal = config.GetValue<bool>("displaySecsreal", false);

		displayTZksc = config.GetValue<bool>("displayTZksc", true);
		displayTZlocal = config.GetValue<bool>("displayTZlocal", true);
		displayTZrel = config.GetValue<bool>("displayTZrel", false);
	}

}
