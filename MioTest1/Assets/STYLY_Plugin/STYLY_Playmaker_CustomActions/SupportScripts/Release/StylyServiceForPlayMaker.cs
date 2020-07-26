using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// handler class to handle requests from PlayMaker
    /// </summary>
    public interface IStylyServiceForPlayMakerImpl
    {
        void ChangeStylyScene(string sceneId, Action<Exception> onFinished);

        void OpenURL(string url, Action<Exception> onFinished);
    }

    /// <summary>
    /// A singleton hub class to use STYLY features from
    /// PlayMaker custom actions.
    /// </summary>
    public class StylyServiceForPlayMaker
    {
        private static StylyServiceForPlayMaker instance;

        public static StylyServiceForPlayMaker Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                instance = new StylyServiceForPlayMaker();
                return instance;
            }
        }

        private IStylyServiceForPlayMakerImpl impl;

        public void SetImpl(IStylyServiceForPlayMakerImpl impl)
        {
            this.impl = impl;
        }

        /// <summary>
        /// a service to change styly scene.
        /// </summary>
        /// <param name="sceneId">Scene Id (GUID format like: 1234abc-1234-abcd-ef12-012345abcdef)</param>
        public void ChangeStylyScene(string sceneId, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.ChangeStylyScene(sceneId, onFinished);
            }
            else
            {
                Debug.Log("ChangeStylyScene called with sceneId: <" + sceneId + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// a service to open web page.
        /// </summary>
        /// <param name=""></param>
        public void OpenURL(string url, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.OpenURL(url, onFinished);
            }
            else
            {
                Debug.Log("OpenURL called with url: <" + url + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }
    }
}
