using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressTimer : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How long the countdown (fill) takes, in seconds.")]
    [SerializeField] private float duration = 1f;

    [Tooltip("How long to hold at progress = 1 after finishing, before restarting. Same idea as duration, but for the 'finished/hold' phase.")]
    [SerializeField] private float finishedHoldDuration = 1f;

    [Tooltip("If true, starts counting down automatically on Start().")]
    [SerializeField] private bool playOnStart = false;

    [Tooltip("If true, uses Time.unscaledDeltaTime instead of Time.deltaTime (ignores Time.timeScale / pausing).")]
    [SerializeField] private bool useUnscaledTime = false;

    [Tooltip("If true, after holding at progress = 1 for finishedHoldDuration, automatically resets and restarts the fill, looping forever until Stop() is called.")]
    [SerializeField] private bool loop = true;

    [Header("Events")]
    [Tooltip("Invoked every frame while filling, with progress in [0,1]. Hook this up to set a material float property, fill amount, etc.")]
    public UnityEvent<float> OnProgress;

    [Tooltip("Invoked once when progress reaches 1 (fill finished, hold phase begins). Use this to trigger a UI effect that plays during the hold/wait.")]
    public UnityEvent OnProgressFinished;

    [Tooltip("Invoked once when the hold/wait (finishedHoldDuration) ends, right before the next fill starts (or before stopping, if not looping).")]
    public UnityEvent OnHoldFinished;

    [Tooltip("Invoked once when the countdown starts/restarts (fill phase begins).")]
    public UnityEvent OnStarted;

    public float progress { get; private set; }

    public bool isRunning { get; private set; }

    public bool isHolding { get; private set; }

    public float Duration => duration;

    public float FinishedHoldDuration => finishedHoldDuration;

    private Coroutine routine;

    public Image uiImage;

    private Material materialInstance;

    private static readonly int ProgressProperty = Shader.PropertyToID("_Progress");

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }

        if (uiImage != null)
        {
            materialInstance = uiImage.materialForRendering;
        }
    }

    public void Play()
    {
        Play(duration);
    }

    public void Play(float customDuration)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        duration = Mathf.Max(0.0001f, customDuration);
        routine = StartCoroutine(RunCycle());
    }

    public void Stop()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        isRunning = false;
        isHolding = false;
    }

    public void Reset()
    {
        Stop();
        progress = 0f;
        OnProgress?.Invoke(progress);
    }

    public void SetDuration(float seconds)
    {
        duration = Mathf.Max(0.0001f, seconds);
    }

    public void SetFinishedHoldDuration(float seconds)
    {
        finishedHoldDuration = Mathf.Max(0f, seconds);
    }

    private IEnumerator RunCycle()
    {
        do
        {
            yield return FillRoutine();
            OnProgressFinished?.Invoke();
            yield return HoldRoutine();
        }
        while (loop);

        isRunning = false;
        routine = null;
    }

    private IEnumerator FillRoutine()
    {
        isRunning = true;
        progress = 0f;
        OnStarted?.Invoke();
        OnProgress?.Invoke(progress);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            progress = Mathf.Clamp01(elapsed / duration);
            OnProgress?.Invoke(progress);
            yield return null;
        }

        progress = 1f;
        OnProgress?.Invoke(progress);
    }

    private IEnumerator HoldRoutine()
    {
        isHolding = true;
        float elapsed = 0f;

        while (elapsed < finishedHoldDuration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        isHolding = false;
        OnHoldFinished?.Invoke();
    }

    public void SetProgress()
    {
        if (materialInstance == null) return;
        materialInstance.SetFloat(ProgressProperty, progress);
    }
}