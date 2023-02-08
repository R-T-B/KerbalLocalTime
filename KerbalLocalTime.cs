using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using KSP.IO;
using UnityEngine;
using KSP.UI.Screens;

[KSPAddon(KSPAddon.Startup.EveryScene, false)]
public class LocalTimePart : MonoBehaviour
{
	StylesManager styman = new StylesManager();
	
	protected Rect windowPos;
	protected Rect optionsWindowPos;
	public static bool toggled = false;
	public static bool options = false;
	public static bool displayksc = true;
	public static bool displaylocal = true;
	public static bool displayrel = true;
	public static bool displayreal = true;
	public static bool display24ksc = true;
	public static bool display24local = true;
	public static bool display24rel = true;
	public static bool display24real = true;
	public static bool displaySecsksc = true;
	public static bool displaySecslocal = true;
	public static bool displaySecsrel = true;
	public static bool displaySecsreal = true;
	public static bool displayTZksc = true;
	public static bool displayTZlocal = true;
	public static bool displayTZrel = true;
	private static LocalTimePart instance;
	public static bool coreinited = false;
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
		try
		{
			String vesselMainBodyName = FlightGlobals.ActiveVessel.mainBody.bodyName;
			GUILayout.BeginVertical(styman.layoutStyle);
			if (displaylocal)
			{
				GUILayout.Label("Local Time", styman.timeLabelStyle);
				timeDoubleToDisplay(localTime(FlightGlobals.ActiveVessel.mainBody), timeZone(FlightGlobals.ActiveVessel.mainBody), display24local, displaySecslocal, displayTZlocal, FlightGlobals.ActiveVessel.mainBody.bodyName.First());
			}
			if (displayrel)
			{
				GUILayout.Label(FlightGlobals.ActiveVessel.mainBody.referenceBody.bodyName + " Related Time", styman.timeLabelStyle);
				if (FlightGlobals.ActiveVessel.mainBody.isStar)
				{
					timeDoubleToDisplay(localTime(FlightGlobals.ActiveVessel.mainBody), timeZone(FlightGlobals.ActiveVessel.mainBody), display24local, displaySecslocal, displayTZlocal, FlightGlobals.ActiveVessel.mainBody.bodyName.First());
				}
				else
				{
					timeDoubleToDisplay(localRelativeTime(FlightGlobals.ActiveVessel.mainBody.referenceBody), timeZone(FlightGlobals.ActiveVessel.mainBody.referenceBody), display24rel, displaySecsrel, displayTZrel, FlightGlobals.ActiveVessel.mainBody.referenceBody.bodyName.First());
				}
			}
			if (displayksc)
			{
				GUILayout.Label("KSC Time", styman.timeLabelStyle);
				timeDoubleToDisplay(kscTime(), 0, display24ksc, displaySecsksc, displayTZksc, 'K');
			}
			if (displayreal)
			{
				GUILayout.Label("Real World Time", styman.timeLabelStyle);
				realWorldTime();
			}
			GUILayout.EndVertical();
		}
		catch
		{
            try
            {
				Vessel veselTarget = PlanetariumCamera.fetch.target.vessel;
                String vesselMainBodyName = veselTarget.mainBody.bodyName;
                GUILayout.BeginVertical(styman.layoutStyle);
                if (displaylocal)
                {
                    GUILayout.Label("Local Time", styman.timeLabelStyle);
                    timeDoubleToDisplay(localTime(veselTarget.mainBody), timeZone(veselTarget.mainBody), display24local, displaySecslocal, displayTZlocal, veselTarget.mainBody.bodyName.First());
                }
                if (displayrel)
                {
                    GUILayout.Label(veselTarget.mainBody.referenceBody.bodyName + " Related Time", styman.timeLabelStyle);
                    if (veselTarget.mainBody.isStar)
                    {
                        timeDoubleToDisplay(localTime(veselTarget.mainBody), timeZone(veselTarget.mainBody), display24local, displaySecslocal, displayTZlocal, veselTarget.mainBody.bodyName.First());
                    }
                    else
                    {
                        timeDoubleToDisplay(localRelativeTime(veselTarget.mainBody.referenceBody), timeZone(veselTarget.mainBody.referenceBody), display24rel, displaySecsrel, displayTZrel, veselTarget.mainBody.referenceBody.bodyName.First());
                    }
                }
                if (displayksc)
                {
                    GUILayout.Label("KSC Time", styman.timeLabelStyle);
                    timeDoubleToDisplay(kscTime(), 0, display24ksc, displaySecsksc, displayTZksc, 'K');
                }
                if (displayreal)
                {
                    GUILayout.Label("Real World Time", styman.timeLabelStyle);
                    realWorldTime();
                }
                GUILayout.EndVertical();
            }
            catch
            {
                GUILayout.BeginVertical(styman.layoutStyle);
                if (displayksc)
                {
                    GUILayout.Label("KSC Time", styman.timeLabelStyle);
                    timeDoubleToDisplay(kscTime(), 0, display24ksc, displaySecsksc, displayTZksc, 'K');
                }
                GUILayout.EndVertical();
            }
        }

        //DragWindow makes the window draggable. The Rect specifies which part of the window it can by dragged by, and is 
        //clipped to the actual boundary of the window. You can also pass no argument at all and then the window can by
        //dragged by any part of it. Make sure the DragWindow command is AFTER all your other GUI input stuff, or else
        //it may "cover up" your controls and make them stop responding to the mouse.
        GUI.DragWindow();
 
	}
	
	private void OptionsGUI(int windowID)
	{
		bool before;
		bool wndChanged = false;
		bool optChanged = false;

		GUILayout.BeginVertical();
			
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
			if (windowPos.xMax > Screen.width)
			{
				windowPos.xMin = 0;
                windowPos.xMax = Screen.width /2;
            }
            if (windowPos.yMax > Screen.height)
            {
                windowPos.yMin = 0;
                windowPos.yMax = Screen.height / 2;
            }
            windowPos.height = 0;
            windowPos.width = 200 * GameSettings.UI_SCALE;
        }
		if(optChanged)
		{
            if (optionsWindowPos.xMax > Screen.width)
            {
                optionsWindowPos.xMin = 0;
                optionsWindowPos.xMax = Screen.width / 2;
            }
            if (optionsWindowPos.yMax > Screen.height)
            {
                optionsWindowPos.yMin = 0;
                optionsWindowPos.yMax = Screen.height / 2;
            }
            optionsWindowPos.height = 0;
			optionsWindowPos.width = 350 * GameSettings.UI_SCALE;
		}
		GUI.DragWindow(new Rect(0, 0, 10000, 20));
	}


	
	private void OnGUI()
	{
		if(toggled && ((HighLogic.LoadedSceneIsFlight) || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER) || HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION)))
        {
			styman.loadStyles();
			windowPos = GUILayout.Window(this.GetInstanceID(), windowPos, WindowGUI, "Kerbal Local Time",styman.windowStyle);
		}
		if(options && ((HighLogic.LoadedSceneIsFlight) || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER) || HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION)))
        {
			styman.loadStyles();
			optionsWindowPos = GUILayout.Window(this.GetInstanceID()+1, optionsWindowPos, OptionsGUI, "KLT Options", styman.windowStyle);
		}
	}

	public void Toggle()
	{
		if ((HighLogic.LoadedSceneIsFlight) || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER) || HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION))
		{
			if (toggled == true)
			{
				saveConfig(true,false);
			}
		}
        toggled = !toggled;
	}
	
	public void ToggleOptions()
	{
		if ((HighLogic.LoadedSceneIsFlight) || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER) || HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION))
		{
            if (options == true)
            {
                saveConfig(false,true);
            }
            options = !options;
		}
	}
	
	[KSPAction("Options", actionGroup = KSPActionGroup.None)]
	public void ToggleOptionsAction(KSPActionParam param)
	{
		ToggleOptions();
	}
	private void Start()
	{
		if (instance == null)
		{
			instance = this;
        }
		else
		{
            UnityEngine.Object.DestroyImmediate(this);
        }
		coreinited = true;
        loadConfig();
        windowPos.height = 0;
        windowPos.width = 200 * GameSettings.UI_SCALE;
        optionsWindowPos.height = 0;
        optionsWindowPos.width = 350 * GameSettings.UI_SCALE;
    }
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F7) && !(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)))
		{
			Toggle();
		}
        if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                ToggleOptions();
            }
        }
    }
	private void OnDestroy() 
	{
		if (coreinited)
		{
			if ((toggled == true) && ((HighLogic.LoadedSceneIsFlight) || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER) || HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION)))
			{
				saveConfig(false, false);
			}
			if ((options == true) && ((HighLogic.LoadedSceneIsFlight) || HighLogic.LoadedScene.Equals(GameScenes.SPACECENTER) || HighLogic.LoadedScene.Equals(GameScenes.TRACKSTATION)))
			{
				saveConfig(false, false);
			}
		}
	}
	protected static double localSolarTime(CelestialBody body) //may be used later for planets rise
	{
		double solarDayLength = body.solarDayLength;
		double time = Planetarium.GetUniversalTime() + (body.solarDayLength / 2);
		double day = (int)(time / solarDayLength);
		double TimeOfDay = ((time - (day * solarDayLength)) / solarDayLength) * 24;
		return TimeOfDay;

	}
	protected static double trueRelativeTime(CelestialBody body) //relative to reference body
	{
        if (body.isStar)
            return 12;

        return localSolarTime(body) + exactTimeZone(body);
    }
	protected static double trueTime(CelestialBody body)
	{
        if (body.isStar)
            return 12;

        return localSolarTime(body) + exactTimeZone(body);
    }
	protected static double kmtRelativeTime(CelestialBody body) //relative to reference body
	{
        if (body.isStar)
			return 12;
		
		return localSolarTime(body);
	}
	protected static double kmtTime(CelestialBody body)
	{
        if (body.isStar)
            return 12;
		
		return localSolarTime(body);
	}
    protected static double timeZone(CelestialBody body)
	{
		double timeZ;
		if (body.isStar)
		{
			return 0;
		}
		else
		{
            timeZ = Math.Round(body.GetLongitude(FlightGlobals.ActiveVessel.GetWorldPos3D(),true) * 24 / 360) + 5;
        }

		while(timeZ > 12)
			timeZ -= 24;
		while(timeZ < -11)
			timeZ += 24;
		return timeZ;
	}
	protected static double exactTimeZone(CelestialBody body)
    {
        double timeZ;
        if (body.isStar)
        {
            return 0;
        }
        else
        {
            timeZ = body.GetLongitude(FlightGlobals.ActiveVessel.GetWorldPos3D(), true) * 24 / 360 + 5;
        }

        while (timeZ > 12)
            timeZ -= 24;
        while (timeZ < -11)
            timeZ += 24;
        return timeZ;
    }
    protected static double localRelativeTime(CelestialBody body)
	{
		if(FlightGlobals.ActiveVessel.mainBody.isStar)
			return 12;
		
		double localT = kmtRelativeTime(body) + timeZone(body);

		while (localT < 0)
			localT += 24;

		return localT % 24;
	}
	protected static double localTime(CelestialBody body)
	{
		if(FlightGlobals.ActiveVessel.mainBody.isStar)
			return 12;
		
		double localT = kmtTime(body) + timeZone(body);

		while (localT < 0)
			localT += 24;

		return localT % 24;
	}
	protected static double kscTime()
	{
        return localSolarTime(FlightGlobals.GetHomeBody());
    }
	protected void realWorldTime()
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
	protected void timeDoubleToDisplay(double time, double timeZone, bool display24, bool displaySecs, bool displayTZ, char TZfirstLetter)
	{
        while (timeZone > 12)
            timeZone -= 24;
        while (timeZone < -11)
            timeZone += 24;
        double amState = 0;
		if (!display24)
		{
			if (time >= 12)
			{
				amState = 1;
				time -= 12;
			}
            if (time < 0)
            {
                time += 12;
            }
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
        String result = "";

        result += hourfirdig.ToString() + hoursecdig.ToString() + ":" + minfirdig.ToString() + minsecdig.ToString();

        if (displaySecs)
            result += ":" + secfirdig.ToString() + secsecdig.ToString();

        if (!display24)
            result += (amState == 0) ? "AM" : "PM";

        if (displayTZ)
            result += " (" + TZfirstLetter.ToString() + "MT" + ((timeZone >= 0) ? "+" : "") + Math.Round(timeZone).ToString() + ")";

        GUILayout.Label(result,styman.timeStyle);
	}
    public static string kscTimeStringToDisplay()
    {
		double time = kscTime();
		double timeZone = 0;
		bool display24 = display24ksc;
        bool displayTZ = displayTZksc;
		bool displaySecs = displaySecsksc;
        while (timeZone > 12)
            timeZone -= 24;
        while (timeZone < -11)
            timeZone += 24;
        double amState = 0;
        if (!display24)
        {
            if (time >= 12)
            {
                amState = 1;
                time -= 12;
            }
            if (time < 0)
            {
                time += 12;
            }
        }

        double timecpy = time;
        double hourfirdig = Math.Floor(time / 10);
        double hoursecdig = Math.Floor(time) % 10;

        timecpy = 60 * (time - Math.Floor(time));

        double minfirdig = Math.Floor(timecpy / 10);
        double minsecdig = Math.Floor(timecpy) % 10;

        timecpy = 60 * (timecpy - Math.Floor(timecpy));

        double secfirdig = Math.Floor(timecpy / 10);
        double secsecdig = Math.Floor(timecpy) % 10;

        double gmtSign = (timeZone < 0) ? -1 : 1;
        double timeZonecpy = timeZone * gmtSign;
        double gmtfirdig = Math.Floor(timeZonecpy / 10);
        double gmtsecdig = Math.Floor(timeZonecpy) % 10;

        bool dispminsep = (System.DateTime.Now.Millisecond < 500) || displaySecs;
        String result = "";

        result += hourfirdig.ToString() + hoursecdig.ToString() + ":" + minfirdig.ToString() + minsecdig.ToString();

        if (displaySecs)
            result += ":" + secfirdig.ToString() + secsecdig.ToString();

        if (!display24)
            result += (amState == 0) ? "AM" : "PM";

        if (displayTZ)
            result += " (" + "K" + "MT" + ((timeZone >= 0) ? "+" : "") + Math.Round(timeZone).ToString() + ")";

		return result;
    }
    protected CelestialBody getKerbin()
	{
		return FlightGlobals.GetHomeBody();
	}
	protected void saveConfig(bool forcetogglefalse, bool forceoptionsfalse)
	{
		print("Kerbal Local Time : saving config");
		config.SetValue("windowPos", windowPos);
		config.SetValue("optionsWindowPos", optionsWindowPos);
		if (!forcetogglefalse)
		{
			config.SetValue("toggled", toggled);
		}
		else
		{
            config.SetValue("toggled", false);
        }
		if (!forceoptionsfalse)
		{
            config.SetValue("options", options);
        }
		else
		{
            config.SetValue("options", false);
        }

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
