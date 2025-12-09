#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
#endregion

namespace NinjaTrader.NinjaScript.Indicators
{
	public class MultiSessionShade : Indicator
	{
		private Brush brushAsia, brushLondon, brushNY;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Shades Asia, London, and NY sessions.";
				Name										= "MultiSessionShade";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= false;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				
				// --- ASIA Defaults (5pm - 2am CT) ---
				ShowAsia = true;
				AsiaStart = DateTime.Parse("17:00");
				AsiaEnd = DateTime.Parse("02:00");
				AsiaColor = Colors.SlateBlue;
				AsiaOpacity = 15;

				// --- LONDON Defaults (2am - 10:30am CT) ---
				ShowLondon = true;
				LondonStart = DateTime.Parse("02:00");
				LondonEnd = DateTime.Parse("10:30");
				LondonColor = Colors.DarkGreen;
				LondonOpacity = 15;

				// --- NY Defaults (8:30am - 3:15pm CT) ---
				ShowNY = true;
				NYStart = DateTime.Parse("08:30");
				NYEnd = DateTime.Parse("15:15");
				NYColor = Colors.DarkRed;
				NYOpacity = 15;
			}
			else if (State == State.Configure)
			{
				// Create brushes with opacity
				brushAsia = new SolidColorBrush(AsiaColor) { Opacity = AsiaOpacity / 100.0 };
				brushAsia.Freeze();

				brushLondon = new SolidColorBrush(LondonColor) { Opacity = LondonOpacity / 100.0 };
				brushLondon.Freeze();

				brushNY = new SolidColorBrush(NYColor) { Opacity = NYOpacity / 100.0 };
				brushNY.Freeze();
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 1) return;

			// Check and Draw Asia
			if (ShowAsia && IsTimeInSession(AsiaStart, AsiaEnd))
				BackBrush = brushAsia;

			// Check and Draw London
			// Note: If overlap exists, this will paint over Asia (blending if opacity < 100)
			if (ShowLondon && IsTimeInSession(LondonStart, LondonEnd))
				BackBrush = brushLondon;

			// Check and Draw NY
			if (ShowNY && IsTimeInSession(NYStart, NYEnd))
				BackBrush = brushNY;
		}

		// Helper function to check time ranges
		private bool IsTimeInSession(DateTime start, DateTime end)
		{
			TimeSpan barTime = Time[0].TimeOfDay;
			TimeSpan s = start.TimeOfDay;
			TimeSpan e = end.TimeOfDay;

			if (s > e) // Overnight session (e.g. 17:00 to 02:00)
				return barTime >= s || barTime < e;
			else       // Intraday session (e.g. 08:30 to 15:15)
				return barTime >= s && barTime < e;
		}

		#region Properties
		// --- ASIA SETTINGS ---
		[NinjaScriptProperty]
		[Display(Name="Show Asia", Order=1, GroupName="1. Asia Session")]
		public bool ShowAsia { get; set; }
		
		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="Asia Start", Order=2, GroupName="1. Asia Session")]
		public DateTime AsiaStart { get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="Asia End", Order=3, GroupName="1. Asia Session")]
		public DateTime AsiaEnd { get; set; }

		[XmlIgnore]
		[Display(Name="Asia Color", Order=4, GroupName="1. Asia Session")]
		public Color AsiaColor { get; set; }
		[Browsable(false)]
		public string AsiaColorSerialize { get { return AsiaColor.ToString(); } set { AsiaColor = (Color)ColorConverter.ConvertFromString(value); } }
		
		[NinjaScriptProperty]
		[Range(0, 100)]
		[Display(Name="Asia Opacity", Order=5, GroupName="1. Asia Session")]
		public int AsiaOpacity { get; set; }


		// --- LONDON SETTINGS ---
		[NinjaScriptProperty]
		[Display(Name="Show London", Order=1, GroupName="2. London Session")]
		public bool ShowLondon { get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="London Start", Order=2, GroupName="2. London Session")]
		public DateTime LondonStart { get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="London End", Order=3, GroupName="2. London Session")]
		public DateTime LondonEnd { get; set; }

		[XmlIgnore]
		[Display(Name="London Color", Order=4, GroupName="2. London Session")]
		public Color LondonColor { get; set; }
		[Browsable(false)]
		public string LondonColorSerialize { get { return LondonColor.ToString(); } set { LondonColor = (Color)ColorConverter.ConvertFromString(value); } }

		[NinjaScriptProperty]
		[Range(0, 100)]
		[Display(Name="London Opacity", Order=5, GroupName="2. London Session")]
		public int LondonOpacity { get; set; }


		// --- NY SETTINGS ---
		[NinjaScriptProperty]
		[Display(Name="Show NY", Order=1, GroupName="3. NY Session")]
		public bool ShowNY { get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="NY Start", Order=2, GroupName="3. NY Session")]
		public DateTime NYStart { get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="NY End", Order=3, GroupName="3. NY Session")]
		public DateTime NYEnd { get; set; }

		[XmlIgnore]
		[Display(Name="NY Color", Order=4, GroupName="3. NY Session")]
		public Color NYColor { get; set; }
		[Browsable(false)]
		public string NYColorSerialize { get { return NYColor.ToString(); } set { NYColor = (Color)ColorConverter.ConvertFromString(value); } }

		[NinjaScriptProperty]
		[Range(0, 100)]
		[Display(Name="NY Opacity", Order=5, GroupName="3. NY Session")]
		public int NYOpacity { get; set; }
		#endregion
	}
}