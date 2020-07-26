using System;
using UnityEngine;

namespace STYLY.Interaction.SDK.V1
{
    /// <summary>
    /// FeatureComponentに実装部をバインドするシングルトンクラス。
    /// setupperGetterを渡して初期化し、利用する。
    /// </summary>
    public class ImplBinder
    {
        private Func<Type, IImplSetupper> setupperGetter;

        private bool initialized = false;
        private static ImplBinder instance;

        public static ImplBinder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImplBinder();
                }

                return instance;
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="setupperGetter">Typeに応じたImpleSetupperを返すDelegate</param>
        public void Init(Func<Type, IImplSetupper> setupperGetter)
        {
            this.setupperGetter = setupperGetter;
            initialized = true;
        }

        /// <summary>
        /// 実装部のバインド処理
        /// </summary>
        /// <param name="component">対象コンポーネント</param>
        public void Bind(Component component)
        {
            if (!initialized)
            {
                Debug.Log("Never initialized.");
                return;
            }

            IImplSetupper implSetupper = setupperGetter(component.GetType());

            if (implSetupper != null)
            {
                implSetupper.Setup(component);
            }
            else
            {
                Debug.Log("Setupper is not found!:" + component);
            }
        }
    }
}