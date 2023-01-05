#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Gamepangin
{
    public static class EditorCoroutines {
     
        public class Coroutine {
            public IEnumerator enumerator;
            public Action<bool> onUpdate;
            public List<IEnumerator> history = new();
        }
     
        static readonly List<Coroutine> Coroutines = new();
     
        public static void Execute (IEnumerator enumerator, Action<bool> onUpdate = null) {
            if (Coroutines.Count == 0) {
                EditorApplication.update += Update;
            }
            var coroutine = new Coroutine { enumerator = enumerator, onUpdate = onUpdate };
            Coroutines.Add (coroutine);
        }
     
        static void Update () {
            for (int i = 0; i < Coroutines.Count; i++) {
                var coroutine = Coroutines[i];
                bool done = !coroutine.enumerator.MoveNext ();
                if (done) {
                    if (coroutine.history.Count == 0) {
                        Coroutines.RemoveAt (i);
                        i--;
                    } else {
                        done = false;
                        coroutine.enumerator = coroutine.history[coroutine.history.Count - 1];
                        coroutine.history.RemoveAt (coroutine.history.Count - 1);
                    }
                } else {
                    if (coroutine.enumerator.Current is IEnumerator) {
                        coroutine.history.Add (coroutine.enumerator);
                        coroutine.enumerator = (IEnumerator)coroutine.enumerator.Current;
                    }
                }
                if (coroutine.onUpdate != null) coroutine.onUpdate (done);
            }
            if (Coroutines.Count == 0) EditorApplication.update -= Update;
        }
     
        internal static void StopAll () {
            Coroutines.Clear ();
            EditorApplication.update -= Update;
        }
     
    }
}

#endif