using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource beatSource;
    [SerializeField] private AudioSource sfxSource;

    private void OnEnable()
    {
        GameEventManager.Instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.AddListener<SimulateCountDownEnded>(OnSimulationCountdownEnded);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveListener<GameStateChangedEvent>(OnGameStateChanged);
        GameEventManager.Instance.RemoveListener<SimulateCountDownEnded>(OnSimulationCountdownEnded);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if(e.stateType == GameStateType.SimulateLevel)
        {
            //Simulate starts
            //Play rewind here
            sfxSource.Play();
            beatSource.Stop();
        }
    }

    private void OnSimulationCountdownEnded(SimulateCountDownEnded e)
    {
        //Reset the beat audio here
        //beatSource.Reset();
        beatSource.Play();
    }

}
