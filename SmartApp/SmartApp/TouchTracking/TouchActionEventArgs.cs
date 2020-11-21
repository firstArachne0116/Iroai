using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SmartApp.TouchTracking {

    public class TouchActionEventArgs : EventArgs {

        public TouchActionEventArgs(long id, TouchActionType type, List<Point> location, bool isInContact, int pointerCount) {
            Id = id;
            Type = type;
            Location = location;
            IsInContact = isInContact;
            PointerCount = pointerCount;
        }

        public long Id { private set; get; }

        public TouchActionType Type { private set; get; }

        public List<Point> Location { private set; get; }

        public bool IsInContact { private set; get; }

        public int PointerCount { private set; get; }
    }
}