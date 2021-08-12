/**
 * MetroFramework - Modern UI for WinForms
 * 
 * The MIT License (MIT)
 * Copyright (c) 2011 Sven Walter, http://github.com/viperneo
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in the 
 * Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 * and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 * OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Windows.Forms;

namespace MetroFramework.Animation
{
    public delegate void AnimationAction();
    public delegate bool AnimationFinishedEvaluator();

    public abstract class AnimationBase
    {
        public event EventHandler AnimationCompleted;
        private void OnAnimationCompleted()
        {
            AnimationCompleted?.Invoke(this, EventArgs.Empty);
        }

        private DelayedCall timer;

        private AnimationAction actionHandler;
        private AnimationFinishedEvaluator evaluatorHandler;

        protected TransitionType transitionType;
        protected int counter;
        protected int startTime;
        protected int targetTime;

        public bool IsCompleted
        {
            get
            {
                return timer == null || !timer.IsWaiting;
            }
        }
        public bool IsRunning
        {
            get
            {
                return timer != null && timer.IsWaiting;
            }
        }

        public void Cancel()
        {
            if (IsRunning)
                timer.Cancel();
        }

        protected void Start(Control control, TransitionType transitionType, int duration, AnimationAction actionHandler)
        {
            Start(control, transitionType, duration, actionHandler, null);
        }

        protected void Start(Control control, TransitionType transitionType, int duration, AnimationAction actionHandler, AnimationFinishedEvaluator evaluatorHandler)
        {
            this.transitionType = transitionType;
            this.actionHandler = actionHandler;
            this.evaluatorHandler = evaluatorHandler;

            counter = 0;
            startTime = 0;
            targetTime = duration;

            timer = DelayedCall.Start(DoAnimation, duration);
        }

        private void DoAnimation()
        {
            if (evaluatorHandler == null || evaluatorHandler.Invoke())
            {
                OnAnimationCompleted();
            }
            else
            {
                actionHandler.Invoke();
                counter++;

                timer.Start();
            }
        }

        protected int MakeTransition(float t, float b, float d, float c)
        {
            return transitionType switch
            {
                TransitionType.Linear => (int)((c * t / d) + b),// simple linear tweening - no easing 
                TransitionType.EaseInQuad => (int)((c * (t /= d) * t) + b),// quadratic (t^2) easing in - accelerating from zero velocity
                TransitionType.EaseOutQuad => (int)((-c * (t /= d) * (t - 2)) + b),// quadratic (t^2) easing out - decelerating to zero velocity
                TransitionType.EaseInOutQuad => (t /= d / 2) < 1 ? (int)((c / 2 * t * t) + b) : (int)((-c / 2 * (((--t) * (t - 2)) - 1)) + b),// quadratic easing in/out - acceleration until halfway, then deceleration
                TransitionType.EaseInCubic => (int)((c * (t /= d) * t * t) + b),// cubic easing in - accelerating from zero velocity
                TransitionType.EaseOutCubic => (int)((c * (((t = (t / d) - 1) * t * t) + 1)) + b),// cubic easing in - accelerating from zero velocity
                TransitionType.EaseInOutCubic => (t /= d / 2) < 1 ? (int)((c / 2 * t * t * t) + b) : (int)((c / 2 * (((t -= 2) * t * t) + 2)) + b),// cubic easing in - accelerating from zero velocity
                TransitionType.EaseInQuart => (int)((c * (t /= d) * t * t * t) + b),// quartic easing in - accelerating from zero velocity
                TransitionType.EaseInExpo => t == 0 ? (int)b : (int)((c * Math.Pow(2, 10 * ((t / d) - 1))) + b),// exponential (2^t) easing in - accelerating from zero velocity
                TransitionType.EaseOutExpo => t == d ? (int)(b + c) : (int)((c * (-Math.Pow(2, -10 * t / d) + 1)) + b),// exponential (2^t) easing out - decelerating to zero velocity
                _ => 0,
            };
        }
    }
}
