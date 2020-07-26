#if PLAYMAKER

using System.Runtime.InteropServices;
using UnityEngine;
using STYLY;

namespace HutongGames.PlayMaker.Actions.STYLY
{
	[ActionCategory("STYLY")]
	[Tooltip("Open Url")]
	public class ApplicationOpenUrl : FsmStateAction
	{
		[RequiredField]
		public FsmString url;
		
		[Tooltip("Event to send if there's an error before open url.")]
		public FsmEvent errorEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Error message if there's an error before open url.")]
		public FsmString errorString;

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(url.Value))
			{
				Finish();
				return;
			}
			StylyServiceForPlayMaker.Instance.OpenURL(url.Value, error =>
			{
				if (error != null)
				{
					if (errorString != null)
					{
						errorString.Value = error.Message;
					}
					Fsm.Event(errorEvent);
					return;
				}
				Finish();
			});
			Finish();
		}

		public override void Reset()
		{
			url = "";
		}
	}
}

#endif
