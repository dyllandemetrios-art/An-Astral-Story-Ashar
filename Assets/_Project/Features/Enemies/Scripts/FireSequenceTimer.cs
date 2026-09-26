namespace Ashar.Enemies
{
    /// <summary>What the timer asks the shooter to do this frame.</summary>
    public enum FireSequenceEvent
    {
        /// <summary>Nothing to do.</summary>
        None,

        /// <summary>The warning line should start being shown.</summary>
        TelegraphStarted,

        /// <summary>One shot must be fired now.</summary>
        Fire,
    }

    /// <summary>
    /// The clock of an enemy's firing: when to warn, when to fire, and how the shots of a burst are spaced.
    /// RESPONSIBILITIES: turn the passing of time into "start the warning" and "fire now" events.
    /// HOW IT WORKS: a cycle is: wait, then (if there is a warning time) warn, then fire. A burst fires several times,
    /// one burst spacing apart, and then waits again. The interval is the time from the start of one cycle to the start
    /// of the next, so it includes the warning and the burst. Call Advance() once per frame with the frame time.
    /// WHY: it has no Unity code, so the timing can be unit-tested exactly, and the shooter that uses it stays simple.
    /// </summary>
    public class FireSequenceTimer
    {
        private readonly float _interval;        // Time from the start of one cycle to the start of the next.
        private readonly float _telegraphTime;   // Warning time before the first shot of a cycle (0 = none).
        private readonly int _shotsPerCycle;     // Shots fired in one cycle: the burst count, or 1.
        private readonly float _shotSpacing;     // Time between two shots of a burst.

        private float _wait;                     // Time left before the next thing happens while waiting.
        private float _telegraphLeft;            // Warning time left while warning.
        private int _shotsLeft;                  // Shots left to fire in the current burst.
        private float _spacingLeft;              // Time left before the next shot of the burst.
        private State _state = State.Waiting;    // What the timer is doing now.

        private enum State
        {
            Waiting,
            Telegraphing,
            Bursting,
        }

        /// <summary>Creates a timer. shotsPerCycle is 1 unless the pattern is a burst.</summary>
        public FireSequenceTimer(float interval, float telegraphTime, int shotsPerCycle, float shotSpacing)
        {
            _interval = interval;
            _telegraphTime = telegraphTime;
            _shotsPerCycle = shotsPerCycle < 1 ? 1 : shotsPerCycle;
            _shotSpacing = shotSpacing;
            Restart(0f);
        }

        /// <summary>True while the warning line should be shown.</summary>
        public bool IsTelegraphing => _state == State.Telegraphing;

        /// <summary>Progress of the current warning from 0 (just started) to 1 (about to fire); 0 when not warning.</summary>
        public float TelegraphProgress => _state == State.Telegraphing && _telegraphTime > 0f ? 1f - _telegraphLeft / _telegraphTime : 0f;

        /// <summary>Starts a new cycle. skipSeconds shortens the first wait, so several enemies do not fire in step.</summary>
        public void Restart(float skipSeconds)
        {
            _state = State.Waiting;
            _wait = System.Math.Max(0f, CycleWait() - skipSeconds);
        }

        /// <summary>Advances the clock by one frame and returns what the shooter must do now.</summary>
        public FireSequenceEvent Advance(float deltaTime)
        {
            switch (_state)
            {
                case State.Waiting:
                    _wait -= deltaTime;
                    if (_wait > 0f)
                    {
                        return FireSequenceEvent.None;
                    }

                    if (_telegraphTime > 0f)
                    {
                        _state = State.Telegraphing;
                        _telegraphLeft = _telegraphTime;
                        return FireSequenceEvent.TelegraphStarted;
                    }

                    return BeginFiring(_wait);

                case State.Telegraphing:
                    _telegraphLeft -= deltaTime;
                    return _telegraphLeft > 0f ? FireSequenceEvent.None : BeginFiring(_telegraphLeft);

                default: // Bursting
                    _spacingLeft -= deltaTime;
                    if (_spacingLeft > 0f)
                    {
                        return FireSequenceEvent.None;
                    }

                    _shotsLeft--;
                    if (_shotsLeft <= 0)
                    {
                        FinishCycle(_spacingLeft);
                    }
                    else
                    {
                        _spacingLeft += _shotSpacing;
                    }

                    return FireSequenceEvent.Fire;
            }
        }

        /// <summary>
        /// Fires the first shot of the cycle, and either finishes the cycle or starts the burst. The overshoot is the time
        /// by which the last frame went past the exact moment of the shot (zero or negative): it is carried over so the
        /// firing rate stays exact instead of losing a part of a frame at every shot.
        /// </summary>
        private FireSequenceEvent BeginFiring(float overshoot)
        {
            _shotsLeft = _shotsPerCycle - 1;
            if (_shotsLeft <= 0)
            {
                FinishCycle(overshoot);
            }
            else
            {
                _state = State.Bursting;
                _spacingLeft = _shotSpacing + overshoot;
            }

            return FireSequenceEvent.Fire;
        }

        /// <summary>Goes back to waiting, for the rest of the interval, minus the overshoot carried over from the last shot.</summary>
        private void FinishCycle(float overshoot)
        {
            _state = State.Waiting;
            float used = _telegraphTime + (_shotsPerCycle - 1) * _shotSpacing;
            _wait = System.Math.Max(0f, _interval - used + overshoot);
        }

        /// <summary>Time to wait at the start of a cycle before the warning: the interval minus the warning and the burst.</summary>
        private float CycleWait()
        {
            float used = _telegraphTime + (_shotsPerCycle - 1) * _shotSpacing;
            return System.Math.Max(0f, _interval - used);
        }
    }
}
