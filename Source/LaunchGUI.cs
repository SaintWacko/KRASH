﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace KRASH
{
	[KSPAddon (KSPAddon.Startup.EditorAny, false)]
	public class LaunchGUI : MonoBehaviour
	{
		public const String TITLE = "KRASH";
		//private const int WIDTH = 725;
		//private const int HEIGHT = 425;
		//private Rect bounds = new Rect (Screen.width / 2 - WIDTH / 2, Screen.height / 2 - HEIGHT / 2, WIDTH, HEIGHT);
		private bool visible = false;
		private ApplicationLauncherButton button = null;
		private int INITORBIT = 10000;

		bool limitMaxCosts = false;
		public static LaunchGUI LaunchGuiInstance;

		const string texPathDefault = "KRASH/Textures/KRASH";
		//const string texPathOn = "KRASH/Textures/AppLauncherIcon-On";
		//const string texPathOff = "KRASH/Textures/AppLauncherIcon-Off";

		// Following code copied from KerbalKonstructs
		public enum SiteType
		{
			VAB,
			SPH,
			Any
		}

		enum SelectionType
		{
			launchsites,
			celestialbodies
		}

		private void Start ()
		{
			if (button == null) {
				OnGuiAppLauncherReady ();
			}
		}

		private void Awake ()
		{
			if (LaunchGUI.LaunchGuiInstance == null) {
				GameEvents.onGUIApplicationLauncherReady.Add (this.OnGuiAppLauncherReady);
				LaunchGuiInstance = this;
			}
		}

		private void OnDestroy ()
		{
			GameEvents.onGUIApplicationLauncherReady.Remove (this.OnGuiAppLauncherReady);
			if (this.button != null) {
				ApplicationLauncher.Instance.RemoveModApplication (this.button);
			}
		}

		private void ButtonState (bool state)
		{
//			Log.Info ("ApplicationLauncher on" + state.ToString ());

		}

		private void OnGuiAppLauncherReady ()
		{
			try {
				button = ApplicationLauncher.Instance.AddModApplication (
					GUIToggle, GUIToggle, 	
					null, null,
					null, null,
					ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, //visibleInScenes
					GameDatabase.Instance.GetTexture (texPathDefault, false) //texture
				);
			} catch (Exception ex) {
				Log.Error ("Error adding ApplicationLauncher button: " + ex.Message);
			}

		}


		//
		// The following function (only) is from KerbalEngineer
		// and is covered by the GPL V3 (license included with mod)
		//
		private void Update()
		{
			try
			{
				if (this.button == null)
				{
					return;
				}

				if (EditorLogic.fetch != null && EditorLogic.fetch.ship.parts.Count > 0)
				{
					if ( this.button.State == RUIToggleButton.ButtonState.DISABLED)
					{
						this.button.Enable();
					}
					//else if ( this.button.State != RUIToggleButton.ButtonState.FALSE)
					//{
					//	this.button.SetFalse();
					//}
				}
				else if (this.button.State != RUIToggleButton.ButtonState.DISABLED)
				{
					this.button.Disable();
				}
			}
			catch (Exception ex)
			{
				Log.Info("BuildToolbar->Update: " + ex);
			}
		}
		//
		// End of function from KER
		//


		public bool Visible ()
		{ 
			return this.visible;
		}

		public void SetVisible (bool visible)
		{
			this.visible = visible;
		}

		//bool isEditorLocked = false;
		private void EditorLock(bool state)
		{
			if (state)
			{
				InputLockManager.SetControlLock(ControlTypes.All, "KRASHEditorLock");
				EditorLogic.fetch.Lock (true, true, true, "KRASH_Editor");
				//isEditorLocked = true;
			}
			else
			{
				InputLockManager.SetControlLock(ControlTypes.None, "KRASHEditorLock");
				EditorLogic.fetch.Unlock ("KRASH_Editor");
				//isEditorLocked = false;
			}
		}

		public void GUIToggle ()
		{
			EditorLock (false);
			SetVisible (!visible);
			if (visible)
				APIManager.ApiInstance.SimMenuEvent.Fire ((Vessel)FlightGlobals.ActiveVessel, KRASHShelter.simCost);
		}

		//		Rect windowRect = new Rect(((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 570, 580);
		//Rect windowRect = new Rect (((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 425, 580);
		// ASH 28102014 - Needs to be bigger for filter
		Rect windowRect = new Rect (((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 570, 580);

		public void OnGUI ()
		{
			
			try {
				if (this.Visible ()) {
					drawSelector ();
//					windowRect = GUI.Window(0xB00B1E6, windowRect, drawSelectorWindow, "Launch Site Selector");
//					this.bounds = GUILayout.Window (this.GetInstanceID (), this.bounds, this.Window, TITLE, HighLogic.Skin.window);
				}
			} catch (Exception e) {
				Log.Error ("exception: " + e.Message);
			}
		}

		private void DrawTitle (String text)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.Label (text, HighLogic.Skin.label);
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
		}

		#if true

		[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
		public class PersistentKey : Attribute
		{
		}

		[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
		public class PersistentField : Attribute
		{
		}

		#if false
		public class LaunchSite
		{
			[PersistentKey]
			public string name;

			public string author;
			public SiteType type;
			public Texture logo;
			public Texture icon;
			public string description;

			public string category;
			public float opencost;
			public float closevalue;

			[PersistentField]
			public string openclosestate;

			public GameObject GameObject;
			public PSystemSetup.SpaceCenterFacility facility;

			public LaunchSite(string sName, string sAuthor, SiteType sType, Texture sLogo, Texture sIcon, string sDescription, string sDevice, float fOpenCost, float fCloseValue, string sOpenCloseState, GameObject gameObject, PSystemSetup.SpaceCenterFacility newFacility = null)
			{
				name = sName;
				author = sAuthor;
				type = sType;
				logo = sLogo;
				icon = sIcon;
				description = sDescription;
				category = sDevice;
				opencost = fOpenCost;
				closevalue = fCloseValue;
				openclosestate = sOpenCloseState;
				GameObject = gameObject;
				facility = newFacility;
			}
		}
		


#else
		public class LaunchSite
		{
			[PersistentKey]
			public string name;

//			public PSystemSetup.SpaceCenterFacility facility;
			string author;
			SiteType type;
			GameObject gameObject;

			public LaunchSite (string sName, string sAuthor, SiteType sType, GameObject sGameObject)
			{
				name = sName;
				author = sAuthor;
				type = sType;
				gameObject = sGameObject;
			}
			public SiteType GetSiteType()
			{
				return type;
			}
		}
		#endif
		public Vector2 sitesScrollPosition, bodiesScrollPosition;
		public Vector2 descriptionScrollPosition;
		//		Rect windowRect = new Rect(((Screen.width - Camera.main.rect.x) / 2) + Camera.main.rect.x - 125, (Screen.height / 2 - 250), 570, 580);
		//		private SiteType editorType = SiteType.Any;
		//		private bool orbitSelection = false;
		public List<LaunchSite> sites = new List<LaunchSite> ();

		List<CelestialBody> bodiesList;

		public static LaunchSite selectedSite = null;
		static SelectionType selectType = SelectionType.launchsites;

		public Boolean isCareerGame ()
		{
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				// disableCareerStrategyLayer is configurable in KerbalKonstructs.cfg
				//if (!KerbalKonstructs.instance.disableCareerStrategyLayer)
				{
					return true;
				}
				//	else
				//		return false;
			} else {
				return false;
			}
		}

		public bool inited = false;
		public static LaunchSite runway = new LaunchSite ("Runway", "Squad", SiteType.SPH, SpaceCenter.Instance.gameObject/*, new PSystemSetup.SpaceCenterFacility()*/);
		public static LaunchSite launchpad = new LaunchSite ("LaunchPad", "Squad", SiteType.VAB, SpaceCenter.Instance.gameObject/*, new PSystemSetup.SpaceCenterFacility()*/);

		void setOrbit(CelestialBody selectedBody)
		{
			if (simType != SimType.LANDED) {
				if (selectedBody.atmosphereDepth <= 1) {
					newAltitude = INITORBIT;
				} else {
					newAltitude = selectedBody.atmosphereDepth + 100;
				}
				altitude = newAltitude.ToString ();
			}
		}

		public void drawSelector ()
		{
			if (!inited) {
//				orbitSelection = false;
				selectType = SelectionType.launchsites;
				simType = SimType.LAUNCHPAD;;

				if (EditorDriver.editorFacility.Equals (EditorFacility.SPH)) {
					simType = SimType.RUNWAY;
					selectedSite = runway;
				} else {
					simType = SimType.LAUNCHPAD;
					selectedSite = launchpad;
				}
				inited = true;

				bodiesList = getAllowableBodies ();

				sites.Add (runway);
				sites.Add (launchpad);
			}

			if (sites.Count == 0) {
				Log.Info ("adding to sites");
				// (string sName, string sAuthor, SiteType sType, GameObject sGameObject)
				sites.Add (launchpad);
				sites.Add (runway);
				selectType = SelectionType.launchsites;
			}


			// Camera.main is null when first loading a scene
			if (Camera.main != null) {
				windowRect = GUI.Window (0xB00B1E6, windowRect, drawSelectorWindow, "Launch Site Selector");
			}

			if (windowRect.Contains (Event.current.mousePosition)) {
				EditorLock (true);
			} else {
				EditorLock (false);
			}
		}

		private enum ProgressItem
		{
			REACHED,
			ORBITED,
			LANDED,
			ESCAPED,
			RETURNED_FROM
		}

		public enum SimType
		{
			RUNWAY,
			LAUNCHPAD,
			LANDED,
			ORBITING
		}

		static public CelestialBody selectedBody = FlightGlobals.Bodies.Where (cb => cb.isHomeWorld).FirstOrDefault ();
		string altitude = "";
		string latitude = "", longitude = "";
		static public double newAltitude = 0.0, newLatitude = 0.0, newLongitude = 0.0;
		static public SimType simType = SimType.LANDED;
		Vector3 shipSize;

		List<CelestialBody> getAllowableBodies (String filter = "ALL")
		{
			CelestialBody parent;
			List<CelestialBody> bodiesList = new List<CelestialBody> ();
			CelestialBody[] tmpBodies = GameObject.FindObjectsOfType (typeof(CelestialBody)) as CelestialBody[]; 

			foreach (CelestialBody body in GameObject.FindObjectsOfType (typeof(CelestialBody))) {
				if (body.orbit != null && body.orbit.referenceBody != null) {
					parent = body.orbit.referenceBody;
				} else
					parent = null;
				switch (filter) {
				case "ALL":
					bodiesList.Add (body);
					break;
				case "PLANETS":
					if (parent == Sun.Instance.sun)
						bodiesList.Add (body);
					break;
				case "MOONS":
					if (parent != Sun.Instance.sun && parent != null)
						bodiesList.Add (body);
					break;
				default:
					bodiesList.Add (body);
					break;
				}
			}
			return bodiesList;
		}

		public void drawSelectorWindow (int id)
		{
			GUI.skin = HighLogic.Skin;
			//string smessage = "";
			//ScreenMessageStyle smsStyle = ScreenMessageStyle.UPPER_RIGHT;

			// ASH 28102014 Category filter handling added.
			// ASH 07112014 Disabling of restricted categories added.
			//GUILayout.BeginArea (new Rect (10, 25, 415, 550));
			GUILayout.BeginArea (new Rect (10, 25, 370, 545));

			GUILayout.BeginHorizontal ();

			// 			if (GUILayout.Button ("Current Launch Facility", GUILayout.Width (175))) {
			if (GUILayout.Button (FlightGlobals.Bodies.Where (cb => cb.isHomeWorld).FirstOrDefault ().name)) {
//				orbitSelection = false;
				if (EditorDriver.editorFacility.Equals (EditorFacility.VAB)) {
					selectType = SelectionType.launchsites;
					simType = SimType.LAUNCHPAD;
					selectedSite = launchpad;
				} else{
					selectType = SelectionType.launchsites;
					simType = SimType.RUNWAY;
					selectedSite = runway;
				}
			}

			if (GUILayout.Button ("Orbit selection")) {
				selectType = SelectionType.celestialbodies;
				simType = SimType.ORBITING;
				setOrbit(selectedBody);
				//newAltitude = selectedBody.atmosphereDepth + 100;
				//altitude = newAltitude.ToString ();
			}
						
			if (GUILayout.Button ("Landed")) {
				selectType = SelectionType.celestialbodies;
				simType = SimType.LANDED;

				// Make sure ship is high enough to avoid exploding inside the surface
				// Get largest dimension, then add 5 for safety
				shipSize = ShipConstruction.CalculateCraftSize(EditorLogic.fetch.ship);
				newAltitude = Math.Floor( Math.Max(shipSize.z, Math.Max(shipSize.y, shipSize.x))) + 5;

				altitude = newAltitude.ToString ();
			}

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			if (simType != SimType.LAUNCHPAD && simType != SimType.RUNWAY) {
				if (GUILayout.Button ("All", GUILayout.Width (45))) {
					selectType = SelectionType.celestialbodies;
					bodiesList = getAllowableBodies ("ALL");
					//bodies = GameObject.FindObjectsOfType (typeof(CelestialBody)) as CelestialBody[]; 
					//sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "RocketPad");
				}
				if (GUILayout.Button ("Planets", GUILayout.Width (60))) {
					bodiesList = getAllowableBodies ("PLANETS");
					selectType = SelectionType.celestialbodies;
					//				sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "RocketPad");
				}
				if (GUILayout.Button ("Moons", GUILayout.Width (60))) {
					bodiesList = getAllowableBodies ("MOONS");
					selectType = SelectionType.celestialbodies;
					//				sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "RocketPad");
				}
			} 

			GUILayout.EndHorizontal ();
			GUILayout.Space (10);


			// ASH 28102014 Category filter handling added
//			if (sites == null) sites = (editorType == SiteType.Any) ? LaunchSiteManager.getLaunchSites() : LaunchSiteManager.getLaunchSites(editorType, true, "ALL");

			if (selectType == SelectionType.launchsites) {
				#if true
				sitesScrollPosition = GUILayout.BeginScrollView (sitesScrollPosition);
				foreach (LaunchSite site in sites) {

//					if (isCareerGame ())
						GUILayout.BeginHorizontal ();
					// Light icons in the launchsite list only shown in career so only need horizontal for two elements for that mode

					GUI.enabled = !(selectedSite == site);
					if (GUILayout.Button (site.name, GUILayout.Height (30))) {
						selectedSite = site;

						// ASH Career Mode Unlocking
						// In career the launchsite is not set by the launchsite list but rather in the launchsite description
						// panel on the right
//					if (!isCareerGame())
						{
							//smessage = "Launchsite set to " + site.name;
							//ScreenMessages.PostScreenMessage (smessage, 10, smsStyle);
							if (selectedSite.GetSiteType() == SiteType.VAB)
								simType = SimType.LAUNCHPAD;
							else
								simType = SimType.RUNWAY;
								
							selectType = SelectionType.launchsites;
						
						}
					}
					GUI.enabled = true;
//					if (isCareerGame ()) {
//					// if site is closed show red light
//					// if site is open show green light

//					// If a site has an open cost of 0 it's always open
//					if (site.openclosestate == "Open" || site.opencost == 0)
//					{
//						site.openclosestate = "Open";
//						GUILayout.Label(tIconOpen, GUILayout.Height(30), GUILayout.Width(30));
//					}
//					else
//					{
//						GUILayout.Label(tIconClosed, GUILayout.Height(30), GUILayout.Width(30));
//					}
//					// Light icons in the launchsite list only shown in career
						GUILayout.EndHorizontal ();
//					}
				}
				GUILayout.EndScrollView ();
				#endif			
			} else {
				bodiesScrollPosition = GUILayout.BeginScrollView (bodiesScrollPosition);

				foreach (CelestialBody body in bodiesList) {
					KSPAchievements.CelestialBodySubtree tree = ProgressTracking.Instance.celestialBodyNodes.Where (node => node.Body == body).FirstOrDefault ();

					bool scienceOrbit;
					if (!isCareerGame())
						scienceOrbit = true;
					else
						scienceOrbit = ResearchAndDevelopment.GetSubjects ().Where (ss => ss.science > 0.0f && ss.IsFromBody (body) && ss.id.Contains ("InSpace")).Any ();

					if (!isCareerGame () || scienceOrbit || body.isHomeWorld ) {
						if (simType != SimType.LANDED || body.isHomeWorld || tree.landing.IsComplete) {
//						Log.Info ("body: " + body.name + "  is reached: " + tree.IsReached);
							GUI.enabled = !(selectedBody == body);
							if (GUILayout.Button (body.name, GUILayout.Height (30))) {
								selectedBody = body;

								setOrbit (selectedBody);

								//						LaunchSiteManager.setLaunchSite(site);
								//smessage = "Reference body set to " + body.name;
								//ScreenMessages.PostScreenMessage (smessage, 10, smsStyle);

							}
						}
					}
				}
				GUILayout.EndScrollView ();
			}


			GUILayout.EndArea ();

			GUI.enabled = true;
			drawRightSelectorWindow (selectType);
			GUI.DragWindow ();
		}

		void drawRightSelectorWindow (SelectionType type)
		{
			//string smessage = "";
			//ScreenMessageStyle smsStyle = (ScreenMessageStyle)2;
			//GUI.skin = HighLogic.Skin;

			GUI.skin.label.padding = new RectOffset (0, 0, 0, 0);
			GUILayout.BeginArea (new Rect (385, 25, 180, 545));
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			var oldColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.green;
			var bstyle = new GUIStyle (GUI.skin.button);
			bstyle.normal.textColor = Color.yellow;
			//bstyle.normal.background = new Texture2D(2,2);

			if (GUILayout.Button ("Start simulation", bstyle, GUILayout.Width (170.0f), GUILayout.Height (125.0f))) {
				GUI.backgroundColor = oldColor;
				EditorLock (false);
				//				Log.Info ("Active vessel: " + FlightGlobals.fetch.activeVessel.orbitDriver.ToString () + "   SelectedBody: " + selectedBody.ToString ());
				LaunchSim ();
				SetVisible (false);

				return;
			}
			GUI.backgroundColor = oldColor;
			GUILayout.EndHorizontal ();
			GUILayout.Space (15);
			GUILayout.BeginHorizontal ();
			//var style = new GUIStyle();
			//style.normal.textColor = Color.green;

			GUIStyle fontColorCyan = new GUIStyle(GUI.skin.label);
			fontColorCyan.normal.textColor = Color.cyan;

			GUILayout.Label ("Simulation Settings", fontColorCyan);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Start location:");
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			switch (type) {
			case SelectionType.launchsites:
				
				GUILayout.Box (FlightGlobals.Bodies.Where (cb => cb.isHomeWorld).FirstOrDefault ().name, GUILayout.Height(21));
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				switch (simType) {
				case SimType.LAUNCHPAD:
					GUILayout.Box ("Launchpad");
					break;
				case SimType.RUNWAY:
					GUILayout.Box ("Runway");
					break;
				}
				GUILayout.EndHorizontal ();
				break;
			case SelectionType.celestialbodies:
				if (selectedBody != null) {;
					GUILayout.Box (selectedBody.name);
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Altitude:");
					GUILayout.FlexibleSpace ();
					altitude = GUILayout.TextField (altitude, GUILayout.MinWidth (90.0F), GUILayout.MaxWidth (90.0F), GUILayout.Height(18));
					try {
						newAltitude = Convert.ToDouble (altitude);
					} catch (Exception) {
					} finally {
						altitude = newAltitude.ToString ();
						GUILayout.EndHorizontal ();
						if (newAltitude > selectedBody.sphereOfInfluence - selectedBody.Radius)
							newAltitude = selectedBody.sphereOfInfluence - selectedBody.Radius;

						if (newAltitude <= Math.Floor( Math.Max(shipSize.z, Math.Max(shipSize.y, shipSize.x))) + 1)
							newAltitude = Math.Floor( Math.Max(shipSize.z, Math.Max(shipSize.y, shipSize.x))) + 1;

					}
					if (simType == SimType.LANDED) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Latitude:");
						GUILayout.FlexibleSpace ();
						latitude = GUILayout.TextField (latitude, GUILayout.MinWidth (90.0F), GUILayout.MaxWidth (90.0F), GUILayout.Height(18));
						try {
							newLatitude = Convert.ToDouble (latitude);
						} catch (Exception) {
						} finally {
						}
						latitude = newLatitude.ToString ();
						GUILayout.EndHorizontal ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Longitude:");
						GUILayout.FlexibleSpace ();
						longitude = GUILayout.TextField (longitude, GUILayout.MinWidth (90.0F), GUILayout.MaxWidth (90.0F), GUILayout.Height(18));
						try {
							newLongitude = Convert.ToDouble (longitude);
						} catch (Exception) {
						} finally {
						}
						longitude = newLongitude.ToString ();
						GUILayout.EndHorizontal ();
					}
				}

				break;
		#if false
			case SelectionType.launchsites:
				if (selectedSite != null)
					GUILayout.Box(selectedSite.name);
				//GUILayout.Box("By " + selectedSite.author);
				//GUILayout.Box(selectedSite.logo);
				descriptionScrollPosition = GUILayout.BeginScrollView(descriptionScrollPosition);
				GUI.enabled = false;
				//GUILayout.TextArea(selectedSite.description);//, GUILayout.ExpandHeight(true));
				GUILayout.TextArea("Test selectedSite description");
				GUI.enabled = true;
				GUILayout.EndScrollView();

				break;
		#endif
			}
			GUILayout.Space (10);
			GUIStyle fontColorYellow =new GUIStyle(GUI.skin.label);
			fontColorYellow.normal.textColor = Color.yellow;
			GUIStyle fontColorStd = new GUIStyle(GUI.skin.label);
			//GUIStyle fontColorCyan = new GUIStyle(GUI.skin.label);
			//fontColorCyan.normal.textColor = Color.cyan;

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Part count:");
			GUILayout.FlexibleSpace ();
			GUILayout.Label (EditorLogic.fetch.ship.parts.Count.ToString (), fontColorYellow);
			GUILayout.EndHorizontal ();

			float dryMass, fuelMass;
			EditorLogic.fetch.ship.GetShipMass(out dryMass, out fuelMass);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Dry Mass:");
			GUILayout.FlexibleSpace ();

			GUILayout.Label(dryMass.ToString(), fontColorYellow);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Fuel Mass:");
			GUILayout.FlexibleSpace ();
			GUILayout.Label(fuelMass.ToString(), fontColorYellow);
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Total Mass:");
			GUILayout.FlexibleSpace ();
			GUILayout.Label((dryMass + fuelMass).ToString(), fontColorYellow);
			GUILayout.EndHorizontal ();
			GUILayout.Space (10);
			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Simulation Costs:", fontColorCyan);
				GUILayout.EndHorizontal ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Sim setup cost:");
				GUILayout.FlexibleSpace ();
				double simSetupCost = Math.Floor (KRASH.cfg.flatSetupCost +
				                     EditorLogic.fetch.ship.parts.Count * KRASH.cfg.perPartSetupCost +
				                     (dryMass + fuelMass) * KRASH.cfg.perTonSetupCost);
			
				GUILayout.Label (simSetupCost.ToString (), fontColorYellow);
				GUILayout.EndHorizontal ();


				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Est. Sim/min cost:");
				GUILayout.FlexibleSpace ();

				float estSimPerMin = KRASH.cfg.flatPerMinCost +
				                    EditorLogic.fetch.ship.parts.Count * KRASH.cfg.perPartPerMinCost +
				                    (dryMass + fuelMass) * KRASH.cfg.perTonPerMinCost;
			
				GUILayout.Label (estSimPerMin.ToString (), fontColorYellow);
				GUILayout.EndHorizontal ();
				float m = KRASH.cfg.AtmoMultipler;
				float estSimAtmoPerMin = estSimPerMin;
				
				if (m < 1.0F)
					m = 1.0F;
				if (m > 1.0) {
					estSimAtmoPerMin = (float)Math.Floor (estSimPerMin * m + 1.0);
					GUILayout.BeginHorizontal (GUILayout.Height (18));
					GUILayout.Label ("Est. Atmo Sim/min cost:");
					GUILayout.FlexibleSpace ();

					GUILayout.Label (estSimAtmoPerMin.ToString (), fontColorYellow);
					GUILayout.EndHorizontal ();
				}


				GUILayout.BeginHorizontal ();
				limitMaxCosts = GUILayout.Toggle (limitMaxCosts, "Limit max costs");
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
				if (limitMaxCosts) {
					if (KRASHShelter.LimitSimCost == 0) {
						KRASHShelter.LimitSimCost = Math.Floor (simSetupCost + KRASH.cfg.DefaultSimTime * estSimAtmoPerMin);
					}
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Limit: ");
					GUILayout.FlexibleSpace ();
					double f;
					string s = GUILayout.TextField (KRASHShelter.LimitSimCost.ToString (), GUILayout.MinWidth (90.0F), GUILayout.MaxWidth (90.0F), GUILayout.Height (18));
					try {
						f = Convert.ToDouble (s);
					} catch (Exception) {
						f = KRASHShelter.LimitSimCost;
					} finally {
					}
					if (f < simSetupCost)
						f = simSetupCost;
					KRASHShelter.LimitSimCost = f;
					GUILayout.EndHorizontal ();
				}
			}

			GUILayout.FlexibleSpace ();
			GUILayout.BeginHorizontal ();
			bstyle = new GUIStyle (GUI.skin.button);
			bstyle.normal.textColor = Color.yellow;
			//bstyle.normal.background = new Texture2D(2,2);
			GUI.backgroundColor = Color.red;

			if (GUILayout.Button ("Cancel", bstyle, GUILayout.Width (170.0f), GUILayout.Height (30.0f))) {
				GUI.backgroundColor = oldColor;
				GUIToggle ();
				return;
			}
			GUI.backgroundColor = oldColor;
			GUILayout.EndHorizontal ();
			//GUILayout.FlexibleSpace ();
			GUILayout.EndVertical ();
			GUILayout.EndArea ();
			GUI.DragWindow (new Rect (0, 0, 10000, 10000));
		}
		#endif

		public  void setLaunchSite ( LaunchSite site)
		{
			Log.Info ("setLaunchSite");
			Log.Info ("simType: " + simType.ToString ());
			Log.Info ("site.name: " + site.name);
			KRASHShelter.bodiesListAtSimStart = getAllowableBodies ("ALL");

			if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER) {
				KRASHShelter.preSimStatus = new List<PreSimStatus> ();
				foreach (CelestialBody body in KRASHShelter.bodiesListAtSimStart) {
					PreSimStatus s = new PreSimStatus ();
					s.flightsGlobalIndex = body.flightGlobalsIndex;
					KSPAchievements.CelestialBodySubtree tree = ProgressTracking.Instance.celestialBodyNodes.Where (node => node.Body == body).FirstOrDefault ();
					s.isReached = tree.IsReached;
					;
					s.landed = tree.landing.IsComplete;

					bool scienceFlying = ResearchAndDevelopment.GetSubjects ().Where (ss => ss.science > 0.0f && ss.IsFromBody (body) && ss.id.Contains ("Flying")).Any ();
					s.scienceFromAtmo = scienceFlying;

					bool scienceOrbit = ResearchAndDevelopment.GetSubjects ().Where (ss => ss.science > 0.0f && ss.IsFromBody (body) && ss.id.Contains ("InSpace")).Any ();
					s.scienceFromOrbit = scienceOrbit; 

					KRASHShelter.preSimStatus.Add (s);
					s = null;
				}
			}
			// Debug.Log("KK: EditorLogic.fetch.launchSiteName set to " + site.name);
			//Trick KSP to think that you launched from Runway or LaunchPad
			//I'm sure Squad will break this in the future
			//This only works because they use multiple variables to store the same value, basically its black magic.
			//--medsouz
				if (simType == SimType.LAUNCHPAD || simType == SimType.RUNWAY) {
					Log.Info ("site.name: " + site.name);
					EditorLogic.fetch.launchSiteName = site.name;
				}

//			}
		}

		public void LaunchSim ()
		{
			Log.Info ("LaunchSim");
			KRASH.instance.Activate ();
			EditorLogic.fetch.launchVessel ();
		}

	}
}