using System;
using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using CSCore;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Networking;
using Utilla;

namespace MonkeNeedMusic
{
	/// <summary>
	/// This is your mod's main class.
	/// </summary>

	/* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
	[ModdedGamemode]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
		bool inRoom;
		public AssetBundle LoadAssetBundle(string path)
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			AssetBundle bundle = AssetBundle.LoadFromStream(stream);
			stream.Close();
			return bundle;
		}




		private void Start()
		{
            Events.GameInitialized += OnGameInitialized;
            StartCoroutine(startMusic());
        } 

		void OnEnable()
		{
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			HarmonyPatches.ApplyHarmonyPatches();
		}

		void OnDisable()
		{
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

			HarmonyPatches.RemoveHarmonyPatches();
		}

		private void OnGameInitialized(object sender, EventArgs e)
		{
			/* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */

			StartCoroutine(startMusic());

		}

		void Update()
		{
			/* Code here runs every frame when the mod is enabled */
			Debug.Log(Application.streamingAssetsPath);
		}

		/* This attribute tells Utilla to call this method when a modded room is joined */
		[ModdedGamemodeJoin]
		public void OnJoin(string gamemode)
		{
			/* Activate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = true;
		}

		/* This attribute tells Utilla to call this method when a modded room is left */
		[ModdedGamemodeLeave]
		public void OnLeave(string gamemode)
		{
			/* Deactivate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = false;
		}

		IEnumerator startMusic()
		{
			var bundle = LoadAssetBundle("monkeneedmusic.Resources.musiccube");
			var music = bundle.LoadAsset<GameObject>("musicCube");
			GameObject musicBox = Instantiate(music);
			string musicFileName = Application.streamingAssetsPath + "/music.mp3";
			musicBox.name = "XenonMusicMod";
			AudioSource audioSource = musicBox.GetComponent<AudioSource>();

			var dh = new DownloadHandlerAudioClip($"file://{musicFileName}", AudioType.MPEG);
			dh.compressed = true;

			using (UnityWebRequest wr = new UnityWebRequest($"file://{musicFileName}", "GET", dh, null))
			{
				yield return wr.SendWebRequest();
				if (wr.responseCode == 200)
				{
					audioSource.clip = dh.audioClip;
				}
			}
		}
	}
}

