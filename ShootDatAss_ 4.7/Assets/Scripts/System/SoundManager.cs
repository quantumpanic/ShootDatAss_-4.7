using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour 
{
	[System.Serializable]
	public class SoundInfo
	{
		public AudioClip audioClip;
		public string audioName;
	}
	
	public enum BgmState
	{
		STOP = 0,
		PLAY,
		PAUSED
	};
	
	public static SoundManager instance;
	
	public List<SoundInfo> soundList = new List<SoundInfo>();
	
	
	private AudioSource _BGM;
	private AudioSource _SFX;
	
	private int _bgmVolume;
	private int _sfxVolume;
	private bool _bgmIsMute;
	private bool _sfxIsMute;
	private BgmState _bgmState = BgmState.STOP;
	private string _playingBgmName;
	private string _onHoldBGM;
	private Dictionary<string, AudioClip> _soundList = new Dictionary<string, AudioClip>();
	
	// BGM Volume starts from 0 - 100
	public int bgmVolume
	{
		get 
		{ 
			return _bgmVolume;
		}
		set 
		{
			_bgmVolume = Mathf.Clamp (value,0,100);
			PlayerPrefs.SetInt("BgmVolume",_bgmVolume);
			PlayerPrefs.Save();
			if ( _BGM != null)
			{
				_BGM.volume = _bgmVolume / 100.0f;
			}
		}
	}
	
	// SFX Volume starts from 0 - 100
	public int sfxVolume
	{
		get 
		{ 
			return _sfxVolume;
		}
		set 
		{
			_sfxVolume = Mathf.Clamp (value,0,100);
			PlayerPrefs.SetInt("SfxVolume",_sfxVolume);
			PlayerPrefs.Save();
		}
	}
	
	// True to mute BGM
	public bool bgmMute
	{
		get 
		{ 
			return _bgmIsMute;
		}
		set 
		{
			_bgmIsMute = value;
			PlayerPrefs.SetInt("BgmMute",_bgmIsMute?1:0);
			PlayerPrefs.Save();
			if ( _bgmIsMute) 
				_BGM.enabled = false;
			else
			{
				_BGM.enabled = true;
				Debug.Log(_onHoldBGM);
				PlayBgm(_onHoldBGM);
			}
		}
	}
	
	// True to mute SFX
	public bool sfxMute
	{
		get
		{ 
			return _sfxIsMute;
		}
		set 
		{
			_sfxIsMute = value;
			PlayerPrefs.SetInt("SfxMute",_sfxIsMute?1:0);
			PlayerPrefs.Save();
		}
	}
	
	public BgmState bgmState
	{
		get
		{
			return _bgmState;
		}
	}
	
	public string playingBgmName
	{
		get
		{
			if ( _bgmState == BgmState.PLAY)
				return _playingBgmName;
			else
				return string.Empty;
		}
	}
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		_BGM = gameObject.AddComponent<AudioSource>();
		_bgmVolume = PlayerPrefs.GetInt("BgmVolume",70);
		_sfxVolume = PlayerPrefs.GetInt("SfxVolume",100);
		_bgmIsMute = (PlayerPrefs.GetInt("BgmMute", 0) == 1);
		_sfxIsMute = (PlayerPrefs.GetInt("SfxMute", 0) == 1);
		bgmVolume = 50;
		_BGM.volume = 0.5f;
		if ( _bgmIsMute) 
			_BGM.enabled = false;
		else
			_BGM.enabled = true;
		
		foreach ( SoundInfo info in soundList)
		{
			_soundList.Add(info.audioName, info.audioClip);
		}
	}
	
	/// <summary>
	/// Plaies the bgm.
	/// </summary>
	/// <param name='name'>
	/// Name of the sound.
	/// </param>
	public void PlayBgm(string name)
	{
		_onHoldBGM = name;
		if ( _bgmIsMute) 
		{
			return;
		}
		if ( _BGM)
		{
			if ( _soundList.ContainsKey(name))
			{
				_BGM.clip = _soundList[name];
				_BGM.loop = true;
				_BGM.Play ();
				_playingBgmName = name;
				_bgmState = BgmState.PLAY;
			}
			else
			{
				Debug.Log(string.Format("PlayBgm error, Sound with name '{0}' not found in sound list",name));
			}
		}
	}
	
	/// <summary>
	/// Play sfx attached to gameObject.
	/// </summary>
	/// <param name='name'>
	/// Name of the sound.
	/// <param name='go'>
	/// Object to attach to.
	/// </param>
	public AudioSource PlaySfx(string name,GameObject go = null, bool loop = false)
	{
		if ( _soundList.ContainsKey(name))
		{
			if ( !_sfxIsMute)
			{
				if (go == null) go = GameObject.Find("listenerObject");
				return PlayAudioClip(_soundList[name],go.transform,_sfxVolume/100.0f, loop);
			}
			else return null;
		}
		else
        {
            Debug.Log(string.Format("PlaySfx error, Sound with name '{0}' not found in sound list", name));
			return null;
		}
	}

	AudioSource PlayAudioClip(AudioClip clip, Transform emitter, float volume, bool loop) {
		GameObject go = new GameObject("One shot audio");
		go.transform.parent = emitter;
		go.transform.localPosition = Vector3.zero;
        AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 0;
		source.minDistance = 10f;
		source.maxDistance = 12f;
		source.dopplerLevel = 0;
		source.rolloffMode = AudioRolloffMode.Linear;
		source.clip = clip;
		source.volume = volume;
		source.loop = loop;
		source.Play();
		if (!source.loop) Destroy(go, clip.length);
		return source;
	}

	/// <summary>
	/// Plays sfx at location.
	/// </summary>
	/// <param name='name'>
	/// Name of the sound.
	/// <param name='pos'>
	/// Position to play the sound.
	/// </param>
	public AudioSource PlaySfx(string name,Vector3 pos, bool loop = false)
	{
		if ( _soundList.ContainsKey(name))
		{
			if ( !_sfxIsMute)
			{
				return PlayAudioClip(_soundList[name], pos, _sfxVolume/100.0f, loop);
			}
			else return null;
		}
		else
        {
            Debug.Log(string.Format("PlaySfx error, Sound with name '{0}' not found in sound list", name));
			return null;
		}
	}
	
	AudioSource PlayAudioClip(AudioClip clip, Vector3 pos, float volume, bool loop) {
		GameObject go = new GameObject("One shot audio");
		go.transform.position = pos;
		AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 0;
		source.minDistance = 10f;
		source.maxDistance = 12f;
		source.dopplerLevel = 0;
		source.rolloffMode = AudioRolloffMode.Linear;
		source.clip = clip;
		source.volume = volume;
		source.loop = loop;
		source.Play();
		if (!source.loop) Destroy(go, clip.length);
		return source;
	}
	
	public void PauseBgm(bool pause)
	{
		if ( _bgmIsMute) return;
		if ( pause && _bgmState == BgmState.PLAY)
		{
			_BGM.Pause();
			_bgmState = BgmState.PAUSED;
		}
		else if ( !pause && _bgmState == BgmState.PAUSED)
		{
			_BGM.Play ();
			_bgmState = BgmState.PLAY;
		}
	}
	
	public void StopBgm()
	{
		if ( _bgmIsMute) return;
		if ( _bgmState != BgmState.STOP)
		{
			_BGM.Stop();
			_bgmState = BgmState.STOP;
		}
	}
	
	public void TempSetBGMVolume(float newVolume)
	{
		if( _BGM)
		{
			_BGM.volume = newVolume;
		}
	}
	
}
