using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using SmartApp.TouchTracking;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ResolutionGroupName("SmartApp")]
[assembly: ExportEffect(typeof(SmartApp.Droid.PlatformSpecific.TouchEffect), "TouchEffect")]
namespace SmartApp.Droid.PlatformSpecific
{
    public class TouchEffect : PlatformEffect
    {
        Android.Views.View view;
        Element _formsElement;
        TouchTracking.TouchEffect _pclTouchEffect;

        bool _capture;
        Func<double, double> _fromPixels;
        int[] twoIntArray = new int[2];

        static Dictionary<Android.Views.View, TouchEffect> viewDictionary = new Dictionary<Android.Views.View, TouchEffect>();
        static Dictionary<int, TouchEffect> idToEffectDictionary = new Dictionary<int, TouchEffect>();

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the PCL
            TouchTracking.TouchEffect touchEffect =
                (TouchTracking.TouchEffect)Element.Effects.
                    FirstOrDefault(e => e is TouchTracking.TouchEffect);

            if (touchEffect != null && view != null)
            {
                viewDictionary.Add(view, this);

                _formsElement = Element;

                _pclTouchEffect = touchEffect;

                // Save fromPixels function
                _fromPixels = view.Context.FromPixels;

                // Set event handler on View
                view.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            try
            {
                if (viewDictionary.ContainsKey(view))
                {
                    viewDictionary.Remove(view);
                    view.Touch -= OnTouch;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        void OnTouch(object sender, Android.Views.View.TouchEventArgs args)
        {
            // Two object common to all the events
            Android.Views.View senderView = sender as Android.Views.View;
            MotionEvent motionEvent = args.Event;

            // Get the pointer index
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            int id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(twoIntArray);

            List<Point> points = new List<Point>();
            for (int i = 0; i < motionEvent.PointerCount; i ++)
            {
                Point screenPointerCoords = new Point(twoIntArray[0] + motionEvent.GetX(i),
                    twoIntArray[1] + motionEvent.GetY(i));
                points.Add(screenPointerCoords);
            }
            
            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, points, true);

                    idToEffectDictionary.Add(id, this);

                    _capture = _pclTouchEffect.Capture;
                    break;

                case MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (_capture)
                        {

                            FireEvent(this, id, TouchActionType.Moved, points, true);
                        }
                        else
                        {
                            CheckForBoundaryHop(id, points);

                            if (idToEffectDictionary[id] != null)
                            {
                                FireEvent(idToEffectDictionary[id], id, TouchActionType.Moved, points, true);
                            }
                        }
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Released, points, false);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, points);

                        if (idToEffectDictionary[id] != null)
                        {
                            FireEvent(idToEffectDictionary[id], id, TouchActionType.Released, points, false);
                        }
                    }
                    idToEffectDictionary.Remove(id);
                    break;

                case MotionEventActions.Cancel:
                    if (_capture)
                    {
                        FireEvent(this, id, TouchActionType.Cancelled, points, false);
                    }
                    else
                    {
                        if (idToEffectDictionary[id] != null)
                        {
                            FireEvent(idToEffectDictionary[id], id, TouchActionType.Cancelled, points, false);
                        }
                    }
                    idToEffectDictionary.Clear();
                    break;
            }
        }

        void CheckForBoundaryHop(int id, List<Point> pointerLocation)
        {
            TouchEffect touchEffectHit = null;

            foreach (Android.Views.View view in viewDictionary.Keys)
            {
                // Get the view rectangle
                try
                {
                    view.GetLocationOnScreen(twoIntArray);
                }
                catch // System.ObjectDisposedException: Cannot access a disposed object.
                {
                    continue;
                }
                Rectangle viewRect = new Rectangle(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

                if (viewRect.Contains(pointerLocation[pointerLocation.Count - 1]))
                {
                    touchEffectHit = viewDictionary[view];
                }
            }

            if (touchEffectHit != idToEffectDictionary[id])
            {
                if (idToEffectDictionary[id] != null)
                {
                    FireEvent(idToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
                }
                if (touchEffectHit != null)
                {
                    FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
                }
                idToEffectDictionary[id] = touchEffectHit;
            }
        }

        void FireEvent(TouchEffect touchEffect, int id, TouchActionType actionType, List<Point> pointerLocation, bool isInContact)
        {
            // Get the method to call for firing events
            Action<Element, TouchActionEventArgs> onTouchAction = touchEffect._pclTouchEffect.OnTouchAction;

            List<Point> points = new List<Point>();
            for (int i = 0; i < pointerLocation.Count; i++)
            {
                double x = pointerLocation[i].X - twoIntArray[0];
                double y = pointerLocation[i].Y - twoIntArray[1];
                Point point = new Point(_fromPixels(x), _fromPixels(y));
                points.Add(point);
            }

            // Call the method
            onTouchAction(touchEffect._formsElement,
                new TouchActionEventArgs(id, actionType, points, isInContact, points.Count));
        }
    }
}