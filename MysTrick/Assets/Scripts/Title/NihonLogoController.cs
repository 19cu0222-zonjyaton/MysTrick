using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class NihonLogoController : MonoBehaviour
{
	VideoPlayer videoPlayer;

	void Awake()
	{
		videoPlayer = GetComponent<VideoPlayer>();
	}

	void Start()
	{
		videoPlayer.loopPointReached += LoopPointReached;
		videoPlayer.Play();
	}

	public void LoopPointReached(VideoPlayer vp)
	{
		SceneManager.LoadScene("Title");
		videoPlayer.Stop();
	}
}
