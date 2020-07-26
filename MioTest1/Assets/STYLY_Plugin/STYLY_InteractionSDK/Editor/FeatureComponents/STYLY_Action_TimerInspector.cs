using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// STYLY_Action_Timerのインスペクターエディタ拡張。
    /// </summary>
    [CustomEditor( typeof(STYLY_Action_Timer))]
    public class STYLY_Action_TimerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            STYLY_Action_Timer target = (STYLY_Action_Timer) this.target;
            
            target.TimerType = (TimerTypeEnum)EditorGUILayout.EnumPopup("Timer Type", target.TimerType);            
            if (target.TimerType == TimerTypeEnum.Constant)
            {
                target.Time = EditorGUILayout.FloatField("Time", target.Time);
            }
            else if (target.TimerType == TimerTypeEnum.RandomInRange)
            {
                target.MinTime = EditorGUILayout.FloatField("Min time", target.MinTime);
                target.MaxTime = EditorGUILayout.FloatField("Max time", target.MaxTime);
            }
            else
            {
                Debug.LogError("Unknown TimerType!");
            }
        }
    }
}
